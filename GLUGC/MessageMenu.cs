using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLUGC
{
    public partial class MessageMenu : Form
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
        public string formtype;
        public string message;
        public string date;
        public string id;
        public string chatid;
        public MessageMenu()
        {
            InitializeComponent();
        }
        private Form1 form = null;
        private minizoo mini = null;
        public MessageMenu(Form1 callingForm)
        {
            form = callingForm as Form1;
            InitializeComponent();
        }
        public MessageMenu(minizoo miniF)
        {
            mini = miniF as minizoo;
            InitializeComponent();
        }

        //Form1 obj = (Form1)Application.OpenForms["Form1"];
        private void MessageMenu_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            viewmesssage a = new viewmesssage();
            a.message = message;
            a.date = date;
            a.id = id;
            a.chatid = chatid;
            a.ShowDialog();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (formtype == "minizoo")
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    String query1 = "SELECT * FROM tblChats where Id like '" + chatid + "'";
                    SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                    SqlDataReader dr1 = cmd2.ExecuteReader();

                    if (dr1.Read())
                    {
                        mini.chatid = "";
                        mini.chatid = chatid;
                        string s = dr1["sender"].ToString();
                        string result = s.Remove(s.Length - 2);

                        mini.label7.Visible = true;
                        mini.pictureBox4.Visible = true;
                        mini.label7.Text = "Replying to: @" + result + " Message: " + dr1["message"].ToString();
                        GCGLU.Close();
                        this.Close();
                    }
                }
            }
            else
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    GCGLU.Open();
                    String query1 = "SELECT * FROM tblChats where Id like '" + chatid + "'";
                    SqlCommand cmd2 = new SqlCommand(query1, GCGLU);
                    SqlDataReader dr1 = cmd2.ExecuteReader();

                    if (dr1.Read())
                    {
                        form.chatid = "";
                        form.chatid = chatid;
                        string s = dr1["sender"].ToString();
                        string result = s.Remove(s.Length - 2);

                        form.label7.Visible = true;
                        form.pictureBox4.Visible = true;
                        form.label7.Text = "Replying to: @" + result + " Message: " + dr1["message"].ToString();
                        GCGLU.Close();
                        this.Close();
                    }
                }
            }

           
        }
    }
}
