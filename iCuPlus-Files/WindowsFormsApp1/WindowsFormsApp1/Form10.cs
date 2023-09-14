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
    public partial class Form10 : Form
    {
        public Form10()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DM me (my profile will open automatically) on reddit and ATTACH THE NOW OBLIGATORY Self-test log and describe your issue in-depth and tell me what have you tried to solve it.");
            System.Diagnostics.Process.Start("https://www.reddit.com/user/The_Hackintosh/");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Any donation is worth alot, it dosent matter if in code, money or test devices. However the best form of support to me are the latter two. My PayPal will open automatically after this message" +
                "however donations in physical test devices are crucial for the development of A6+ untethered JB's. If you wish to donate a device, click GET HELP and shoot me a DM :)");
            System.Diagnostics.Process.Start("https://www.paypal.com/paypalme/hackintoshh");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form11 form11 = new Form11();
            form11.Show();
        }

        private void Form10_Load(object sender, EventArgs e)
        {

        }
    }
}
