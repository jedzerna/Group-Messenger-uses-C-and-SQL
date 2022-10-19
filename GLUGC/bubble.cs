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
    public partial class bubble : UserControl
    {
        public bubble()
        {
            InitializeComponent();
        }

        private void bubble_Load(object sender, EventArgs e)
        {

        }
        public bubble(string message, msgtype messagetype)
        {
            InitializeComponent();

            // Set the text in the bubble from the message in the parameter.
            lblmessage.Text = message;

            // Change Color based on Message type.
            if (messagetype.ToString() == "In")
            {
                // incoming User message
                this.BackColor = Color.Gray;
            }
            else
            {
                // Outgoing Bot Message
                this.BackColor = Color.FromArgb(0, 164, 147);
            }
            Setheight();
        }
        void Setheight()
        {
            // resize the bubble after its been called
            Graphics g = CreateGraphics();
            SizeF size = g.MeasureString(lblmessage.Text, lblmessage.Font, lblmessage.Width);

            // Set the height for the bubble
            lblmessage.Height = int.Parse(Math.Round(size.Height + 2, 0).ToString());
            guna2Panel1.Height = int.Parse(Math.Round(size.Height + 2, 0).ToString());
            this.Height = int.Parse(Math.Round(size.Height + 2, 0).ToString());
        }
        public enum msgtype
        {
            In,
            Out
        }
    }
}
