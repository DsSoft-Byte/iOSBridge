using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Xml;
using System.Web;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://dssoft.ch/ver.txt");
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            var sr = new System.IO.StreamReader(response.GetResponseStream());

            string newestversions = sr.ReadToEnd();
            string currentversion = Application.ProductVersion;
            //MessageBox.Show(newestversions);

            if (newestversions.Contains(currentversion))
            {
                MessageBox.Show("You are up to date!");
                //label7.Text = newestversions;
            }
            else
            {
                using (var client = new WebClient())
                    if (File.Exists(@"C:\iCuPlus.zip"))
                        MessageBox.Show("Update file detected ERROR 2");
                    else

                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile("https://raw.githubusercontent.com/DsSoft-Byte/iCu-X/main/iCures.zip", @"C:\iCuPlus.zip");
                        string zipPath = @"C:\iCuPlus.zip";
                        string extractPath = @"C:\UpdateData";
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);

                        Form2 form2 = new Form2();

                        form2.Show();
                        //label7.Text = newestversions;
                    }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (var client = new WebClient())
                MessageBox.Show("REMOVE The C:/UpdateData Directory NOW, before clicking OK, if you dont, this process WILL CRASH, if its not there then proceed.");
                if (File.Exists(@"C:\iOSBridgeFS.zip"))
                {
                    string zipPath = @"C:\iOSBridgeFS.zip";
                    string extractPath = @"C:\UpdateData";
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                // Will crash here occasionally.
                    System.Diagnostics.Process.Start(@"C:\UpdateData\iCures\icuplusupdater.exe");
                    MessageBox.Show("Exit all iOSBridge windows and update then.");
                }
                else
                    MessageBox.Show("No valid iOSBridge downgrade file provided.");
                {
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (var client = new WebClient())
                MessageBox.Show("REMOVE The C:/UpdateData Directory NOW, before clicking OK, if you dont, this process WILL CRASH, if its not there then proceed.");
            if (File.Exists(@"C:\iOSBridge.zip"))
                {
                    string zipPath = @"C:\iOSBridge.zip";
                    string extractPath = @"C:\UpdateData";
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                    System.Diagnostics.Process.Start(@"C:\UpdateData\iCures\icuplusupdater.exe");
                    MessageBox.Show("Exit all iOSBridge windows and update then.");
                }
                else
                    MessageBox.Show("No valid iOSBridge FS file provided.");
                {
             }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }


        private void button3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("There is no iCu Uninstaller yet. Just delete the iCures directory. User Manual Code 002");
            Form14 form14 = new Form14();
            form14.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var generalTxt = new FileInfo(@"C:\UpdateData\iCures\idact.bat");
            var specialTxt = new FileInfo(@"C:\iCuPlus.zip");

            if (generalTxt.Exists && specialTxt.Exists)
                //if (specialTxt.Exists)
                System.IO.File.Delete(@"C:\iCuPlus.zip");
            string root = @"C:\UpdateData";
            // If directory does not exist, don't even try   
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }

            else
                MessageBox.Show("Either The UpdateData Directory is missing or the iCuPlus zip file is, delete those files by hand. User Manual Code 001");
        }

        private void button56_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9();
            form9.Show();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}

