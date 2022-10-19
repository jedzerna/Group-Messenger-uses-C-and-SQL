using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLUGC
{
    public partial class MiniZooSettings : Form
    {
        public MiniZooSettings()
        {
            InitializeComponent();
        }
        bool changes = false;
        private void MiniZooSettings_Load(object sender, EventArgs e)
        {
            changes = true;
            color();
            guna2ComboBox1.Text = Properties.Settings.Default.font;
         
            //if (Properties.Settings.Default.names != "")
            //{
            //    guna2CustomCheckBox2.Checked = true;
            //}
            //else
            //{
            //    guna2CustomCheckBox2.Checked = false;
            //}
            //if (Properties.Settings.Default.darkmode == "true")
            //{
            //    guna2CustomCheckBox2.Checked = true;
            //}
            //else
            //{
            //    guna2CustomCheckBox2.Checked = false;
            //}
            changes = false;
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changes == false)
            {
                MessageBox.Show("Not Available, Please wait for the next update. Thank you yawa ay...");
                return;
                //Cursor.Current = Cursors.WaitCursor;
                //Properties.Settings.Default.font = guna2ComboBox1.Text;
                //Properties.Settings.Default.Save();
                //minizoo.UpdateFont();
                //Cursor.Current = Cursors.Default;
            }
        }

        private void guna2CustomCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private minizoo minizoo = null;
        public MiniZooSettings(minizoo callingForm)
        {
            minizoo = callingForm as minizoo;
            InitializeComponent();
        }
        //private mini mini = null;
        //public MessageMenu(Form1 callingForm)
        //{
        //    form = callingForm as Form1;
        //    InitializeComponent();
        //}
        private void guna2CustomCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.WaitCursor;
            //if (changes == false)
            //{
            //    string ddd = "";
            //    if (guna2CustomCheckBox2.Checked)
            //    {
            //        Properties.Settings.Default.darkmode = "true";
            //        Properties.Settings.Default.Save();
            //        ddd = "true";
            //    }
            //    else
            //    {
            //        Properties.Settings.Default.darkmode = "";
            //        Properties.Settings.Default.Save();
            //        ddd = "false";
            //    }
            //    color();
            //    //minizoo.color();

            //    minizoo.colordw = ddd;
            //}
            //Cursor.Current = Cursors.Default;
        }
        private void color()
        {
            if (Properties.Settings.Default.darkmode == "true")
            {
                this.BackColor = Color.FromArgb(15, 14, 15);
                label1.ForeColor = Color.White;
                label2.ForeColor = Color.White;
                label4.ForeColor = Color.White;


                guna2ComboBox1.FillColor = Color.FromArgb(56, 56, 57);
                guna2ComboBox1.ForeColor = Color.White;
                guna2ComboBox1.ItemsAppearance.BackColor = Color.FromArgb(56, 56, 57);
                guna2ComboBox1.ItemsAppearance.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = Color.White;

                label1.ForeColor = Color.Black;
                label2.ForeColor = Color.Black;
                label4.ForeColor = Color.Black;


                guna2ComboBox1.FillColor = Color.FromArgb(51, 52, 144);
                guna2ComboBox1.ItemsAppearance.BackColor = Color.FromArgb(51, 52, 144);
                guna2ComboBox1.ItemsAppearance.ForeColor = Color.White;
                guna2ComboBox1.ForeColor = Color.White;
            }
        }
    }
}
