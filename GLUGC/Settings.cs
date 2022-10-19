using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLUGC
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }
        public string type;
        public string id;

        bool changes = false;
        private void Settings_Load(object sender, EventArgs e)
        {
            changes = true;
            if (type != "Admin")
            {
                guna2Button2.Visible = false;
                guna2Button4.Visible = false;
            }
            login();
            if (Properties.Settings.Default.darkmode == "true")
            {
                guna2ToggleSwitch1.Checked = true;
            }
            else
            {
                guna2ToggleSwitch1.Checked = false;
            }
            changes = false;
        }
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
                            Image imgsql = System.Drawing.Image.FromStream(mstream);
                            guna2PictureBox1.Image = imgsql;
                        }
                        type = dr1["type"].ToString();

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

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("Are you sure to delete all messages??", "Update?", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.Yes)
            {
                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    string sqlTrunc = "TRUNCATE TABLE tblChats";
                    SqlCommand cmd = new SqlCommand(sqlTrunc, GCGLU);
                    GCGLU.Open();
                    cmd.ExecuteNonQuery();
                    GCGLU.Close();
                }

                Image imggg = resizeImage(guna2PictureBox1.Image, new Size(20, 20));

                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                Image fimage = ByteToImage(xByte);

                using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                {
                    MemoryStream ms = new MemoryStream();
                    fimage.Save(ms, fimage.RawFormat);
                    byte[] img11 = ms.ToArray();

                    GCGLU.Open();

                    string insStmt2 = "insert into tblChats ([sender],[message],[date],[thumbnailimg],[sendersid]) values (@sender,@message,@date,@thumbnailimg,@sendersid)";

                    SqlCommand insCmd2 = new SqlCommand(insStmt2, GCGLU);

                    insCmd2.Parameters.AddWithValue("@sender", label2.Text + ": ");
                    insCmd2.Parameters.AddWithValue("@message", "Deleted all convos");

                    DateTime date = DateTime.Now;
                    string fdate = date.ToString("MM/dd/yyyy HH:mm");
                    insCmd2.Parameters.AddWithValue("@date", fdate);
                    insCmd2.Parameters.Add("@thumbnailimg", SqlDbType.Image).Value = img11;
                    insCmd2.Parameters.AddWithValue("@sendersid", id);

                    insCmd2.ExecuteNonQuery();

                    GCGLU.Close();
                }
                MessageBox.Show("Messages truncated!!!");
            }
        }

        string path;
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    var path2 = fbd.SelectedPath.ToString() + "\\PERID.DBF";

                    if (File.Exists(path2))
                    {
                        path = fbd.SelectedPath.ToString();

                        using (SqlConnection GCGLU = new SqlConnection(ConfigurationManager.ConnectionStrings["GCGLU"].ConnectionString))
                        {

                            string sqlTrunc = "TRUNCATE TABLE PERID";
                            SqlCommand cmd = new SqlCommand(sqlTrunc, GCGLU);
                            GCGLU.Open();
                            cmd.ExecuteNonQuery();
                            GCGLU.Close();

                            OleDbConnection oConn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + path + "';Extended Properties=dBase IV;");
                            OleDbCommand command = new OleDbCommand("select * from PERID.DBF", oConn);
                            DataTable dt = new DataTable();
                            oConn.Open();
                            dt.Load(command.ExecuteReader());
                            oConn.Close();

                            DataTableReader reader = dt.CreateDataReader();
                            GCGLU.Open();  ///this is my connection to the sql server
                            SqlBulkCopy sqlcpy = new SqlBulkCopy(GCGLU);
                            sqlcpy.DestinationTableName = "PERID";  //copy the datatable to the sql table
                            sqlcpy.WriteToServer(dt);
                            GCGLU.Close();
                            reader.Close();

                        }
                    }
                    else
                    {
                        path = "";
                        MessageBox.Show("The PERID.DBF doesn't exist make sure to locate the file with the specified folder.");
                    }


                }
            }
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (changes == false)
            {
                string ddd = "";
                if (guna2ToggleSwitch1.Checked)
                {
                    Properties.Settings.Default.darkmode = "true";
                    Properties.Settings.Default.Save();
                    ddd = "true";
                }
                else
                {
                    Properties.Settings.Default.darkmode = "";
                    Properties.Settings.Default.Save();
                    ddd = "false";
                }
                //color();
                ////minizoo.color();

                //minizoo.colordw = ddd;
                MessageBox.Show("Change will take effect by reopening the app. Thank you.");
            }
            Cursor.Current = Cursors.Default;
        }
    }
}
