using GLUGC.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace GLUGC
{
    public partial class minizoo : Form
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
        public minizoo()
        {
            InitializeComponent();
        }

        Thread updating;
        public Image img;
        private string version;
        public string type;
        public string id;
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
        Image imgsql;
        string username;
        string typesql;
        public string chatid = "";

        private void logsin()
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
                            guna2PictureBox1.Image = System.Drawing.Image.FromStream(mstream);

                        }
                        username = dr1["username"].ToString();
                        typesql = dr1["type"].ToString();

                        label2.Text = dr1["username"].ToString();
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

       
        int datanumbers = 0;
        bool loadingpa = false;
        private void minizoo_Load(object sender, EventArgs e)
        {
            label1.Text = Properties.Settings.Default.darkmode;
            color();
            loadingpa = true;
            SuspendLayout();

            logsin();


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
            loaddgv();

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
           
            Image imggg = resizeImage(guna2PictureBox1.Image, new Size(20, 20));

            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
            pictureBox1.Image = ByteToImage(xByte);
            //active(); 
            ChangeControlStyles(dataGridView2, ControlStyles.OptimizedDoubleBuffer, true);
            this.dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = this.dataGridView2.ColumnHeadersDefaultCellStyle.BackColor;

            ResumeLayout();
            loadingpa = false;
            timer1.Start();
            timer2.Start();
            timer4.Start();
        }

        public void color()
        {
            if (Properties.Settings.Default.darkmode == "true")
            {
                this.BackColor = Color.FromArgb(15, 14, 15);
                label6.ForeColor = Color.White;
                label5.ForeColor = Color.White;
                label7.ForeColor = Color.White;


                guna2Panel1.FillColor = Color.FromArgb(34, 35, 35);
                guna2TextBox1.FillColor = Color.FromArgb(34, 35, 35);
                guna2TextBox1.ForeColor = Color.White;

                dataGridView1.BackgroundColor = Color.FromArgb(34, 35, 35);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(34, 35, 35);
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.RowsDefaultCellStyle.BackColor = Color.FromArgb(34, 35, 35);
                dataGridView1.RowsDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(34, 35, 35);
                dataGridView1.RowsDefaultCellStyle.SelectionForeColor = Color.White;


                dataGridView2.BackgroundColor = Color.FromArgb(15, 14, 15);
                dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 14, 15);
                dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView2.RowsDefaultCellStyle.BackColor = Color.FromArgb(15, 14, 15);
                dataGridView2.RowsDefaultCellStyle.ForeColor = Color.White;
                dataGridView2.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 14, 15);
                dataGridView2.RowsDefaultCellStyle.SelectionForeColor = Color.White;

                pictureBox2.Image = Properties.Resources.icons8_settings_64px;
            }
            else
            {
                this.BackColor = Color.White;

                label6.ForeColor = Color.Black;
                label5.ForeColor = Color.Black;
                label7.ForeColor = Color.Black;


                guna2TextBox1.FillColor = Color.FromArgb(241, 241, 240);
                guna2TextBox1.ForeColor = Color.Black;

                guna2Panel1.FillColor = Color.FromArgb(241, 241, 240);

                dataGridView1.BackgroundColor = Color.FromArgb(241, 241, 240);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 241, 240);
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.RowsDefaultCellStyle.BackColor = Color.FromArgb(241, 241, 240);
                dataGridView1.RowsDefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 241, 240);
                dataGridView1.RowsDefaultCellStyle.SelectionForeColor = Color.Black;

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
        private void loaddgv()
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                int idindt = 70;
                GCGLU.Open();
                string listb = "SELECT MAX(Id) FROM tblChats";
                SqlCommand commandv = new SqlCommand(listb, GCGLU);
                int maxId = Convert.ToInt32(commandv.ExecuteScalar());
                GCGLU.Close();

                //,sortid
                GCGLU.Open();
                DataTable dt = new DataTable();
                string list = "Select sender,message,date,thumbnailimg,Id,sendersid,replyid from tblChats WHERE Id BETWEEN '" + idindt + "' AND '" + maxId + "'";
                SqlCommand command = new SqlCommand(list, GCGLU);
                SqlDataReader reader = command.ExecuteReader();
                dt.Load(reader);
                GCGLU.Close();
                datanumbers = dt.Rows.Count;
                //dataGridView1.DataSource = dt;
                //MessageBox.Show("");
                //dataGridView1.BeginInvoke((Action)delegate ()
                //{
                dataGridView1.Rows.Clear();
                int rooo = 0;
                dataGridView1.Columns["image"].DefaultCellStyle.NullValue = null;
                dataGridView1.Columns["image2"].DefaultCellStyle.NullValue = null;
                foreach (DataRow row in dt.Rows)
                {
                    rooo++;
                    if (rooo != 1)
                    {
                        int c = dataGridView1.Rows.Add();
                        dataGridView1.Rows[c].Cells["Column1"].Value = row["Id"].ToString();
                        dataGridView1.Rows[c].Cells["message"].Value = "";
                        dataGridView1.Rows[c].Cells["sendersid"].Value = "";
                    }

                    if (row["replyid"].ToString() != "")
                    {
                        GCGLU.Open();
                        String query1 = "SELECT * FROM tblChats where Id like '" + row["replyid"].ToString() + "'";
                        SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                        SqlDataReader dr1 = cmd2.ExecuteReader();

                        if (dr1.Read())
                        {
                            int z = dataGridView1.Rows.Add();
                            dataGridView1.Rows[z].Cells["sendersid"].Value = row["sendersid"].ToString();
                            dataGridView1.Rows[z].Cells["Column1"].Value = row["Id"].ToString();
                            dataGridView1.Rows[z].Cells["date"].Value = "";
                            string s = dr1["sender"].ToString();
                            string result = s.Remove(s.Length - 2);
                            dataGridView1.Rows[z].Cells["message"].Value = "Replied to: @" + result + Environment.NewLine + dr1["message"].ToString();

                            GCGLU.Close();
                        }
                    }

                    int a = dataGridView1.Rows.Add();
                    int b = dataGridView1.Rows.Add();
                    dataGridView1.Rows[a].Cells["Column1"].Value = row["Id"].ToString();
                    dataGridView1.Rows[b].Cells["Column1"].Value = row["Id"].ToString();


                    byte[] imgaa = (byte[])(row["thumbnailimg"]);
                    MemoryStream mstream = new MemoryStream(imgaa);

                    if (row["sendersid"].ToString() == id)
                    {
                        dataGridView1.Rows[a].Cells["message"].Value = "You";
                        dataGridView1.Rows[a].Cells["image2"].Value = System.Drawing.Image.FromStream(mstream);
                        dataGridView1.Rows[a].Cells["image"].Value = null;
                    }
                    else
                    {
                        dataGridView1.Rows[a].Cells["message"].Value = row["sender"].ToString();
                        dataGridView1.Rows[a].Cells["image"].Value = System.Drawing.Image.FromStream(mstream);
                        dataGridView1.Rows[a].Cells["image2"].Value = null;
                    }


                    DateTime date = DateTime.Parse(row["date"].ToString());
                    dataGridView1.Rows[a].Cells["date"].Value = date.ToString("MM/dd/yyyy");

                    dataGridView1.Rows[b].Cells["message"].Value = row["message"].ToString();
                    dataGridView1.Rows[b].Cells["date"].Value = date.ToString("hh:mm tt");

                    dataGridView1.Rows[a].Cells["sendersid"].Value = row["sendersid"].ToString();

                    dataGridView1.Rows[b].Cells["sendersid"].Value = row["sendersid"].ToString();
                }
                int nRowIndex = dataGridView1.Rows.Count - 1;
                int nColumnIndex = 1;
                if (dataGridView1.Rows.Count != 0)
                {
                    dataGridView1.Rows[nRowIndex].Selected = false;
                    dataGridView1.Rows[nRowIndex].Cells[nColumnIndex].Selected = false;
                    dataGridView1.FirstDisplayedScrollingRowIndex = nRowIndex;
                }
                //});
            }
        }
        public void loadprein()
        {
            int rowscount = datanumbers;
            if (loadingpa == false)
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
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {

                    GCGLU.Open();

                    string list = "SELECT COUNT(*) FROM tblChats";
                    SqlCommand command = new SqlCommand(list, GCGLU);
                    Int32 count = (Int32)command.ExecuteScalar();

                    //MessageBox.Show(count.ToString());
                    GCGLU.Close();

                    if (count >= rowscount)
                    {

                        int nRowIndex = dataGridView1.Rows.Count - 1;
                        try
                        {
                            //string idindt = dataGridView1.Rows[nRowIndex].Cells["Column1"].Value.ToString();
                            string idindt = "70";
                            GCGLU.Open();
                            string listb = "SELECT MAX(Id) FROM tblChats";
                            SqlCommand commandv = new SqlCommand(listb, GCGLU);
                            int maxId = Convert.ToInt32(commandv.ExecuteScalar());
                            GCGLU.Close();

                            DataTable dat = new DataTable();
                            GCGLU.Open();
                            //string list = "Select sender,message,date,thumbnailimg,Id from tblChats";
                            string listA = "Select sender,message,date,thumbnailimg,Id,sendersid,replyid from tblChats WHERE Id BETWEEN '" + idindt + "' AND '" + maxId + "'";
                            SqlCommand commandA = new SqlCommand(listA, GCGLU);
                            SqlDataReader readerA = commandA.ExecuteReader();
                            dat.Load(readerA);
                            GCGLU.Close();
                            int check = 0;
                            foreach (DataRow rows in dat.Rows)
                            {
                                check++;
                                if (check != 1)
                                {
                                    bool checkrows = false;
                                    foreach (DataGridViewRow row in dataGridView1.Rows)
                                    {
                                        if (row.Cells["Column1"].Value.ToString() != "" && rows["Id"].ToString() == row.Cells["Column1"].Value.ToString())
                                        {
                                            checkrows = true;
                                            break;
                                        }
                                    }
                                    if (checkrows == false)
                                    {
                                        dataGridView1.BeginInvoke((Action)delegate ()
                                        {
                                            int c = dataGridView1.Rows.Add();
                                            dataGridView1.Rows[c].Cells["Column1"].Value = rows["Id"].ToString();
                                            dataGridView1.Rows[c].Cells["message"].Value = "";

                                            if (rows["replyid"].ToString() != "")
                                            {
                                                using (SqlConnection GCGLUa = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                                                {
                                                    GCGLUa.Open();
                                                    String query1 = "SELECT * FROM tblChats where Id like '" + rows["replyid"].ToString() + "'";
                                                    SqlCommand cmd2 = new SqlCommand(query1, GCGLUa);
                                                    SqlDataReader dr1 = cmd2.ExecuteReader();

                                                    if (dr1.Read())
                                                    {
                                                        int z = dataGridView1.Rows.Add();
                                                        dataGridView1.Rows[z].Cells["sendersid"].Value = rows["sendersid"].ToString();
                                                        dataGridView1.Rows[z].Cells["Column1"].Value = rows["Id"].ToString();
                                                        dataGridView1.Rows[z].Cells["date"].Value = "";
                                                        string s = dr1["sender"].ToString();
                                                        string result = s.Remove(s.Length - 2);
                                                        dataGridView1.Rows[z].Cells["message"].Value = "Replied to: @" + result + Environment.NewLine + dr1["message"].ToString();

                                                    }
                                                    GCGLUa.Close();
                                                }
                                            }



                                            int a = dataGridView1.Rows.Add();
                                            int b = dataGridView1.Rows.Add();
                                            dataGridView1.Rows[a].Cells["Column1"].Value = rows["Id"].ToString();
                                            dataGridView1.Rows[b].Cells["Column1"].Value = rows["Id"].ToString();
                                            byte[] imgaa = (byte[])(rows["thumbnailimg"]);
                                            MemoryStream mstream = new MemoryStream(imgaa);
                                            DateTime date = DateTime.Parse(rows["date"].ToString());


                                            if (rows["sendersid"].ToString() == id)
                                            {
                                                dataGridView1.Rows[a].Cells["message"].Value = "You";
                                                dataGridView1.Rows[a].Cells["image2"].Value = System.Drawing.Image.FromStream(mstream);
                                                dataGridView1.Rows[a].Cells["image"].Value = null;
                                            }
                                            else
                                            {
                                                dataGridView1.Rows[a].Cells["message"].Value = rows["sender"].ToString();
                                                dataGridView1.Rows[a].Cells["image"].Value = System.Drawing.Image.FromStream(mstream);
                                                dataGridView1.Rows[a].Cells["image2"].Value = null;
                                            }



                                            dataGridView1.Rows[a].Cells["date"].Value = date.ToString("MM/dd/yyyy");
                                            dataGridView1.Rows[b].Cells["message"].Value = rows["message"].ToString();
                                            dataGridView1.Rows[b].Cells["date"].Value = date.ToString("hh:mm tt");
                                            dataGridView1.Rows[a].Cells["sendersid"].Value = rows["sendersid"].ToString();
                                            dataGridView1.Rows[b].Cells["sendersid"].Value = rows["sendersid"].ToString();

                                            if (dataGridView1.FirstDisplayedScrollingRowIndex >= dataGridView1.Rows.Count - 30)
                                            {
                                                if (this.WindowState == FormWindowState.Minimized)
                                                {
                                                    notifier();
                                                }
                                                nRowIndex = dataGridView1.Rows.Count - 1;
                                                int nColumnIndex = 1;
                                                if (dataGridView1.Rows.Count != 0)
                                                {
                                                    dataGridView1.Rows[nRowIndex].Selected = false;
                                                    dataGridView1.Rows[nRowIndex].Cells[nColumnIndex].Selected = false;
                                                    dataGridView1.FirstDisplayedScrollingRowIndex = nRowIndex;
                                                }


                                            }
                                            else
                                            {
                                                notifier();
                                            }
                                        });
                                    }

                                }
                            }
                        }
                        catch
                        {
                            updating = null;
                        }




                    }
                    else
                    {
                        loaddgv();
                    }
                }
            }
            updating = null;

        }
        private void notifier()
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                GCGLU.Open();
                String query1 = "SELECT * FROM tblChats where Id like '" + dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Column1"].Value + "'";
                SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                SqlDataReader dr1 = cmd2.ExecuteReader();

                if (dr1.Read())
                {
                    PopupNotifier popup = new PopupNotifier();
                    byte[] imgaa = (byte[])(dr1["thumbnailimg"]);
                    MemoryStream mstream = new MemoryStream(imgaa);
                    popup.Image = System.Drawing.Image.FromStream(mstream);
                    popup.TitleText = "New Message: " + dr1["message"].ToString();
                    string fontset = Properties.Settings.Default.font;
                    popup.ContentFont = new Font("Segoe UI", Convert.ToInt32(fontset), GraphicsUnit.Pixel);
                    popup.ContentText = dr1["message"].ToString();
                    popup.Popup();
                }
                GCGLU.Close();
            }
        }
        DataTable d = new DataTable();
        private void active()
        {

            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
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



                                var imagehelper = new ImageHelper();

                                ImageConverter _imageConverter = new ImageConverter();
                                byte[] xByte = (byte[])_imageConverter.ConvertTo(imagehelper.CropImage(pic), typeof(byte[]));

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
                dataGridView2.BeginInvoke((Action)delegate ()
                {
                    dataGridView2.ClearSelection();
                });

            }
            thread = null;
        }
        int L = 0;
        private void ChangeControlStyles(Control ctrl, ControlStyles flag, bool value)
        {
            MethodInfo method = ctrl.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
                method.Invoke(ctrl, new object[] { flag, value });
        }

        //public void viewdate()
        //{
        //    if (Properties.Settings.Default.date != "")
        //    {
        //        dataGridView1.Columns["date"].Visible = true;
        //    }
        //    else
        //    {
        //        dataGridView1.Columns["date"].Visible = false;
        //    }
        //}
        //public void viewname()
        //{
        //    if (Properties.Settings.Default.names != "")
        //    {
        //        dataGridView1.Columns["senderimg"].Visible = true;
        //    }
        //    else
        //    {
        //        dataGridView1.Columns["senderimg"].Visible = false;
        //    }
        //}
        public void UpdateFont()
        {
            fontset = Properties.Settings.Default.font;
            //Change cell font
            //foreach (DataGridViewColumn c in dataGridView1.Columns)
            //{
            //    c.DefaultCellStyle.Font = new Font("Segoe UI", Convert.ToInt32(fontset), GraphicsUnit.Pixel);
            //}
            //foreach (DataGridViewRow r in dataGridView1.Rows)
            //{
            //    r.DefaultCellStyle.Font = new Font("Segoe UI", Convert.ToInt32(fontset), GraphicsUnit.Pixel);
            //}
            guna2TextBox1.Font = new Font(guna2TextBox1.Font.FontFamily, Convert.ToInt32(fontset));
            this.dataGridView1.DefaultCellStyle.Font = new Font("Tahoma", Convert.ToInt32(fontset));

        }
        private string fontset;
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }



        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void guna2TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
           
        }
        private void minizoo_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updating == null)
            {
                updating =
                  new Thread(new ThreadStart(loadprein));
                updating.Start();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            send();
        }
        private void send()
        {
            if (guna2TextBox1.Text.Trim() == "")
            {
                MessageBox.Show("Please enter a message..");
                return;
            }
            if (label2.Text == "")
            {
                MessageBox.Show("Please enter your username");
            }
            else
            {

                Image imggg = resizeImage(guna2PictureBox1.Image, new Size(20, 20));

                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                pictureBox1.Image = ByteToImage(xByte);

                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    MemoryStream ms = new MemoryStream();
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    byte[] img11 = ms.ToArray();

                    GCGLU.Open();

                    string insStmt2 = "insert into tblChats ([sender],[message],[date],[thumbnailimg],[sendersid],[replyid]) values (@sender,@message,@date,@thumbnailimg,@sendersid,@replyid)";

                    SqlCommand insCmd2 = new SqlCommand(insStmt2, GCGLU);

                    insCmd2.Parameters.AddWithValue("@sender", label2.Text.Trim() + ": ");
                    insCmd2.Parameters.AddWithValue("@message", guna2TextBox1.Text.Trim());

                    DateTime date = DateTime.Now;
                    string fdate = date.ToString("MM/dd/yyyy HH:mm");
                    insCmd2.Parameters.AddWithValue("@date", fdate);
                    insCmd2.Parameters.Add("@thumbnailimg", SqlDbType.Image).Value = img11;
                    insCmd2.Parameters.AddWithValue("@sendersid", id);
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

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Not Available, Please wait for the next update. Thank you yawa ay...");
            //return;
            MiniZooSettings m = new MiniZooSettings(this);
            m.ShowDialog();
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
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public string colordw
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }
        private void label1_TextChanged(object sender, EventArgs e)
        {
            color();
        }
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow.Cells["date"].Value.ToString() != "")
            {
                if (dataGridView1.CurrentRow.Cells["message"].Value.ToString() != "")
                {
                    MessageMenu a = new MessageMenu(this);
                    a.message = dataGridView1.CurrentRow.Cells["message"].Value.ToString();
                    a.date = dataGridView1.CurrentRow.Cells["date"].Value.ToString();
                    a.id = dataGridView1.CurrentRow.Cells["sendersid"].Value.ToString();
                    a.chatid = dataGridView1.CurrentRow.Cells["Column1"].Value.ToString();
                    a.formtype = "minizoo";
                    a.ShowDialog();
                }
            }
        }
        private void minizoo_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer4.Stop();
            if (thread != null)
            {
                thread.Abort();

            }
            if (updating != null)
            {
                updating.Abort();
            }

            Cursor.Current = Cursors.Default;
            this.Hide();
            Form1 frm = new Form1();
            frm.id = id;
            frm.type = typesql;
            frm.imgsql = imgsql;
            frm.ShowDialog();
            //Close the form.(frm_form1)
            this.Dispose();




         
        }

        private void guna2CustomCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox1.Checked)
            {
                dataGridView1.Columns["date"].Visible = true;
            }
            else
            {
                dataGridView1.Columns["date"].Visible = false;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    if (row.Cells["sendersid"].Value == null || row.Cells["sendersid"].Value == DBNull.Value || String.IsNullOrWhiteSpace(row.Cells["sendersid"].Value.ToString()))
            //    {
            //        // here is your message box...
            //    }
            //    else
            //    {
            //        if (row.Cells["sendersid"].Value.ToString() == id)
            //        {
            //            row.Cells["message"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            //        }
            //    }
            //}
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["sendersid"].Value == null || row.Cells["sendersid"].Value == DBNull.Value || String.IsNullOrWhiteSpace(row.Cells["sendersid"].Value.ToString()))
                {
                    // here is your message box...
                }
                else
                {
                    row.Height = 5;
                    row.MinimumHeight = 3;

                    if (row.Cells["message"].Value == null || row.Cells["message"].Value == DBNull.Value || String.IsNullOrWhiteSpace(row.Cells["message"].Value.ToString()))
                    {
                    }
                    else
                    {
                        if (row.Cells["sendersid"].Value.ToString() == id)
                        {
                            row.Cells["message"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        if (row.Cells["message"].Value.ToString().Contains("Replied to: "))
                        {
                            row.Cells["message"].Style.Font = new Font(e.CellStyle.Font, FontStyle.Italic);
                            row.Cells["message"].Style.ForeColor = Color.DimGray;
                        }
                    }
                }
            }
            //fontset = Properties.Settings.Default.font;
            ////Change cell font
            ////foreach (DataGridViewColumn c in dataGridView1.Columns)
            ////{
            ////    c.DefaultCellStyle.Font = new Font("Segoe UI", Convert.ToInt32(fontset), GraphicsUnit.Pixel);
            ////}
            //foreach (DataGridViewRow r in dataGridView1.Rows)
            //{
            //    r.DefaultCellStyle.Font = new Font("Segoe UI", Convert.ToInt32(fontset), GraphicsUnit.Pixel);
            //}
            //guna2TextBox1.Font = new Font(guna2TextBox1.Font.FontFamily, Convert.ToInt32(fontset));

        }
        //private string fontset;
        private void timer4_Tick(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            label7.Visible = false;
            pictureBox4.Visible = false;
            chatid = "";
        }
    }
}
