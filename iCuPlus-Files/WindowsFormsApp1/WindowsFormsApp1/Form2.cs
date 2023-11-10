using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (File.Exists(@"C:\UpdateData\iCures\icuplusupdater.exe"))
            {
                Process.Start(@"C:\UpdateData\iCures\icuplusupdater.exe");
                Application.Exit();
            }
            else
                MessageBox.Show("Update Data Corrupted. Updater Missing." +
                    "Please Check your UpdateData folder in C: for outdated update files potentially from iCu non-X.");

            }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var generalTxt = new FileInfo(@"C:\UpdateData\iCures\idact.bat");
            var specialTxt = new FileInfo(@"C:\iCuPlus.zip");

            if (generalTxt.Exists && specialTxt.Exists)
                if (specialTxt.Exists)
                System.IO.File.Delete(@"C:\iCuPlus.zip");
            string root = @"C:\UpdateData";
            // If directory does not exist, don't even try   
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
                this.Close();
            }

            else
                MessageBox.Show("Error. User Manual Code 001");
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    }
