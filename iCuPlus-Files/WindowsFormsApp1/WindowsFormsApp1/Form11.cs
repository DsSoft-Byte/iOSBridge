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
    public partial class Form11 : Form
    {
        public Form11()
        {
            InitializeComponent();
        }

        private void Form11_Load(object sender, EventArgs e)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://github.com/DsSoft-Byte/iCu-X/blob/main/version.txt");
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            var sr = new System.IO.StreamReader(response.GetResponseStream());

            string newestversions = sr.ReadToEnd();
            string currentversion = Application.ProductVersion;
            //MessageBox.Show(newestversions);

            if (newestversions.Contains(currentversion))
            {
                textBox2.Text ="You are up to date, Help Available!";
            }
            else
            {
                textBox2.Text = "Outdated, Help will not be granted except its an update issue!";
                MessageBox.Show("Help is not available for you because you are running an outdated version, that is probably your issue! Only help with updating will be granted!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
