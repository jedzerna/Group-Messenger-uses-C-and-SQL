using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLUGC
{
    public partial class accountinfo : Form
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
        public string username;
        public accountinfo()
        {
            InitializeComponent();
        }

        private void accountinfo_Load(object sender, EventArgs e)
        {
            login();
        }
        private string password;
        public string id;
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
                        id = dr1["Id"].ToString();
                        if (dr1["image"] != DBNull.Value)
                        {
                            byte[] img = (byte[])(dr1["image"]);
                            MemoryStream mstream = new MemoryStream(img);
                            pictureBox1.Image= System.Drawing.Image.FromStream(mstream);

                        }
                        guna2TextBox1.Text = dr1["username"].ToString();
                        password = dr1["password"].ToString();
                        username = dr1["username"].ToString();
                        guna2TextBox4.Text = dr1["displayname"].ToString();

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

        private void guna2Button2_Click(object sender, EventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(open.FileName);

                var imagehelper = new ImageHelper();

                //pictureBox1.Image = imagehelper.CropImage(pictureBox1.Image);

                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(imagehelper.CropImage(pictureBox1.Image), typeof(byte[]));

                pictureBox1.Image = ByteToImage(xByte);
            }
        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox1.Checked == true)
            {
                //string a = textBox2.Text;
                guna2TextBox3.PasswordChar = '\0';
            }
            else
            {
                guna2TextBox3.PasswordChar = '●';
            }
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox2.Checked == true)
            {
                //string a = textBox2.Text;
                guna2TextBox2.PasswordChar = '\0';
            }
            else
            {
                guna2TextBox2.PasswordChar = '●';
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "" || guna2TextBox2.Text == "" || guna2TextBox3.Text == "")
            {
                MessageBox.Show("Please fill up all the text box..");
            }
            else
            {
                if (password != guna2TextBox3.Text)
                {
                    MessageBox.Show("Password is not match!");
                }
                else
                {
                    using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                    {
                        if (username == guna2TextBox1.Text)
                        {
                            update();
                        }
                        else
                        {
                            GCGLU.Open();
                            SqlCommand check_User_Name = new SqlCommand("SELECT COUNT(*) FROM [tblAccounts] WHERE ([username] = @username)", GCGLU);
                            check_User_Name.Parameters.AddWithValue("@username", guna2TextBox1.Text);
                            int UserExist = (int)check_User_Name.ExecuteScalar();

                            if (UserExist > 0)
                            {

                                MessageBox.Show("Username already exist...");

                                GCGLU.Close();
                            }
                            else
                            {
                                GCGLU.Close();
                                update();
                            }
                        }
                      
                    }
                }
            }
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }
        private void update()
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {

                Image imggg = resizeImage(pictureBox1.Image, new Size(20, 20));

                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                guna2PictureBox1.Image = ByteToImage(xByte);

                MemoryStream ms = new MemoryStream();
                guna2PictureBox1.Image.Save(ms, guna2PictureBox1.Image.RawFormat);
                byte[] img11 = ms.ToArray();

                SqlCommand cmd = new SqlCommand("update tblChats set sender=@sender,thumbnailimg=@thumbnailimg where sendersid=@sendersid", GCGLU);

                GCGLU.Open();
                cmd.Parameters.AddWithValue("@sendersid", id);
                cmd.Parameters.AddWithValue("@sender", guna2TextBox4.Text + ": ");
                cmd.Parameters.Add("@thumbnailimg", SqlDbType.Image).Value = img11;
                cmd.ExecuteNonQuery();
                GCGLU.Close();
            }
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                byte[] img11 = ms.ToArray();

                SqlCommand cmd = new SqlCommand("update tblAccounts set username=@username,password=@password,image=@image,displayname=@displayname where Id=@Id", GCGLU);


                GCGLU.Open();
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@username", guna2TextBox1.Text);
                cmd.Parameters.AddWithValue("@password", guna2TextBox2.Text);
                cmd.Parameters.AddWithValue("@displayname", guna2TextBox4.Text);
                cmd.Parameters.Add("@image", SqlDbType.Image).Value = img11;
                cmd.ExecuteNonQuery();
                GCGLU.Close();
                MessageBox.Show("Saved" + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Note: The changes will effect when you relogin. Thanks");
            }

        }
    }
}
