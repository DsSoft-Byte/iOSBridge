using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string path = @"C:\iCures\Username.txt";
            //using (FileStream fs = File.Create(path))

            string path = @"c:\iCures\Username.txt";

            try
            {
                // Create the file, or overwrite if the file exists.
                using (FileStream fs = File.Create(path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(textBox1.Text);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                    this.Close();
                }

                //using (StreamWriter writetext = new StreamWriter(@"C:/iCures/Username.txt"))
                //{
                   // writetext.WriteLine(textBox1.Text);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Form9_Load(object sender, EventArgs e)
        {

        }
    }
}
