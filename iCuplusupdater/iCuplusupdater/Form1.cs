using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace iCuplusupdater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CreateVBScript();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CopyAndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CreateVBScript()
        {
            try
            {
                string scriptContent = @"On Error Resume Next

Dim objShell
Set objShell = CreateObject(""WScript.Shell"")

' Delete the directory and its contents using command line
objShell.Run ""cmd /c rmdir /s /q C:\UpdateData"", 0, True

Set objShell = Nothing";

                // Specify the path for the VBScript file
                string scriptPath = Path.Combine(Path.GetTempPath(), "DeleteUpdaterDirectory.vbs");

                // Write the script content to the file
                File.WriteAllText(scriptPath, scriptContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating the VBScript: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RunVBScript()
        {
            try
            {
                string scriptPath = Path.Combine(Path.GetTempPath(), "DeleteUpdaterDirectory.vbs");

                if (File.Exists(scriptPath))
                {
                    Process.Start("cscript", $"\"{scriptPath}\"");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running the VBScript: {ex.Message}");
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        private void CopyAndUpdate()
        {
            try
            {
                string sourceDir = @"C:\UpdateData\iCures\";
                string destinationDir = @"C:\iCures\";

                CopyDirectory(sourceDir, destinationDir, true);

                MessageBox.Show("Update succeeded");

                RunVBScript();
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
