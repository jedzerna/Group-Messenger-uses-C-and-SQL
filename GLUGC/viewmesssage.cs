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
    public partial class viewmesssage : Form
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
        public string message;
        public string date;
        public string id;
        public string chatid;
        public Image img;

        public viewmesssage()
        {
            InitializeComponent();
        }

        private void viewmesssage_Load(object sender, EventArgs e)
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
                        pictureBox1.Image = System.Drawing.Image.FromStream(mstream);

                    }
                    label5.Text = dr1["username"].ToString();

                    label2.Text = dr1["displayname"].ToString();
                }
                else
                {
                    pictureBox1.Image = img;
                }
                GCGLU.Close();
            }
            using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
            {
                GCGLU.Open();
                String query1 = "SELECT * FROM tblChats where Id like '" + chatid + "'";
                SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                SqlDataReader dr1 = cmd2.ExecuteReader();

                if (dr1.Read())
                {
                    guna2TextBox1.Text = dr1["message"].ToString();

                    label4.Text = dr1["date"].ToString();
                }
                GCGLU.Close();
            }
        }
    }
}
