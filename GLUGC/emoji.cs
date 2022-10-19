using System;
using System.Collections;
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
    public partial class emoji : Form
    {
        public emoji()
        {
            InitializeComponent();
        }

        private void emoji_Load(object sender, EventArgs e)
        {
            CreateEmotions();
        }
        public const string Tent = "⛺";
        public const string Fuelpump = "⛽";
        public const string Scissors = "✂";
        public const string White_Check_Mark = "✅";
        public const string Airplane = "✈";
        public const string Envelope = "✉";
        public const string Fist = "✊";
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            guna2HtmlLabel1.Text = Tent;
            richTextBox2.Text = Tent;
            String s = String.Empty;
            for (int i = 0; i < richTextBox2.Lines.Length; i++)
            {
                s = richTextBox2.Lines[i];
            }
            MessageBox.Show(s);
            richTextBox1.Text += Environment.NewLine+richTextBox2.Text;

            AddEmotions();
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
        Hashtable emotions;
        void CreateEmotions()
        {

            Image sad = resizeImage(Properties.Resources.sad, new Size(20, 20));
            Image smile = resizeImage(Properties.Resources.smile, new Size(20, 20));

            emotions = new Hashtable(6);
            emotions.Add(":(", sad);
            emotions.Add(":)", smile);

        }
        void AddEmotions()
        {
            foreach (string emote in emotions.Keys)
            {
                while (richTextBox2.Text.Contains(emote))
                {
                    int ind = richTextBox2.Text.IndexOf(emote);
                    richTextBox2.Select(ind, emote.Length);
                    Clipboard.SetImage((Image)emotions[emote]);
                    richTextBox2.Paste();
                }
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            AddEmotions();

        }
    }
}
