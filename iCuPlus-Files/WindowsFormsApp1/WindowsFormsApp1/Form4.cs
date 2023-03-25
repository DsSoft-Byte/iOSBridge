using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            // Add a name prompt at firstrun of iCu+, make user.lic file, and at the start of this window, read from it, if file dosent exist throw error and exit.
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            string sfileReader;
            sfileReader = System.IO.File.ReadAllText(@"C:/iCures/Username.txt");
            label10.Text = sfileReader;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/paypalme/hackintoshh");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.reddit.com/r/setupapp/");
        }
    }
}
