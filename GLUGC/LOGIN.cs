using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace GLUGC
{
    public partial class LOGIN : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleparam = base.CreateParams;
                handleparam.ExStyle |= 0x02000000;
                return handleparam;
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.DoubleBuffered = true;
        }
        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }
        private const int WM_NCHITTEST = 0x84;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int CS_DBLCLKS = 0x8;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;



        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }



        [DllImport("dwmapi.dll")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsCompositionEnabled()
        {
            if (Environment.OSVersion.Version.Major < 6) return false;

            bool enabled;
            DwmIsCompositionEnabled(out enabled);

            return enabled;
        }



        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
         );

        private bool CheckIfAeroIsEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);

                return (enabled == 1) ? true : false;
            }
            return false;
        }


        public void ApplyShadows(Form form)
        {
            var v = 2;

            DwmSetWindowAttribute(form.Handle, 2, ref v, 4);

            MARGINS margins = new MARGINS()
            {
                bottomHeight = 1,
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0
            };

            DwmExtendFrameIntoClientArea(form.Handle, ref margins);
        }
        public LOGIN()
        {
            InitializeComponent();
            ApplyShadows(this);
            this.FormBorderStyle = FormBorderStyle.None;
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 13, 13));
        }

        public static string[] GetLocalIPv4(NetworkInterfaceType _type)
        {
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                            MessageBox.Show(ip.Address.ToString());
                        }
                    }
                }
            }
            return ipAddrList.ToArray();


        }
        int num;
        public void random1()
        {
            Random rnd2 = new Random();
            int month1 = rnd2.Next(aa, bb);
            num = month1;
        }
        private string version;
        int aa = 0;
        int bb = 0;
        private void LOGIN_Load(object sender, EventArgs e)
        {

           
            try
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    String queryA = "SELECT * FROM tblSettings where id = 1";
                    SqlCommand cmdA = new SqlCommand(queryA, GCGLU);
                    SqlDataReader drA = cmdA.ExecuteReader();

                    if (drA.Read())
                    {
                        aa = Convert.ToInt32(drA["start"].ToString());
                        bb = Convert.ToInt32(drA["limit"].ToString());
                    }
                    GCGLU.Close();
                    random1();

                    GCGLU.Open();
                    String queryAv = "SELECT * FROM tblWallpaper where name = '"+num+"'";
                    SqlCommand camdA = new SqlCommand(queryAv, GCGLU);
                    SqlDataReader dsarA = camdA.ExecuteReader();

                    if (dsarA.Read())
                    {
                        if (dsarA["image"] != DBNull.Value)
                        {
                            byte[] img = (byte[])(dsarA["image"]);
                            MemoryStream mstream = new MemoryStream(img);
                            this.BackgroundImage = System.Drawing.Image.FromStream(mstream);
                            //guna2PictureBox1.Image = imgsql;
                        }
                    }
                    GCGLU.Close();



                    //this.BackgroundImage = 
                    var fileName1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\systemVER.vsn";

                    version = "";
                    if (File.Exists(fileName1))
                    {
                        string text = File.ReadAllText(fileName1);
                        version = text.ToString().Trim();
                    }

                    GCGLU.Open();
                    String query = "SELECT * FROM updatestat where id = 1";
                    SqlCommand cmd = new SqlCommand(query, GCGLU);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        if (dr["version"].ToString() != version)
                        {
                            DialogResult dialogResult1 = MessageBox.Show("Updates Available", "Update?", MessageBoxButtons.YesNo);
                            if (dialogResult1 == DialogResult.Yes)
                            {
                                Process[] workerssa = Process.GetProcessesByName("GLUGCBack(Do not Close)");
                                foreach (Process worker in workerssa)
                                {
                                    worker.Kill();
                                    worker.WaitForExit();
                                    worker.Dispose();
                                }
                                string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\AppUpdator";
                                //MessageBox.Show(path + "\AppUpdator.exe");
                                Process.Start(path + "\\SystemAppUpdator.exe");


                                Process[] workers = Process.GetProcessesByName("GLUGC");
                                foreach (Process worker in workers)
                                {
                                    worker.Kill();
                                    worker.WaitForExit();
                                    worker.Dispose();
                                }

                            }
                        }
                        else
                        {
                            version = dr["version"].ToString();
                        }

                    }
                    dr.Close();
                    GCGLU.Close();


                }
            }
            catch
            {
                timer1.Start();
                guna2ProgressIndicator1.Start();
                guna2ProgressIndicator1.Visible = true;
            }


            Process[] workersa = Process.GetProcessesByName("GLUGCBack(Do not Close)");
            foreach (Process worker in workersa)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }

            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue("GLUGC", Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Cannot connect to JED-PC Server. Please contact Jed!!Thanks");
            }

            try
            {
                if (Properties.Settings.Default.remember == "true")
                {
                    using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                    {
                        GCGLU.Open();
                        String query1 = "SELECT * FROM tblAccounts where Id like '" + Properties.Settings.Default.accid.Trim() + "'";
                        SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                        SqlDataReader dr1 = cmd2.ExecuteReader();
                        Form1 f = new Form1();
                        if (dr1.Read())
                        {
                            f.version = version;
                            f.id = dr1["Id"].ToString();
                            f.type = dr1["type"].ToString();
                        }
                        dr1.Close();
                        GCGLU.Close();
                        this.Hide();
                        f.ShowDialog();
                    }
                }
            }
            catch
            {
                timer1.Start();
                guna2ProgressIndicator1.Start();
                guna2ProgressIndicator1.Visible = true;
            }

        }

        //private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        //private static readonly string StartupValue = "GLUGC";
        //public void enforceAdminPrivilegesWorkaround()
        //{

        //    try
        //    {
        //        RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
        //        key.SetValue(StartupValue, Application.ExecutablePath.ToString());
        //    }
        //    catch (System.Security.SecurityException ex)
        //    {
        //        MessageBox.Show("Please run as administrator");
        //        System.Environment.Exit(1);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message);
        //    }
        //}
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            login();
        }
        private void login()
        {
            try
            {
                if (guna2ProgressIndicator1.Visible == false)
                {
                    using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                    {
                        GCGLU.Open();
                        SqlCommand cmd = new SqlCommand("select username,password from tblAccounts where username = '" + guna2TextBox1.Text + "' COLLATE SQL_Latin1_General_CP1_CS_AS  and password = '" + guna2TextBox2.Text + "' COLLATE SQL_Latin1_General_CP1_CS_AS", GCGLU);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            GCGLU.Close();
                            getdata();
                        }
                        else
                        {
                            GCGLU.Close();
                            getnumber();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void getdata()
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                GCGLU.Open();
                String query1 = "SELECT * FROM tblAccounts where username like '" + guna2TextBox1.Text.Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                SqlDataReader dr1 = cmd2.ExecuteReader();
                Form1 f = new Form1();
                if (dr1.Read())
                {
                    f.version = version;
                    f.id = dr1["Id"].ToString();
                    f.type = dr1["type"].ToString();
                    if (guna2CustomCheckBox1.Checked)
                    {
                        Properties.Settings.Default.remember = "true";
                        Properties.Settings.Default.accid = dr1["Id"].ToString();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default.remember = "false";
                        Properties.Settings.Default.accid = "";
                        Properties.Settings.Default.Save();
                    }
                }
                dr1.Close();
                GCGLU.Close();
                this.Hide();
                f.ShowDialog();
            }
        }
        private void getnumber()
        {
            try
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    SqlCommand cmd = new SqlCommand("select number,password from tblAccounts where number = '" + guna2TextBox1.Text + "' COLLATE SQL_Latin1_General_CP1_CS_AS  and password = '" + guna2TextBox2.Text + "' COLLATE SQL_Latin1_General_CP1_CS_AS", GCGLU);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        GCGLU.Close();
                        getdatanum();
                    }
                    else
                    {
                        GCGLU.Close();
                        MessageBox.Show("Invalid Login please check username and password");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void getdatanum()
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                GCGLU.Open();
                String query1 = "SELECT * FROM tblAccounts where number like '" + guna2TextBox1.Text.Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                SqlDataReader dr1 = cmd2.ExecuteReader();
                Form1 f = new Form1();
                if (dr1.Read())
                {
                    f.version = version;
                    //f.username = guna2TextBox1.Text;
                    f.id = dr1["Id"].ToString();
                    f.type = dr1["type"].ToString();

                    if (guna2CustomCheckBox1.Checked)
                    {
                        Properties.Settings.Default.remember = "true";
                        Properties.Settings.Default.accid = dr1["Id"].ToString();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default.remember = "false";
                        Properties.Settings.Default.accid = "";
                        Properties.Settings.Default.Save();
                    }
                }
                dr1.Close();
                GCGLU.Close();
                this.Hide();
                f.ShowDialog();
            }
        }
        private void label3_Click(object sender, EventArgs e)
        {
            signup s = new signup();
            s.ShowDialog();
        }

        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                login();
                e.Handled = true;
            }
        }

        private void guna2TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                login();
                e.Handled = true;
            }
        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
      
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            //MessageBox.Show(path + "\AppUpdator.exe");
            Process.Start(path + "\\GLUGCBack(Do not Close).exe");

            Process[] workers = Process.GetProcessesByName("GLUGC");
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }
            //WindowStyle = WindowStyle.SingleBorderWindow;
            //WindowState = WindowState.Minimized;
            //FormBorderStyle = FormBorderStyle.FixedSingle;
            //WindowState = FormWindowState.Minimized;
            //FormBorderStyle = FormBorderStyle.None;
        }

        private void guna2CustomCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox2.Checked == true)
            {
                //string a = textBox2.Text;
                guna2TextBox2.PasswordChar = '\0';
            }
            else
            {
                guna2TextBox2.PasswordChar = '●';
            }
        }

        private void guna2CustomCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
         
        }

        private void LOGIN_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] workersa = Process.GetProcessesByName("GLUGCBack(Do not Close)");
            foreach (Process worker in workersa)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }
        }
        int a = 0;
        Thread updating;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (updating == null)
                {
                    updating =
                      new Thread(new ThreadStart(checker));
                    updating.Start();
                }
            }
            catch
            {
            }
        }
        bool ok = true;
        private void checker()
        {
            try
            {
                //label2.Text = a++.ToString();
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    GCGLU.Close();
                    ok = true;
                    if (ok == true)
                    {
                        guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                        {
                            guna2ProgressIndicator1.Stop();
                            guna2ProgressIndicator1.Visible = false;
                        });
                    }
                }
                updating = null;
            }
            catch
            {
                ok = false;
                updating = null;
            }
        }

        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void LOGIN_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
