using Microsoft.VisualBasic;
using System;
using System.IO;

namespace iCuplusupdater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
                CopyDirectory(@"C:\UpdateData\iCures", @"C:\iCures", true);

                static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
                {
                    // Get information about the source directory
                    var dir = new DirectoryInfo(sourceDir);

                    // Check if the source directory exists
                    if (!dir.Exists)
                        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

                    // Cache directories before we start copying
                    DirectoryInfo[] dirs = dir.GetDirectories();

                    // Create the destination directory
                    Directory.CreateDirectory(destinationDir);

                    // Get the files in the source directory and copy to the destination directory
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        string targetFilePath = Path.Combine(destinationDir, file.Name);
                        file.CopyTo(targetFilePath, true);
                    }

                    // If recursive and copying subdirectories, recursively call this method
                    if (recursive)
                    {
                        foreach (DirectoryInfo subDir in dirs)
                        {
                            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                            CopyDirectory(subDir.FullName, newDestinationDir, true);

                        Thread.Sleep(4000);
                        MessageBox.Show("Something Happened, Ignore this message. Developer code -11.");
                        }
                    }
                }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    }
