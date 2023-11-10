using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send("github.com", 1000);
            MessageBox.Show(reply.Status.ToString());

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send("dssoft.ch", 1000);
            MessageBox.Show(reply.Status.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send("dssoft-byte.github.io", 1000);
            MessageBox.Show(reply.Status.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send("buyhamstersinbulk.000webhostapp.com", 5000);
            MessageBox.Show(reply.Status.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Server is under Maintenance");
            //Ping ping = new Ping();
            //PingReply reply = ping.Send("192.168.XX.XX", 1000);
            //MessageBox.Show(reply.Status.ToString());
        }

        private void Form8_Load(object sender, EventArgs e)
        {

        }
    }
}
