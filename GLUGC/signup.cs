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
    public partial class signup : Form
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
        public signup()
        {
            InitializeComponent();
        }

        private void signup_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "" || guna2TextBox2.Text == "" || guna2TextBox3.Text == "")
            {
                MessageBox.Show("Please enter all the required detail");
            }
            else if (label6.Text == "Type your registered Employee ID here")
            {
                MessageBox.Show("Please type your registered Employee ID");
            }
            else if (label6.Text == "EMPCODE IS NOT AVAILABLE")
            {
                MessageBox.Show("Employee ID is not registered");
            }
            else
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    SqlCommand check_User_Name = new SqlCommand("SELECT COUNT(*) FROM [tblAccounts] WHERE ([empcode] = @empcode)", GCGLU);
                    check_User_Name.Parameters.AddWithValue("@empcode", guna2TextBox3.Text);
                    int UserExist = (int)check_User_Name.ExecuteScalar();

                    if (UserExist > 0)
                    {

                        MessageBox.Show("Employee ID is already exist in this program, please remember your password...");

                        GCGLU.Close();
                    }
                    else
                    {
                        GCGLU.Close();

                      
                            GCGLU.Open();
                            SqlCommand check_User_Name1 = new SqlCommand("SELECT COUNT(*) FROM [tblAccounts] WHERE ([username] = @username)", GCGLU);
                        check_User_Name1.Parameters.AddWithValue("@username", guna2TextBox2.Text);
                            int UserExist1 = (int)check_User_Name1.ExecuteScalar();

                            if (UserExist1 > 0)
                            {

                                MessageBox.Show("Username already exist");

                            }
                            else
                            {
                                GCGLU.Close();
                                signupadd();
                            }

                        
                    }

                }
            }
        }
        string lb = "{";
        string rb = "}";
        private void signupadd()
        {
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                var imagehelper = new ImageHelper();
                ImageConverter _imageConverter = new ImageConverter();
                byte[] img11 = (byte[])_imageConverter.ConvertTo(imagehelper.CropImage(pictureBox1.Image), typeof(byte[]));

                GCGLU.Open();

                string insStmt2 = "insert into tblAccounts ([username],[password],[empcode],[datecreated],[device],[image],[type]) " +
                    "values (@username,@password,@empcode,@datecreated,@device,@image,@type)";

                SqlCommand insCmd2 = new SqlCommand(insStmt2, GCGLU);

                insCmd2.Parameters.AddWithValue("@username", guna2TextBox2.Text.Replace("[", lb).Replace("]", rb).Replace("​*", "[*​]").Replace("%", "[%]").Replace("'", "''"));
                insCmd2.Parameters.AddWithValue("@password", guna2TextBox1.Text.Replace("[", lb).Replace("]", rb).Replace("​*", "[*​]").Replace("%", "[%]").Replace("'", "''"));
                insCmd2.Parameters.AddWithValue("@empcode", guna2TextBox3.Text.Replace("[", lb).Replace("]", rb).Replace("​*", "[*​]").Replace("%", "[%]").Replace("'", "''"));
         
                DateTime dat = DateTime.Now;
                string date = dat.ToString("MM/dd/yyyy");
                insCmd2.Parameters.AddWithValue("@datecreated", date);
                insCmd2.Parameters.AddWithValue("@device", System.Environment.MachineName.ToString().Replace("[", lb).Replace("]", rb).Replace("​*", "[*​]").Replace("%", "[%]").Replace("'", "''"));
                insCmd2.Parameters.Add("@image", SqlDbType.Image).Value = img11;
                if (guna2TextBox3.Text == "ZER404")
                {
                    insCmd2.Parameters.AddWithValue("@type", "Admin");
                }
                else
                {
                    insCmd2.Parameters.AddWithValue("@type", "User");
                }
                insCmd2.ExecuteNonQuery();

                GCGLU.Close();
                MessageBox.Show("Added");
                guna2TextBox1.Text = "";
                guna2TextBox2.Text = "";
                guna2TextBox3.Text = "";
                pictureBox1.Image = Properties.Resources.defaultcontacts;
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

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (guna2TextBox3.Text != "")
            {

                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    SqlCommand check_User_Name = new SqlCommand("SELECT COUNT(*) FROM [PERID] WHERE ([EMPCODE] = @EMPCODE)", GCGLU);
                    check_User_Name.Parameters.AddWithValue("@EMPCODE", guna2TextBox3.Text);
                    int UserExist = (int)check_User_Name.ExecuteScalar();

                    if (UserExist > 0)
                    {
                        label6.Text = "EMPCODE Exist";
                    }
                    else
                    {
                        label6.Text = "EMPCODE IS NOT AVAILABLE";
                    }

                    GCGLU.Close();
                }
            }
        }
    }
}
