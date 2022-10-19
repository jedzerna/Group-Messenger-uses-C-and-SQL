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
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using static GLUGC.bubble;

namespace GLUGC
{
    public partial class Form2 : Form
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
        public Form2()
        {
            InitializeComponent();
        }

        bool loadingpa = false;
        private void Form2_Load(object sender, EventArgs e)
        {
            CreateEmotions();
            //loadingpa = true;
            //bbl_old.Top = 0 - bbl_old.Height;

            //panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            //panel1.PerformLayout();

            //if (loading == null)
            //{
            //    loading =
            //  new System.Threading.Thread(new System.Threading.ThreadStart(start));
            //    loading.Start();
            //}

            //loadingpa = false;


            //Image img1;
            //img1 = Image.FromFile(@"C:\Users\markj\Desktop\1.jpg");
            //dataGridView1.Rows.Add("Column1", img1);

            //Image img2;
            //img2 = Image.FromFile(@"C:\Users\markj\Desktop\2.jpg");
            //dataGridView1.Rows.Add("Column1", img2);

            //Image img3;
            //img3 = Image.FromFile(@"C:\Users\markj\Desktop\3.jpg");
            //dataGridView1.Rows.Add("Column1", img3);

            //Image img4;
            //img4 = Image.FromFile(@"C:\Users\markj\Desktop\4.png");
            //dataGridView1.Rows.Add("Column1", img4);

            //Image img5;
            //img5 = Image.FromFile(@"C:\Users\markj\Desktop\5.jpg");
            //dataGridView1.Rows.Add("Column1", img5);

            //Image img6;
            //img6 = Image.FromFile(@"C:\Users\markj\Desktop\6.jpg");
            //dataGridView1.Rows.Add("Column1", img6);

            //Image img7;
            //img7 = Image.FromFile(@"C:\Users\markj\Desktop\7.jpg");
            //dataGridView1.Rows.Add("Column1", img7);

            //Image img8;
            //img8 = Image.FromFile(@"C:\Users\markj\Desktop\8.jpg");
            //dataGridView1.Rows.Add("Column1", img8);

            //Image img9;
            //img9 = Image.FromFile(@"C:\Users\markj\Desktop\9.jpg");
            //dataGridView1.Rows.Add("Column1", img9);

            //Image img10;
            //img10 = Image.FromFile(@"C:\Users\markj\Desktop\10.jpg");
            //dataGridView1.Rows.Add("Column1", img10);

            //Image img11;
            //img11 = Image.FromFile(@"C:\Users\markj\Desktop\11.jpg");
            //dataGridView1.Rows.Add("Column1", img11);
        }
        Thread loading;
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnfd = new OpenFileDialog();
            opnfd.Filter = "Image Files (*.jpg;*.jpeg;.*.gif;*.png;)|*.jpg;*.jpeg;.*.gif*.png";
            if (opnfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(opnfd.FileName);
                string path = Path.GetFileNameWithoutExtension(opnfd.FileName);
                MemoryStream mmst = new MemoryStream();
                pictureBox1.Image.Save(mmst, pictureBox1.Image.RawFormat);
                byte[] img = mmst.ToArray();
                int a = dataGridView1.Rows.Add();
                dataGridView1.Rows[a].Cells["Column1"].Value = img;
                dataGridView1.Rows[a].Cells["Column2"].Value = path;

            }
        }

        Label bbl_old = new Label();
        //int location = 0;
        public void addInMessage(string message)
        {

        }
        protected void LB_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;

            MessageBox.Show(lbl.Text.Trim());
        }
        protected void btn_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button btnReply = sender as Guna.UI2.WinForms.Guna2Button;
            MessageBox.Show(btnReply.Text.Trim());
        }
        string sendersid = "1";

        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    var data = (Byte[])(row.Cells["Column1"].Value);
                    var stream = new MemoryStream(data);
                    //v.pictureBox4.Image = Image.FromStream(stream);

                    //ImageConverter _imageConverter = new ImageConverter();
                    //byte[] xByte = (byte[])_imageConverter.ConvertTo(row.Cells["Column1"].Value, typeof(byte[]));
                    //Image img = ByteToImage(xByte);

                    MemoryStream ms = new MemoryStream();
                    Image.FromStream(stream).Save(ms, Image.FromStream(stream).RawFormat);
                    byte[] img11 = ms.ToArray();

                    GCGLU.Open();

                    string insStmt2 = "insert into tblWallpaper ([image],[name]) " +
                        "values " +
                        "(@image,@name)";

                    SqlCommand insCmd2 = new SqlCommand(insStmt2, GCGLU);
                    insCmd2.Parameters.Add("@image", SqlDbType.Image).Value = img11;
                    insCmd2.Parameters.AddWithValue("@name", row.Cells["Column2"].Value.ToString());

                    insCmd2.ExecuteNonQuery();

                    GCGLU.Close();
                }

            }
            MessageBox.Show("Done");
        }
        int lastid = 0;
        private void start()
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
                                lblreply.ForeColor = Color.Black;
                                //bbl.AutoSizeHeightOnly = true;
                                lblreply.Font = new Font("Segoe UI Semibold", 12, FontStyle.Italic, GraphicsUnit.Pixel);
                                lblreply.Location = bbl_old.Location;
                                lblreply.Left = 30;
                                lblreply.ForeColor = Color.DimGray;
                                lblreply.Top = bbl_old.Bottom + 35;

                                if (row["sendersid"].ToString() == sendersid/* && sendersid == dr1["sendersid"].ToString()*/)
                                {
                                    if (sendersid == dr1["sendersid"].ToString())
                                    {
                                        lblreply.Text = "You replied to yourself" + Environment.NewLine + dr1["message"].ToString();
                                        lblreply.Width = 360;
                                        lblreply.TextAlign = ContentAlignment.TopRight;
                                    }
                                    else
                                    {
                                        lblreply.Text = "Replied from: @" + dr1["sender"].ToString() + Environment.NewLine + dr1["message"].ToString();
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
                        lblsender.ForeColor = Color.White;
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

                        if (row["sendersid"].ToString() == sendersid)
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
                        lblsender.Height = 16;
                        panel1.Controls.Add(lblsender);


                        byte[] imgaa = (byte[])(row["thumbnailimg"]);
                        MemoryStream mstream = new MemoryStream(imgaa);
                        PictureBox pic = new PictureBox();
                        pic.Image = System.Drawing.Image.FromStream(mstream);
                        pic.Location = lblsender.Location;
                        if (row["sendersid"].ToString() == sendersid)
                        {
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


                        btnReply.Click += new EventHandler(btn_Click);


                        Label lbl = new Label();
                        lbl.AutoSize = false;
                        lbl.Text = Environment.NewLine + row["message"].ToString();
                        lbl.ForeColor = Color.White;
                        lbl.Font = new Font("Segoe UI Semibold", 12, GraphicsUnit.Pixel);
                        lbl.Location = bbl_old.Location;
                        lbl.Top = lblsender.Bottom - 10;
                        //lbl.BorderStyle = BorderStyle.FixedSingle;
                        if (row["sendersid"].ToString() == sendersid)
                        {
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



                        btnReply.FillColor = Color.FromArgb(64, 64, 64);
                        btnReply.Location = lblsender.Location;
                        btnReply.Location = new Point(lblsender.Location.X, lblsender.Location.Y + 17);
                        if (row["sendersid"].ToString() == sendersid)
                        {
                            btnReply.Left = 30;
                        }
                        else
                        {
                            btnReply.Left = 388;
                        }

                        panel1.Controls.Add(btnReply);
                        panel1.Controls.Add(lbl);
                        bbl_old = lbl;

                        bbl_old.Focus();
                        guna2TextBox1.Focus();
                    }
                }
            });
            loading = null;
        }
        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            label2.Text = panel1.HorizontalScroll.Value.ToString();
            panel1.Refresh();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = null;
        }
        Thread updating;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (updating == null)
            //{
            //    updating =
            //      new Thread(new ThreadStart(loadprein));
            //    updating.Start();
            //}
        }
        public void loadprein()
        {
            if (loadingpa == false)
            {
                panel1.BeginInvoke((Action)delegate ()
                {
                    using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                    {
                        GCGLU.Open();
                        string listb = "SELECT MAX(Id) FROM tblChats";
                        SqlCommand commandv = new SqlCommand(listb, GCGLU);
                        int maxId = Convert.ToInt32(commandv.ExecuteScalar());
                        GCGLU.Close();

                        if (lastid != maxId)
                        {
                            DataTable dat = new DataTable();
                            GCGLU.Open();
                            string listA = "Select sender,message,date,thumbnailimg,Id,sendersid,replyid from tblChats WHERE Id BETWEEN '" + lastid + "' AND '" + maxId+ "'";
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
                                            lblreply.ForeColor = Color.Black;
                                            //bbl.AutoSizeHeightOnly = true;
                                            lblreply.Font = new Font("Segoe UI Semibold", 12, FontStyle.Italic, GraphicsUnit.Pixel);
                                            lblreply.Location = bbl_old.Location;
                                            lblreply.Left = 30;
                                            lblreply.ForeColor = Color.DimGray;
                                            lblreply.Top = bbl_old.Bottom + 35;

                                            if (row["sendersid"].ToString() == sendersid/* && sendersid == dr1["sendersid"].ToString()*/)
                                            {
                                                if (sendersid == dr1["sendersid"].ToString())
                                                {
                                                    lblreply.Text = "You replied to yourself" + Environment.NewLine + dr1["message"].ToString();
                                                    lblreply.Width = 360;
                                                    lblreply.TextAlign = ContentAlignment.TopRight;
                                                }
                                                else
                                                {
                                                    lblreply.Text = "Replied from: @" + dr1["sender"].ToString() + Environment.NewLine + dr1["message"].ToString();
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
                                    lblsender.ForeColor = Color.White;
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

                                    if (row["sendersid"].ToString() == sendersid)
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
                                    lblsender.Height = 16;
                                    panel1.Controls.Add(lblsender);


                                    byte[] imgaa = (byte[])(row["thumbnailimg"]);
                                    MemoryStream mstream = new MemoryStream(imgaa);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = System.Drawing.Image.FromStream(mstream);
                                    pic.Location = lblsender.Location;
                                    if (row["sendersid"].ToString() == sendersid)
                                    {
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


                                    btnReply.Click += new EventHandler(btn_Click);


                                    Label lbl = new Label();
                                    lbl.AutoSize = false;
                                    lbl.Text = Environment.NewLine + row["message"].ToString();
                                    lbl.ForeColor = Color.White;
                                    lbl.Font = new Font("Segoe UI Semibold", 12, GraphicsUnit.Pixel);
                                    lbl.Location = bbl_old.Location;
                                    lbl.Top = lblsender.Bottom - 10;
                                    //lbl.BorderStyle = BorderStyle.FixedSingle;
                                    if (row["sendersid"].ToString() == sendersid)
                                    {
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



                                    btnReply.FillColor = Color.FromArgb(64, 64, 64);
                                    btnReply.Location = lblsender.Location;
                                    btnReply.Location = new Point(lblsender.Location.X, lblsender.Location.Y + 17);
                                    if (row["sendersid"].ToString() == sendersid)
                                    {
                                        btnReply.Left = 30;
                                    }
                                    else
                                    {
                                        btnReply.Left = 388;
                                    }

                                    panel1.Controls.Add(btnReply);
                                    panel1.Controls.Add(lbl);
                                    bbl_old = lbl;
                                    notifier();
                                }
                                
                            }
                        }



                    }
                });
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
                    if (sendersid != dr1["sendersid"].ToString())
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
                    }
                }
                GCGLU.Close();
            }
        }
        Hashtable emotions;
        void CreateEmotions()
        {
            emotions = new Hashtable(6);
            emotions.Add(":-)",Properties.Resources.smile);
            emotions.Add(":)", Properties.Resources.smile);

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage((Image)pictureBox1.Image);
            richTextBox1.Paste();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            AddEmotions();
            guna2HtmlLabel1.Text = richTextBox1.Text;
        }
        void AddEmotions()
        {
            foreach (string emote in emotions.Keys)
            {
                while (richTextBox1.Text.Contains(emote))
                {
                    int ind = richTextBox1.Text.IndexOf(emote);
                    richTextBox1.Select(ind, emote.Length);
                    Clipboard.SetImage((Image)emotions[emote]);
                    richTextBox1.Paste();
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();

            //select image
            this.richTextBox1.Select(3, 1);
            if ((this.richTextBox1.SelectionType & RichTextBoxSelectionTypes.Object) == RichTextBoxSelectionTypes.Object)
            {
                richTextBox1.Copy();
            }
            IDataObject idata = Clipboard.GetDataObject();

            //get data presenst
            if (idata.GetDataPresent("System.Drawing.Bitmap"))
            {
                //get image object from data object
                object imgObject = idata.GetData("System.Drawing.Bitmap");
                if (imgObject != null)
                {
                    //cast image
                    Image img = imgObject as Image;

                    //set picture box image
                    if (img != null)
                    {
                        pictureBox2.Image = img;
                        label1.Text = pictureBox2.Image.ToString();
                    }
                }
            }
        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            //if (this.richTextBox1.SelectionType == RichTextBoxSelectionTypes.Object)
            //{
            //    pictureBox1.Image = new Bitmap(imagePath);
            //}
        }
    }
}
