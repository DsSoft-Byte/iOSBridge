using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace WindowsFormsApp1
{
    public partial class Form15 : Form
    {
        WebClient webClient = new WebClient();
        string zipFile = @"C:\iOSBridge.zip"; // Path to save the downloaded zip file
        string extractPath = @"C:\UpdateData";
        public Form15()
        {
            InitializeComponent();
            webClient.DownloadProgressChanged += DownloadProgressChanged;
            webClient.DownloadFileCompleted += DownloadCompleted;
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            // Update progress bar or any visual indicator of download progress
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Form15_Load(object sender, EventArgs e)
        {
            DownloadAndInstallProgram();
        }
        private void DownloadAndInstallProgram()
        {
            try
            {
                webClient.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/DsSoft-Byte/iCu-X/main/iCures-Light.zip"), zipFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage; // Update progress bar with download progress
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //button1.Enabled = true; // Enable the button after download completes
            if (e.Error == null)
            {
                try
                {
                    // Extract the contents of the ZIP file directly to C:\
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath);
                    //MessageBox.Show("Download completed successfully!");
                    File.Delete(zipFile);
                    if (File.Exists(@"C:\UpdateData\iCures\icuplusupdater.exe"))
                    {
                        System.Diagnostics.Process.Start(@"C:\UpdateData\iCures\icuplusupdater.exe");
                        //MessageBox.Show("Exit all iOSBridge windows and update then.");
                        this.Close();
                        Environment.Exit(1);
                    }
                    else
                        MessageBox.Show("No valid iOSBridge FS file provided.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error installing: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Error downloading file: " + e.Error.Message);
            }
        }
    }
}
