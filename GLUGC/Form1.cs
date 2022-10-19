using GLUGC.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GLUGC
{
    public partial class Form1 : Form
    {
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams handleparam = base.CreateParams;
        //        handleparam.ExStyle |= 0x02000000;
        //        return handleparam;
        //    }

        //}

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    this.DoubleBuffered = true;
        //}
        //public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        //{
        //    //Taxes: Remote Desktop Connection and painting
        //    //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
        //    if (System.Windows.Forms.SystemInformation.TerminalServerSession)
        //        return;

        //    System.Reflection.PropertyInfo aProp =
        //          typeof(System.Windows.Forms.Control).GetProperty(
        //                "DoubleBuffered",
        //                System.Reflection.BindingFlags.NonPublic |
        //                System.Reflection.BindingFlags.Instance);

        //    aProp.SetValue(c, true, null);
        //}
        //private const int WM_NCHITTEST = 0x84;
        //private const int WS_MINIMIZEBOX = 0x20000;
        //private const int HTCLIENT = 0x1;
        //private const int HTCAPTION = 0x2;
        //private const int CS_DBLCLKS = 0x8;
        //private const int CS_DROPSHADOW = 0x00020000;
        //private const int WM_NCPAINT = 0x0085;
        //private const int WM_ACTIVATEAPP = 0x001C;



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
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        //public string username;
        public string id;
        //public Image img;
        public Form1()
        {
            InitializeComponent();

            ApplyShadows(this);
            this.panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseWheel);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ResizeRedraw = true;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);

        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
        //        return cp;
        //    }
        //}

        int scrollvalue = 0;
        int stillscrollvalue = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (updating == null)
            {
                updating =
                  new Thread(new ThreadStart(loadprein));
                updating.Start();
            }
        }
        public string version;
        public string chatid = "";
        AutoCompleteStringCollection source = new AutoCompleteStringCollection();
        bool loadingpa = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            color();
            loadingpa = true;
            login();

            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                try
                {
                    GCGLU.Open();
                    String query = "SELECT * FROM updatestat where id = 1";
                    SqlCommand cmd = new SqlCommand(query, GCGLU);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        if (dr["version"].ToString() == version)
                        {
                            update = false;
                        }
                        else
                        {
                            update = true;
                        }

                    }
                    dr.Close();
                    GCGLU.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "  Please contact service provider!ASAP");
                    this.Close();
                }

            }

            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                d.Rows.Clear();
                string Query = "SELECT image,loginval,id FROM tblAccounts ORDER BY loginval ASC";
                GCGLU.Open();
                SqlCommand cmd = new SqlCommand(Query, GCGLU);
                SqlDataReader myReader = cmd.ExecuteReader();
                d.Load(myReader);
                GCGLU.Close();
            }

            if (dgv == null)
            {
                dgv =
              new System.Threading.Thread(new System.Threading.ThreadStart(loaddgv));
                dgv.Start();
            }
            //guna2PictureBox1.Image = imgsql;
        


            //guna2PictureBox1.Image = imgsql;
            Image imggg = resizeImage(imgsql, new Size(20, 20));

            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
            imgsqlcircle = ByteToImage(xByte);

            ChangeControlStyles(dataGridView2, ControlStyles.OptimizedDoubleBuffer, true);
            this.dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = this.dataGridView2.ColumnHeadersDefaultCellStyle.BackColor;

            loadingpa = false;
            timer1.Start();
            timer2.Start();
            timer4.Start();
            ResumeLayout();

            Process[] workers = Process.GetProcessesByName("GLUGCBack(Do not Close)");
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }

        }
        bool dark = false;
        public void color()
        {
            if (Properties.Settings.Default.darkmode == "true")
            {
                this.BackColor = Color.FromArgb(30, 30, 30);
                label5.ForeColor = Color.FromArgb(181, 180, 180);
                label7.ForeColor = Color.FromArgb(181, 180, 180);
                label1.ForeColor = Color.FromArgb(181, 180, 180); 
                label2.ForeColor = Color.FromArgb(165, 164, 165);
                dark = true;
                guna2Panel1.FillColor = Color.FromArgb(34, 35, 35);
                guna2TextBox1.FillColor = Color.FromArgb(34, 35, 35);
                guna2Panel3.FillColor = Color.FromArgb(34, 35, 35);
                guna2TextBox1.ForeColor = Color.FromArgb(181, 180, 180);

                panel1.BackColor = Color.FromArgb(34, 35, 35);
                guna2Panel2.BackColor = Color.FromArgb(34, 35, 35);
                guna2Panel4.BackColor = Color.FromArgb(54, 55, 55);
                guna2Panel4.BorderColor = Color.FromArgb(10, 11, 11);

                dataGridView2.BackgroundColor = Color.FromArgb(30, 30, 30);
                dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
                dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView2.RowsDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
                dataGridView2.RowsDefaultCellStyle.ForeColor = Color.White;
                dataGridView2.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 30, 30);
                dataGridView2.RowsDefaultCellStyle.SelectionForeColor = Color.White;

                pictureBox2.Image = Properties.Resources.icons8_settings_64px;
            }
            else
            {
                this.BackColor = Color.FromArgb(255, 254, 254); 


                dark = false;
                label5.ForeColor = Color.FromArgb(33, 32, 33); 
                label7.ForeColor = Color.FromArgb(33, 32, 33);
                label1.ForeColor = Color.FromArgb(33, 32, 33);
                label2.ForeColor = Color.FromArgb(75, 75, 74);


                guna2Panel4.BackColor = Color.FromArgb(242, 242, 243);
                guna2Panel4.BorderColor = Color.FromArgb(211, 211, 211);

                guna2TextBox1.FillColor = Color.FromArgb(241, 241, 240);

                guna2TextBox1.ForeColor = Color.FromArgb(33, 32, 33);

                guna2Panel3.FillColor = Color.FromArgb(241, 241, 240);
                guna2Panel1.FillColor = Color.FromArgb(241, 241, 240);

                guna2Panel2.BackColor = Color.FromArgb(241, 241, 240);

                panel1.BackColor = Color.FromArgb(241, 241, 240);



                dataGridView2.BackgroundColor = Color.White;
                dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
                dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dataGridView2.RowsDefaultCellStyle.BackColor = Color.White;
                dataGridView2.RowsDefaultCellStyle.ForeColor = Color.Black;
                dataGridView2.RowsDefaultCellStyle.SelectionBackColor = Color.White;
                dataGridView2.RowsDefaultCellStyle.SelectionForeColor = Color.Black;

                pictureBox2.Image = Properties.Resources.icons8_settings_24px_4;
            }
        }
        public Image imgsql;
        public Image imgsqlcircle;
        public string typesql;
        string displayname;
        private void login()
        {
            try
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    String query1 = "SELECT * FROM tblAccounts where Id like '" + id + "'";
                    SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                    SqlDataReader dr1 = cmd2.ExecuteReader();

                    if (dr1.Read())
                    {
                        if (dr1["image"] != DBNull.Value)
                        {
                            byte[] img = (byte[])(dr1["image"]);
                            MemoryStream mstream = new MemoryStream(img);
                            imgsql = System.Drawing.Image.FromStream(mstream);
                            //guna2PictureBox1.Image = imgsql;
                        }
                        typesql = dr1["type"].ToString();
                        displayname = dr1["displayname"].ToString();
                        label1.Text = dr1["displayname"].ToString();
                        pictureBox1.Image = imgsql;
                        Image imggg = resizeImage(imgsql, new Size(40, 40));
                        guna2Button2.Image = imggg;
                        guna2Button2.Text = dr1["displayname"].ToString();
                    }
                    dr1.Close();
                    GCGLU.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ChangeControlStyles(Control ctrl, ControlStyles flag, bool value)
        {
            MethodInfo method = ctrl.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
                method.Invoke(ctrl, new object[] { flag, value });
        }
        protected void LB_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;

            MessageBox.Show(lbl.Text.Trim());
        }
        protected void btn_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button btnReply = sender as Guna.UI2.WinForms.Guna2Button;
            //MessageBox.Show(btnReply.Text.Trim());
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                GCGLU.Open();
                String query1 = "SELECT * FROM tblChats where Id like '" + btnReply.Text.Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                SqlDataReader dr1 = cmd2.ExecuteReader();

                if (dr1.Read())
                {
                    chatid = "";
                    chatid = btnReply.Text.Trim();
                    string s = dr1["sender"].ToString();
                    string result = s.Remove(s.Length - 2);

                    label7.Visible = true;
                    pictureBox4.Visible = true;
                    label7.Text = "Replying to: @" + result + " Message: " + dr1["message"].ToString();
                    GCGLU.Close();
                }
            }
            this.ActiveControl = null;
        }
        Thread dgv;
        int lastid = 0;
        Label bbl_old = new Label();
        private void loaddgv()
        {
            panel1.BeginInvoke((Action)delegate ()
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    int idindt = 70;
                    GCGLU.Open();
                    string listb = "SELECT MAX(Id) FROM tblChats";
                    SqlCommand commandv = new SqlCommand(listb, GCGLU);
                    int maxId = Convert.ToInt32(commandv.ExecuteScalar());
                    GCGLU.Close();

                    int fidindt = maxId - idindt;


                    GCGLU.Open();
                    DataTable dt = new DataTable();
                    string list = "Select sender,message,date,thumbnailimg,Id,sendersid,replyid from tblChats WHERE Id BETWEEN '" + fidindt + "' AND '" + maxId + "'";
                    SqlCommand command = new SqlCommand(list, GCGLU);
                    SqlDataReader reader = command.ExecuteReader();
                    dt.Load(reader);
                    GCGLU.Close();

                    foreach (DataRow row in dt.Rows)
                    {
                        lastid = Convert.ToInt32(row["Id"].ToString());

                        Label lblreply = new Label();
                        bool bot = false;
                        if (row["replyid"].ToString() != "")
                        {
                            bot = true;
                            GCGLU.Open();
                            String query1 = "SELECT * FROM tblChats where Id like '" + row["replyid"].ToString() + "'";
                            SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                            SqlDataReader dr1 = cmd2.ExecuteReader();

                            if (dr1.Read())
                            {
                                lblreply.AutoSize = false;
                                //if (dark == true)
                                //{
                                //    lblreply.ForeColor = Color.White;
                                //}
                                //else
                                //{
                                //    lblreply.ForeColor = Color.Black;
                                //}
                                //bbl.AutoSizeHeightOnly = true;
                                lblreply.Font = new Font("Segoe UI Semibold", 12, FontStyle.Italic, GraphicsUnit.Pixel);
                                lblreply.Location = bbl_old.Location;
                                lblreply.Left = 30;
                                lblreply.ForeColor = Color.DimGray;
                                lblreply.Top = bbl_old.Bottom + 35;

                                if (row["sendersid"].ToString() == id/* && sendersid == dr1["sendersid"].ToString()*/)
                                {
                                    if (id == dr1["sendersid"].ToString())
                                    {
                                        lblreply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                        lblreply.Text = "You replied to yourself" + Environment.NewLine + dr1["message"].ToString();
                                        lblreply.Width = 360;
                                        lblreply.TextAlign = ContentAlignment.TopRight;
                                    }
                                    else
                                    {
                                        lblreply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                        lblreply.Text = "Replied to: @" + dr1["sender"].ToString() + Environment.NewLine + dr1["message"].ToString();
                                        lblreply.Width = 360;
                                        lblreply.TextAlign = ContentAlignment.TopRight;
                                    }
                                    //lblreply.Text = "You";
                                }
                                else
                                {
                                    string strgroupids = row["sender"].ToString().Trim().Remove(row["sender"].ToString().Trim().Length - 1);
                                    lblreply.Text = "@" + strgroupids + " replied to @" + dr1["sender"].ToString() + Environment.NewLine + dr1["message"].ToString().Trim();
                                    lblreply.Width = 380;
                                    lblreply.TextAlign = ContentAlignment.TopLeft;
                                }
                                Graphics b = CreateGraphics();
                                SizeF sizea = b.MeasureString(lblreply.Text, lblreply.Font, lblreply.Width);
                                lblreply.Height = int.Parse(Math.Round(sizea.Height + 2, 0).ToString());
                                panel1.Controls.Add(lblreply);

                                GCGLU.Close();
                            }
                        }
                        else
                        {
                            bot = false;
                        }



                        Label lblsender = new Label();
                        lblsender.AutoSize = false;
                        if (dark == true)
                        {
                            lblsender.ForeColor = Color.FromArgb(181, 180, 180);
                        }
                        else
                        {
                            lblsender.ForeColor = Color.FromArgb(33, 32, 33);
                        }
                        //bbl.AutoSizeHeightOnly = true;
                        lblsender.Font = new Font("Segoe UI Semibold", 12, GraphicsUnit.Pixel);
                        lblsender.Location = bbl_old.Location;
                        lblsender.Left = 30;
                        if (bot == true)
                        {
                            lblsender.Top = lblreply.Bottom;
                        }
                        else
                        {
                            lblsender.Top = bbl_old.Bottom + 35;
                        }
                        //bot = bot + 40;

                        if (row["sendersid"].ToString() == id)
                        {
                            lblsender.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                            lblsender.Text = "You";
                            lblsender.Width = 360;
                            lblsender.TextAlign = ContentAlignment.TopRight;
                        }
                        else
                        {
                            lblsender.Text = "@" + row["sender"].ToString();
                            lblsender.Width = 380;
                            lblsender.TextAlign = ContentAlignment.TopLeft;
                        }
                        lblsender.Height = 16;
                        panel1.Controls.Add(lblsender);


                        byte[] imgaa = (byte[])(row["thumbnailimg"]);
                        MemoryStream mstream = new MemoryStream(imgaa);

                        PictureBox pic = new PictureBox();
                        pic.Image = System.Drawing.Image.FromStream(mstream);
                        pic.Location = lblsender.Location;
                        if (row["sendersid"].ToString() == id)
                        {
                            pic.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                            pic.Left = 390;
                        }
                        else
                        {
                            pic.Left = 10;
                        }
                        pic.Size = new Size(18, 18);
                        pic.SizeMode = PictureBoxSizeMode.Zoom;

                        panel1.Controls.Add(pic);


                        Guna.UI2.WinForms.Guna2Button btnReply = new Guna.UI2.WinForms.Guna2Button();
                        btnReply.Image = Properties.Resources.reply3;
                        btnReply.ImageSize = new Size(20, 15);
                        btnReply.ImageAlign = HorizontalAlignment.Right;
                        btnReply.ImageOffset = new Point(-6, 0);
                        btnReply.Size = new Size(25, 23);
                        btnReply.TextOffset = new Point(0, -100);
                        btnReply.Text = row["Id"].ToString();
                        btnReply.BorderRadius = 4;
                        if (dark == true)
                        {
                            btnReply.FillColor = Color.FromArgb(64, 64, 64);
                        }
                        else
                        {
                            btnReply.FillColor = Color.White;
                        }

                        btnReply.Click += new EventHandler(btn_Click);


                        Label lbl = new Label();
                        lbl.AutoSize = false;
                        lbl.Text = Environment.NewLine + row["message"].ToString() + Environment.NewLine + "  ";
                        if (dark == true)
                        {
                            lbl.ForeColor = Color.FromArgb(181, 180, 180);
                        }
                        else
                        {
                            lbl.ForeColor = Color.FromArgb(33, 32, 33);
                        }
                        lbl.Font = new Font("Segoe UI Semibold", 12, GraphicsUnit.Pixel);
                        lbl.Location = bbl_old.Location;
                        lbl.Top = lblsender.Bottom - 10;
                        //lbl.BorderStyle = BorderStyle.FixedSingle;
                        if (row["sendersid"].ToString() == id)
                        {
                            lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                            lbl.Left = 50;
                            lbl.Width = 360;
                            lbl.TextAlign = ContentAlignment.TopRight;
                        }
                        else
                        {
                            lbl.Left = 30;
                            lbl.Width = 360;
                            lbl.TextAlign = ContentAlignment.TopLeft;
                        }
                        lbl.Click += new EventHandler(LB_Click);
                        Graphics g = CreateGraphics();
                        SizeF size = g.MeasureString(lbl.Text, lbl.Font, lbl.Width);
                        lbl.Height = int.Parse(Math.Round(size.Height + 2, 0).ToString());


                        btnReply.Cursor = Cursors.Hand;

                        lbl.Cursor = Cursors.Hand;
                        btnReply.Location = lblsender.Location;
                        btnReply.Location = new Point(lblsender.Location.X, lblsender.Location.Y + 17);
                        if (row["sendersid"].ToString() == id)
                        {
                            btnReply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                            btnReply.Left = 25;
                        }
                        else
                        {
                            btnReply.Left = 390;
                        }

                        panel1.Controls.Add(btnReply);
                        panel1.Controls.Add(lbl);
                        bbl_old = lbl;

                        bbl_old.Focus();
                        stillscrollvalue = panel1.VerticalScroll.Value - 400;
                        scrollvalue = panel1.VerticalScroll.Value;
                        guna2TextBox1.Focus();
                    }
                }
            });
            dgv = null;
        }
        int datanumbers = 0;
        public Image GetImage(string value)
        {
            byte[] bytes = Convert.FromBase64String(value);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }
        private bool runn;
        public string type;
        private void label1_Click(object sender, EventArgs e)
        {
          
        }
        //int rowscount = 0;
        //DataTable dt = new DataTable();
        public void loadprein()
        {

            if (loadingpa == false)
            {
                try
                {
                    using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                    {
                        L++;
                        SqlCommand cmd = new SqlCommand("update tblAccounts set loginval=@loginval where Id=@Id", GCGLU);
                        GCGLU.Open();
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@loginval", L.ToString());
                        cmd.ExecuteNonQuery();
                        GCGLU.Close();
                    }
                    panel1.BeginInvoke((Action)delegate ()
                    {
                        guna2ProgressIndicator1.Stop();
                        guna2ProgressIndicator1.Visible = false;
                        using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                            {
                                GCGLU.Open();
                                string listb = "SELECT MAX(Id) FROM tblChats";
                                SqlCommand commandv = new SqlCommand(listb, GCGLU);
                                int maxId = Convert.ToInt32(commandv.ExecuteScalar());
                                GCGLU.Close();

                                if (lastid <= maxId)
                                {
                                    DataTable dat = new DataTable();
                                    GCGLU.Open();
                                    string listA = "Select sender,message,date,thumbnailimg,Id,sendersid,replyid from tblChats WHERE Id BETWEEN '" + lastid + "' AND '" + maxId + "'";
                                    SqlCommand commandA = new SqlCommand(listA, GCGLU);
                                    SqlDataReader readerA = commandA.ExecuteReader();
                                    dat.Load(readerA);
                                    GCGLU.Close();
                                    int rows = 0;
                                    foreach (DataRow row in dat.Rows)
                                    {
                                        rows++;
                                        if (rows != 1)
                                        {
                                            lastid = Convert.ToInt32(row["Id"].ToString());

                                            Label lblreply = new Label();
                                            bool bot = false;
                                            if (row["replyid"].ToString() != "")
                                            {
                                                bot = true;
                                                GCGLU.Open();
                                                String query1 = "SELECT * FROM tblChats where Id like '" + row["replyid"].ToString() + "'";
                                                SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                                                SqlDataReader dr1 = cmd2.ExecuteReader();

                                                if (dr1.Read())
                                                {
                                                    lblreply.AutoSize = false;
                                                //if (dark == true)
                                                //{
                                                //    lblreply.ForeColor = Color.White;
                                                //}
                                                //else
                                                //{
                                                //    lblreply.ForeColor = Color.Black;
                                                //}
                                                //bbl.AutoSizeHeightOnly = true;
                                                    lblreply.Font = new Font("Segoe UI Semibold", 12, FontStyle.Italic, GraphicsUnit.Pixel);
                                                    lblreply.Location = bbl_old.Location;
                                                    lblreply.Left = 30;
                                                    lblreply.ForeColor = Color.DimGray;
                                                    lblreply.Top = bbl_old.Bottom + 35;

                                                    if (row["sendersid"].ToString() == id/* && sendersid == dr1["sendersid"].ToString()*/)
                                                    {
                                                        if (id == dr1["sendersid"].ToString())
                                                    {
                                                        lblreply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                                        lblreply.Text = "You replied to yourself" + Environment.NewLine + dr1["message"].ToString();
                                                            lblreply.Width = 360;
                                                            lblreply.TextAlign = ContentAlignment.TopRight;
                                                        }
                                                        else
                                                    {
                                                        lblreply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                                        lblreply.Text = "Replied to: @" + dr1["sender"].ToString() + Environment.NewLine + dr1["message"].ToString();
                                                            lblreply.Width = 360;
                                                            lblreply.TextAlign = ContentAlignment.TopRight;
                                                        }
                                                    //lblreply.Text = "You";
                                                    }
                                                    else
                                                    {
                                                        string strgroupids = row["sender"].ToString().Trim().Remove(row["sender"].ToString().Trim().Length - 1);
                                                        lblreply.Text = "@" + strgroupids + " replied to @" + dr1["sender"].ToString() + Environment.NewLine + dr1["message"].ToString().Trim();
                                                        lblreply.Width = 380;
                                                        lblreply.TextAlign = ContentAlignment.TopLeft;
                                                    }
                                                    Graphics b = CreateGraphics();
                                                    SizeF sizea = b.MeasureString(lblreply.Text, lblreply.Font, lblreply.Width);
                                                    lblreply.Height = int.Parse(Math.Round(sizea.Height + 2, 0).ToString());
                                                    panel1.Controls.Add(lblreply);
                                                    GCGLU.Close();
                                                }
                                            }
                                            else
                                            {
                                                bot = false;
                                            }



                                            Label lblsender = new Label();
                                            lblsender.AutoSize = false;
                                            if (dark == true)
                                            {
                                                lblsender.ForeColor = Color.FromArgb(181, 180, 180);
                                        }
                                            else
                                            {
                                                lblsender.ForeColor = Color.FromArgb(33, 32, 33);
                                        }
                                        //bbl.AutoSizeHeightOnly = true;
                                            lblsender.Font = new Font("Segoe UI Semibold", 12, GraphicsUnit.Pixel);
                                            lblsender.Location = bbl_old.Location;
                                            lblsender.Left = 30;
                                            if (bot == true)
                                            {
                                                lblsender.Top = lblreply.Bottom;
                                            }
                                            else
                                            {
                                                lblsender.Top = bbl_old.Bottom + 35;
                                            }
                                        //bot = bot + 40;

                                            if (row["sendersid"].ToString() == id)
                                            {
                                                lblsender.Text = "You";
                                                lblsender.Width = 360;
                                                lblsender.TextAlign = ContentAlignment.TopRight;
                                            }
                                            else
                                            {
                                                lblsender.Text = "@" + row["sender"].ToString();
                                                lblsender.Width = 380;
                                                lblsender.TextAlign = ContentAlignment.TopLeft;
                                        }
                                        lblsender.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                        lblsender.Height = 16;
                                            panel1.Controls.Add(lblsender);


                                            byte[] imgaa = (byte[])(row["thumbnailimg"]);
                                            MemoryStream mstream = new MemoryStream(imgaa);
                                            PictureBox pic = new PictureBox();
                                            pic.Image = System.Drawing.Image.FromStream(mstream);
                                            pic.Location = lblsender.Location;
                                            if (row["sendersid"].ToString() == id)
                                        {
                                            pic.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                            pic.Left = 390;
                                            }
                                            else
                                            {
                                                pic.Left = 10;
                                            }
                                            pic.Size = new Size(18, 18);
                                            pic.SizeMode = PictureBoxSizeMode.Zoom;
                                            panel1.Controls.Add(pic);


                                            Guna.UI2.WinForms.Guna2Button btnReply = new Guna.UI2.WinForms.Guna2Button();
                                            btnReply.Image = Properties.Resources.reply3;
                                            btnReply.ImageSize = new Size(20, 15);
                                            btnReply.ImageAlign = HorizontalAlignment.Right;
                                            btnReply.ImageOffset = new Point(-6, 0);
                                            btnReply.Size = new Size(25, 23);
                                            btnReply.TextOffset = new Point(0, -100);
                                            btnReply.Text = row["Id"].ToString();
                                            btnReply.BorderRadius = 4;
                                            if (dark == true)
                                            {
                                                btnReply.FillColor = Color.FromArgb(64, 64, 64);
                                            }
                                            else
                                            {
                                                btnReply.FillColor = Color.White;
                                            }

                                            btnReply.Click += new EventHandler(btn_Click);


                                            Label lbl = new Label();
                                            lbl.AutoSize = false;
                                            lbl.Text = Environment.NewLine + row["message"].ToString() + Environment.NewLine + "  ";
                                            if (dark == true)
                                            {
                                                lbl.ForeColor = Color.FromArgb(181, 180, 180);
                                        }
                                            else
                                            {
                                                lbl.ForeColor = Color.FromArgb(33, 32, 33);
                                        }
                                            lbl.Font = new Font("Segoe UI Semibold", 12, GraphicsUnit.Pixel);
                                            lbl.Location = bbl_old.Location;
                                            lbl.Top = lblsender.Bottom - 10;
                                        //lbl.BorderStyle = BorderStyle.FixedSingle;
                                            if (row["sendersid"].ToString() == id)
                                        {
                                            lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                            lbl.Left = 50;
                                                lbl.Width = 360;
                                                lbl.TextAlign = ContentAlignment.TopRight;
                                            }
                                            else
                                            {
                                                lbl.Left = 30;
                                                lbl.Width = 360;
                                                lbl.TextAlign = ContentAlignment.TopLeft;
                                            }
                                            lbl.Click += new EventHandler(LB_Click);
                                            Graphics g = CreateGraphics();
                                            SizeF size = g.MeasureString(lbl.Text, lbl.Font, lbl.Width);
                                            lbl.Height = int.Parse(Math.Round(size.Height + 2, 0).ToString());


                                            btnReply.Cursor = Cursors.Hand;

                                            lbl.Cursor = Cursors.Hand;
                                            btnReply.Location = lblsender.Location;
                                            btnReply.Location = new Point(lblsender.Location.X, lblsender.Location.Y + 17);
                                            if (row["sendersid"].ToString() == id)
                                        {
                                            btnReply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                                            btnReply.Left = 25;
                                            }
                                            else
                                            {
                                                btnReply.Left = 390;
                                            }

                                            panel1.Controls.Add(btnReply);
                                            panel1.Controls.Add(lbl);
                                            bbl_old = lbl;

                                            notifier();
                                        }

                                    }
                                }
                                else
                                {
                                    panel1.Controls.Clear();
                                    panel1.Refresh();
                                    loaddgv();
                                }



                            }

                        });
                }
                catch
                {
                    guna2ProgressIndicator1.Start();
                    guna2ProgressIndicator1.Visible = true;
                    updating = null;
                }
            }
            updating = null;
        }

        private void notifier()
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                GCGLU.Open();
                String query1 = "SELECT * FROM tblChats where Id like '" + lastid + "'";
                SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                SqlDataReader dr1 = cmd2.ExecuteReader();

                if (dr1.Read())
                {
                    if (id != dr1["sendersid"].ToString())
                    {
                        PopupNotifier popup = new PopupNotifier();
                        byte[] imgaa = (byte[])(dr1["thumbnailimg"]);
                        MemoryStream mstream = new MemoryStream(imgaa);
                        popup.Image = System.Drawing.Image.FromStream(mstream);

                        string strgroupids = dr1["sender"].ToString().Trim().Remove(dr1["sender"].ToString().Trim().Length - 1);
                        popup.TitleText = "New Message from: @" + strgroupids;
                        string fontset = Properties.Settings.Default.font;
                        popup.ContentFont = new Font("Segoe UI", Convert.ToInt32(fontset), GraphicsUnit.Pixel);
                        popup.ContentText = dr1["message"].ToString();
                        popup.Popup();

                        //bbl_old.Focus();
                        if (scrollvalue > stillscrollvalue)
                        {
                            bbl_old.Focus();
                            guna2TextBox1.Focus();
                            stillscrollvalue = panel1.VerticalScroll.Value - 400;
                        }
                    }
                    else
                    {

                        bbl_old.Focus();
                        guna2TextBox1.Focus();

                        stillscrollvalue = panel1.VerticalScroll.Value - 400;
                        //if (scrollvalue < panel1.VerticalScroll.Value)
                        //{
                        //    bbl_old.Focus();
                        //}
                    }
                }
                else
                {
                    bbl_old.Focus();
                    guna2TextBox1.Focus();

                    stillscrollvalue = panel1.VerticalScroll.Value - 400;
                    
                }
                GCGLU.Close();
            }
            //label2.Text = panel1.VerticalScroll.Minimum.ToString();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            //int a = dataGridView1.Rows.Add();
            //dataGridView1.Rows[a].Cells["senderimg"].Value = "Jed";
            //dataGridView1.Rows[a].Cells["message"].Value = guna2TextBox1.Text;
            //dataGridView1.ClearSelection();//If you want


            //In case if you want to scroll down as well.

            send();

        }
        string lb = "{";
        string rb = "}";
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
               
            }
            else if (e.KeyCode.Equals(Keys.Enter))
            {
                send();
                e.SuppressKeyPress = true;
                guna2TextBox1.Focus();
            }

        }
        private void send()
        {
            if (guna2TextBox1.Text.Trim() == "")
            {
                MessageBox.Show("Please enter a message..");
                return;
            }
            if (displayname == "")
            {
                MessageBox.Show("Please enter your username");
            }
            else
            {

                Image imggg = resizeImage(imgsql, new Size(22, 22));

                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                Image img = ByteToImage(xByte);

                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, img.RawFormat);
                    byte[] img11 = ms.ToArray();

                    GCGLU.Open();

                    string insStmt2 = "insert into tblChats ([sender],[message],[date],[thumbnailimg],[sendersid],[replyid]) " +
                        "values " +
                        "(@sender,@message,@date,@thumbnailimg,@sendersid,@replyid)";

                    SqlCommand insCmd2 = new SqlCommand(insStmt2, GCGLU);

                    insCmd2.Parameters.AddWithValue("@sender", displayname + ": ");
                    insCmd2.Parameters.AddWithValue("@message", guna2TextBox1.Text.Replace("[", lb).Replace("]", rb).Replace("​*", "[*​]").Replace("%", "[%]").Replace("'", "''"));

                    DateTime date = DateTime.Now;
                    string fdate = date.ToString("MM/dd/yyyy HH:mm");
                    insCmd2.Parameters.AddWithValue("@date", fdate);
                    insCmd2.Parameters.Add("@thumbnailimg", SqlDbType.Image).Value = img11;
                    insCmd2.Parameters.AddWithValue("@sendersid", id);
                    //MessageBox.Show(chatid);
                    if (chatid == "")
                    {
                        insCmd2.Parameters.AddWithValue("@replyid", DBNull.Value);
                    }
                    else
                    {
                        insCmd2.Parameters.AddWithValue("@replyid", chatid);
                    }

                    insCmd2.ExecuteNonQuery();

                    GCGLU.Close();
                }
                label7.Visible = false;
                pictureBox4.Visible = false;
                chatid = "";
                guna2TextBox1.Text = "";

            }
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }
        Thread updating;
        private void label1_TextChanged(object sender, EventArgs e)
        {


        }

        private void guna2TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        private void label3_Click(object sender, EventArgs e)
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                try
                {

                    //var path = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\GLUConstInc\Jr Stocks Inventory\AppUpdator";
                    ////MessageBox.Show(path + "\AppUpdator.exe");
                    //Process.Start(path + "\\AppUpdator.exe");


                    var fileName1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\systemVER.vsn";
                    //MessageBox.Show(CurrentDirectory);
                    //string fileName1 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\GLUGC\GLUGC\gcver.vsn";


                    version = "";
                    if (File.Exists(fileName1))
                    {
                        // Read entire text file content in one string    
                        string text = File.ReadAllText(fileName1);
                        //MessageBox.Show(text.ToString().Trim());
                        version = text.ToString().Trim();
                    }

                    GCGLU.Open();
                    String query = "SELECT * FROM updatestat where id = 1";
                    SqlCommand cmd = new SqlCommand(query, GCGLU);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {

                        if (dr["version"].ToString() == version)
                        {
                            MessageBox.Show("No updates available!!!");
                        }
                        else
                        {
                            timer1.Stop();
                            //updateupdator();
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
                                //MessageBox.Show("Updates are available!!!");
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

                    }
                    dr.Close();
                    GCGLU.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "  Please contact service provider!ASAP");
                    this.Close();
                }

            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text.Trim() != "")
            {
                guna2Button1.Visible = true;
            }
            else
            {
                guna2Button1.Visible = false;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //string fileName1 = Path.GetDirectoryName(Application.ExecutablePath) + @"\gsbck.vsn";
            //FileInfo fi = new FileInfo(fileName1);





            //if (fi.Exists)
            //{
            //    fi.Delete();
            //}

            //using (FileStream fs = fi.Create())
            //{
            //    Byte[] txt = new UTF8Encoding(true).GetBytes(id.ToString());
            //    fs.Write(txt, 0, txt.Length);
            //}

            //using (StreamReader sr = File.OpenText(fileName1))
            //{
            //    string s = "";
            //    while ((s = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(s);
            //    }
            //}

            var fileName1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\gsbck.vsn";

            version = "";
            if (File.Exists(fileName1))
            {
                string text = File.ReadAllText(fileName1);
                version = text.ToString().Trim();
                text = text.Replace(text, id);
                File.WriteAllText(fileName1, text);
            }

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
        }
        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            //Form1 form = (Form1)sender;
            //form.ShowInTaskbar = false;
            //form.Location = new Point(-10000, -10000);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Shown(object sender, EventArgs e)
        {

        }
        minizoo obj = (minizoo)Application.OpenForms["minizoo"];
        //Thread th;
        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Current = Cursors.Default;
            this.Hide();
            minizoo frm = new minizoo();
            frm.id = id;
            frm.ShowDialog();
            this.Dispose();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Settings a = new Settings();
            a.type = type;
            a.id = id;
            a.ShowDialog();
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            var dataGridView = (sender as DataGridView);
            //Check the condition as per the requirement casting the cell value to the appropriate type
            if (e.ColumnIndex == 2 && (string)dataGridView.Rows[e.RowIndex].Cells[0].Value == "2")
                dataGridView.Cursor = Cursors.Default;
            else
                dataGridView.Cursor = Cursors.Hand;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

       

        private void guna2Button4_Click(object sender, EventArgs e)
        {
           
        }
        Thread thread;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (thread == null)
            {
                thread =
              new System.Threading.Thread(new System.Threading.ThreadStart(active));
                thread.Start();
            }
            //active();
        }
        bool u;
        DataTable d = new DataTable();
        private void active()
        {
            try
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {


                    L++;
                    SqlCommand cmd = new SqlCommand("update tblAccounts set loginval=@loginval where Id=@Id", GCGLU);
                    GCGLU.Open();
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@loginval", L.ToString());
                    cmd.ExecuteNonQuery();
                    GCGLU.Close();

                    guna2ProgressIndicator1.Stop();
                    guna2ProgressIndicator1.Visible = false;

                    foreach (DataRow row in d.Rows)
                    {

                        GCGLU.Open();
                        String query = "SELECT loginval,Id,image FROM tblAccounts where Id = '" + row["Id"].ToString() + "'";
                        SqlCommand cmda = new SqlCommand(query, GCGLU);
                        SqlDataReader dr = cmda.ExecuteReader();

                        if (dr.Read())
                        {
                            if (dr["loginval"].ToString() == row["loginval"].ToString() && dr["Id"].ToString() == row["Id"].ToString())
                            {
                                foreach (DataGridViewColumn col in dataGridView2.Columns)
                                {
                                    if (col.Name == row["Id"].ToString())
                                    {
                                        Thread.Sleep(1000);
                                        //MessageBox.Show(row["id"].ToString());
                                        dataGridView2.BeginInvoke((Action)delegate ()
                                        {
                                            dataGridView2.Columns.Remove(row["Id"].ToString());
                                        });
                                        break;
                                    }
                                }


                            }
                            else
                            {

                                bool exist = false;
                                row["loginval"] = dr["loginval"].ToString();
                                //MessageBox.Show(row["loginval"].ToString());
                                foreach (DataGridViewColumn col in dataGridView2.Columns)
                                {
                                    if (col.Name == row["Id"].ToString())
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                                if (exist == false)
                                {
                                    //MessageBox.Show(dr["id"].ToString());
                                    byte[] img = (byte[])(dr["image"]);
                                    MemoryStream mstream = new MemoryStream(img);
                                    Image pic = System.Drawing.Image.FromStream(mstream);


                                    Image imggg = resizeImage(pic, new Size(40, 40));

                                    var imagehelper = new ImageHelper();

                                    ImageConverter _imageConverter = new ImageConverter();
                                    byte[] xByte = (byte[])_imageConverter.ConvertTo(imagehelper.CropImage(imggg), typeof(byte[]));

                                    Image pic1 = ByteToImage(xByte);


                                    DataGridViewImageColumn dgvimg = new DataGridViewImageColumn();
                                    dgvimg.Image = pic1;
                                    dgvimg.ImageLayout = DataGridViewImageCellLayout.Zoom;
                                    dgvimg.Width = 35;
                                    dgvimg.HeaderText = dr["id"].ToString();
                                    dgvimg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                    dgvimg.Name = dr["Id"].ToString();
                                    string drid = dr["Id"].ToString();


                                    Thread.Sleep(1000);
                                    //MessageBox.Show(drid);
                                    dataGridView2.BeginInvoke((Action)delegate ()
                                    {
                                        dataGridView2.Columns.Add(dgvimg);
                                        if (dataGridView2.Rows.Count == 0)
                                        {
                                            dataGridView2.Rows.Add();
                                        }
                                        dataGridView2.Rows[0].Cells[drid].Value = pic1;
                                    });
                                }

                            }
                        }
                        GCGLU.Close();

                    }
                    Thread.Sleep(1000);
                    dataGridView2.BeginInvoke((Action)delegate ()
                    {
                        dataGridView2.ClearSelection();
                    });

                }
                thread = null;

            }
            catch
            {
                guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                {
                    guna2ProgressIndicator1.Start();
                    guna2ProgressIndicator1.Visible = true;
                    thread = null;
                });
            }
        }
        int L = 0;

        private void label2_Click(object sender, EventArgs e)
        {
            accountinfo a = new accountinfo();
            a.id = id;
            a.ShowDialog();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            accountinfo a = new accountinfo();
            a.id = id;
            a.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Validated(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
          
        }

        private void guna2CustomCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }
        bool update = false;
        bool img = false;
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (update == true)
            {
                pictureBox3.Visible = true;
                //GLUGC.Properties.Resources.icons8_warning_shield_48px
                if (img == false)
                {
                    pictureBox3.Image = null;
                    img = true;
                }
                else if (img == true)
                {
                    pictureBox3.Image = Properties.Resources.icons8_warning_shield_48px;
                    img = false;
                }
            }
        }
        private void timer4_Tick(object sender, EventArgs e)
        {

        }

        private void timer4_Tick_1(object sender, EventArgs e)
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                try
                {
                    GCGLU.Open();
                    String query = "SELECT * FROM updatestat where id = 1";
                    SqlCommand cmd = new SqlCommand(query, GCGLU);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        if (dr["version"].ToString() == version)
                        {
                            update = false; 
                            pictureBox3.Visible = false;
                        }
                        else
                        {
                            update = true;
                        }
                    }
                    dr.Close();
                    GCGLU.Close();

                    guna2ProgressIndicator1.Stop();
                    guna2ProgressIndicator1.Visible = false;
                }
                catch (Exception ex)
                {
                    guna2ProgressIndicator1.Start();
                    guna2ProgressIndicator1.Visible = true;
                }
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            label7.Visible = false;
            pictureBox4.Visible = false;
            chatid = "";
        }
        Thread set;
        bool show = false;
        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            set =
                   new System.Threading.Thread(new System.Threading.ThreadStart(showset));
            set.Start();
        }
        private void showset()
        {
            if (show == false)
            {
                guna2Transition1.ShowSync(guna2Panel2);
                guna2Panel2.BeginInvoke((Action)delegate ()
                {
                    guna2Panel2.Visible = true;
                });
                show = true;
            }
            else
            {
                guna2Transition1.HideSync(guna2Panel2);
                guna2Panel2.BeginInvoke((Action)delegate ()
                {
                    guna2Panel2.Visible = false;
                });
                show = false;
            }
        }

        private void guna2Button3_Click_2(object sender, EventArgs e)
        {
            Properties.Settings.Default.remember = "false";
            Properties.Settings.Default.accid = "";
            Properties.Settings.Default.Save();
            if (thread != null)
            {
                thread = null;
            }
            if (updating != null)
            {
                updating = null;
            }
            if (dgv != null)
            {
                dgv = null;
            }
            timer4.Stop();
            timer1.Stop();
            LOGIN f = new LOGIN();
            this.Dispose();

            f.ShowDialog();

            //Process[] workerssa = Process.GetProcessesByName("GLUGCBack(Do not Close)");
            //foreach (Process worker in workerssa)
            //{
            //    worker.Kill();
            //    worker.WaitForExit();
            //    worker.Dispose();
            //}
        }

        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            foreach (Control c in panel1.Controls)
            {
                int size = (int)c.Font.Size;
                c.Font = new Font("Microsoft Sans Serif", ++size);

                Graphics b = CreateGraphics();
                SizeF sizea = b.MeasureString(c.Text, c.Font, c.Width);
                c.Height = int.Parse(Math.Round(sizea.Height + 2, 0).ToString());
            }
        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            scrollvalue = panel1.VerticalScroll.Value;
            panel1.Refresh();

        }
        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            scrollvalue = panel1.VerticalScroll.Value;
            panel1.Refresh();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            label3_Click(sender,e);
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {

        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
           
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
        

        }

        private void guna2Button4_Click_2(object sender, EventArgs e)
        {

            accountinfo a = new accountinfo();
            a.id = id;
            a.ShowDialog();
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dataGridView2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
         
        }

        private void guna2Button2_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        private void label5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void guna2Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x84)
        //    {  // Trap WM_NCHITTEST
        //        Point pos = new Point(m.LParam.ToInt32());
        //        pos = this.PointToClient(pos);
        //        if (pos.Y < cCaption)
        //        {
        //            m.Result = (IntPtr)2;  // HTCAPTION
        //            return;
        //        }
        //        if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
        //        {
        //            m.Result = (IntPtr)17; // HTBOTTOMRIGHT
        //            return;
        //        }
        //    }
        //    base.WndProc(ref m);
        //}
        //private const int cGrip = 16;      // Grip size
        //private const int cCaption = 32;   // Caption bar height;

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    //Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
        //    //if (Properties.Settings.Default.darkmode == "true")
        //    //{
        //    //    ControlPaint.DrawSizeGrip(e.Graphics, Color.FromArgb(30, 30, 30), rc);
        //    //}
        //    //else
        //    //{
        //    //    ControlPaint.DrawSizeGrip(e.Graphics, Color.FromArgb(255, 254, 254), rc);
        //    //}
        //    //rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
        //    //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        //    //this.DoubleBuffered = true;
        //}
        private const int cGrip = 16;
        private const int cCaption = 32;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;
                }

                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            var fileName1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\gsbck.vsn";

            DirectoryInfo dInfo = new DirectoryInfo(fileName1);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
            version = "";
            if (File.Exists(fileName1))
            {
                string text = File.ReadAllText(fileName1);
                version = text.ToString().Trim();
                text = text.Replace(text, id);
                File.WriteAllText(fileName1, text);
            }

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
            if (thread != null)
            {
                thread.Abort();
                //thread = null;

            }
            if (updating != null)
            {
                updating = null;
            }
            Application.Exit();
        }

        private void guna2Panel4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
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

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Resize(object sender, EventArgs e)
        {
        }
    }
}
