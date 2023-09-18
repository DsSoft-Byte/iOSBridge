using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form13 : Form
    {
        private ManagementEventWatcher connectWatcher;
        private ManagementEventWatcher disconnectWatcher;
        private System.Windows.Forms.Timer notificationTimer;



        public Form13()
        {
            InitializeComponent();
            connectWatcher = new ManagementEventWatcher();
            connectWatcher.EventArrived += new EventArrivedEventHandler(HandleConnectEvent);
            connectWatcher.Query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
            connectWatcher.Start();

            disconnectWatcher = new ManagementEventWatcher();
            disconnectWatcher.EventArrived += new EventArrivedEventHandler(HandleDisconnectEvent);
            disconnectWatcher.Query = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
            disconnectWatcher.Start();

        }

        private void HandleConnectEvent(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string deviceName = instance["Name"].ToString(); //OG Code
            string description = instance["Description"].ToString();

            if (description.Contains("Apple Mobile Device"))
            {
                //MessageBox.Show($"An Apple device connected: {deviceName}");
                Thread.Sleep(10);
                System.Diagnostics.Process.Start(@"C:\\iCures\\ideviceinfopipe.bat");
                Thread.Sleep(10);
                string filePath = @"C:\iCures\Dependencies\lim\output.txt";
                Thread.Sleep(4500);
                string fileContent = File.ReadAllText(filePath);

                this.Invoke((MethodInvoker)delegate
                {
                    label2.Text = (fileContent);
                    panel10.Visible = true;

                });
                ShowNotificationPanel();
            }
        }

        private void HandleDisconnectEvent(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string description = instance["Description"].ToString();

            if (description.Contains("Apple Mobile Device"))
            {
                //MessageBox.Show($"Apple device disconnected. {description}");
                this.Invoke((MethodInvoker)delegate
                {
                    label2.Text = ("Libimobiledevice Feed\r\n\r\nInfos About iDevice will show up here!\r\n");

                    this.Invoke((MethodInvoker)delegate
                    {
                        panel10.Visible = false;

                    });

                    //if (label2.Text.Contains("iPhone"))
                    //{
                    // Your code for when label2 contains "iPhone"
                    //MessageBox.Show("An iPhone is connected.");
                    //}
                });
                ShowNotificationPanel();
            }
        }

        private async void ShowNotificationPanel()
        {
            this.Invoke((MethodInvoker)delegate
            {
                panel5.Visible = true;
            });

            // Wait for 2 seconds
            await Task.Delay(2000);

            this.Invoke((MethodInvoker)delegate
            {
                panel5.Visible = false;
            });
        }


        //MessageBox.Show($"USB device change detected: {deviceName}");

        private void Form13_Load(object sender, EventArgs e)
        {

        }

        static void EnterRecoveryMode(string udid)
        {
            string argument = udid != null ? $"-u {udid}" : "";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "C:\\iCures\\Dependencies\\lim\\ideviceenterrecovery.exe";
            startInfo.Arguments = argument;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve the UDID
            ProcessStartInfo udidStartInfo = new ProcessStartInfo();
            udidStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevice_id.exe";
            udidStartInfo.Arguments = "-l";
            udidStartInfo.RedirectStandardOutput = true;
            udidStartInfo.UseShellExecute = false;

            Process udidProcess = new Process();
            udidProcess.StartInfo = udidStartInfo;
            udidProcess.Start();

            string udid = udidProcess.StandardOutput.ReadToEnd().Trim();
            udidProcess.WaitForExit();

            // Enter recovery mode
            EnterRecoveryMode(udid);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            connectWatcher.Stop();
            connectWatcher.Dispose();
            disconnectWatcher.Stop();
            disconnectWatcher.Dispose();
        }

        static void FlashDevice(string[] args)
        {
            string selectedFilePath = args[0];  // Assuming the file path is passed as an argument
            string argument = "";
            if (args.Length > 1)
            {
                if (args[1] == "Erase")
                {
                    argument = "-e";
                }
            }
            string flashingProcessArguments = $"{argument} \"{selectedFilePath}\"";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idr.exe";
            startInfo.Arguments = flashingProcessArguments;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        private string selectedFilePath = ""; // Variable to store selected file path

        private void button13_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Apple IPSW files (*.ipsw)|*.ipsw";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog1.FileName;
                textBox1.Text = (selectedFilePath);
            }
        }

        private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                Console.WriteLine(outLine.Data);
            }
        }

        private void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                Console.WriteLine(outLine.Data);
            }
        }



        private void button14_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("The Programm will display a Error message on second reconnect before sending iBEC, this is normal, just click continiue. Its Not Dangerous.");
                string eraseDevice = radioButton1.Checked ? "-e " : "";
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idr.exe";
                startInfo.Arguments = $"{eraseDevice}\"{selectedFilePath}\"";
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                //MessageBox.Show($"Selected File Path: {eraseDevice}");


                Process process = new Process();
                process.StartInfo = startInfo;

                process.Start();
                process.WaitForExit();
            }
            else
            {
                MessageBox.Show("Please select a file before flashing.");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\gaster.bat");
            // Initiate the enter custom Pwned DFU mode process
            ProcessStartInfo enterCustomPwnedDFUStartInfo = new ProcessStartInfo();
            enterCustomPwnedDFUStartInfo.FileName = "C:\\iCures\\Dependencies\\gaster.exe"; // Replace with actual file path
            enterCustomPwnedDFUStartInfo.Arguments = "pwn";
            enterCustomPwnedDFUStartInfo.CreateNoWindow = false;
            enterCustomPwnedDFUStartInfo.UseShellExecute = false;

            Process enterCustomPwnedDFUProcess = new Process();
            enterCustomPwnedDFUProcess.StartInfo = enterCustomPwnedDFUStartInfo;
            enterCustomPwnedDFUProcess.Start();
            enterCustomPwnedDFUProcess.WaitForExit();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\idpwn.bat");
            // Initiate the enter Pwned DFU mode process
            ProcessStartInfo enterPwnedDFUStartInfo = new ProcessStartInfo();
            enterPwnedDFUStartInfo.FileName = "C:\\iCures\\Dependencies\\libimdevice\\libimdevice\\idevicerestore.exe";
            enterPwnedDFUStartInfo.Arguments = "--pwn";
            enterPwnedDFUStartInfo.CreateNoWindow = false;
            enterPwnedDFUStartInfo.UseShellExecute = false;

            Process enterPwnedDFUProcess = new Process();
            enterPwnedDFUProcess.StartInfo = enterPwnedDFUStartInfo;
            enterPwnedDFUProcess.Start();
            enterPwnedDFUProcess.WaitForExit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\idact-x64.bat");
            string activationServer = "https://buyhamstersinbulk.000webhostapp.com/hamster.php"; // Replace with actual activation server URL

            // Initiate the activation process
            ProcessStartInfo activateStartInfo = new ProcessStartInfo();
            activateStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\ideviceactivation.exe";
            activateStartInfo.Arguments = $"activate -s {activationServer} -d";
            activateStartInfo.CreateNoWindow = false;
            activateStartInfo.UseShellExecute = false;

            Process activateProcess = new Process();
            activateProcess.StartInfo = activateStartInfo;
            activateProcess.Start();
            activateProcess.WaitForExit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\rec.bat");
            // Initiate the exit recovery mode process
            ProcessStartInfo exitRecoveryStartInfo = new ProcessStartInfo();
            exitRecoveryStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\irecovery.exe";
            exitRecoveryStartInfo.Arguments = "-n";
            exitRecoveryStartInfo.CreateNoWindow = false;
            exitRecoveryStartInfo.UseShellExecute = false;

            Process exitRecoveryProcess = new Process();
            exitRecoveryProcess.StartInfo = exitRecoveryStartInfo;
            exitRecoveryProcess.Start();
            exitRecoveryProcess.WaitForExit();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("If you get ERROR: Unable to place device into restore mode while flashing, Update your iTunes installation and restart " +
                "Your Computer, then Open Device Manager and choose Apple Recovery (iBoot) USB Composite Device, Click Uninstall and check the other box too " +
                "That should resolve the issue and you should be able to flash.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcessStartInfo udidStartInfo = new ProcessStartInfo();
            udidStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevice_id.exe";
            udidStartInfo.Arguments = "-l";
            udidStartInfo.RedirectStandardOutput = true;
            udidStartInfo.UseShellExecute = false;

            Process udidProcess = new Process();
            udidProcess.StartInfo = udidStartInfo;
            udidProcess.Start();

            string udid = udidProcess.StandardOutput.ReadToEnd().Trim();
            udidProcess.WaitForExit();

            MessageBox.Show("UDID: " + udid, "Device UDID");

            // Option to copy UDID to clipboard
            DialogResult copyResult = MessageBox.Show("Do you want to copy the UDID to the clipboard?", "Copy UDID", MessageBoxButtons.YesNo);

            if (copyResult == DialogResult.Yes)
            {
                Clipboard.SetText(udid);
                MessageBox.Show("UDID copied to clipboard.", "Copy UDID");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Retrieve the UDID
            ProcessStartInfo udidStartInfo = new ProcessStartInfo();
            udidStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevice_id.exe";
            udidStartInfo.Arguments = "-l";
            udidStartInfo.RedirectStandardOutput = true;
            udidStartInfo.UseShellExecute = false;

            Process udidProcess = new Process();
            udidProcess.StartInfo = udidStartInfo;
            udidProcess.Start();

            string udid = udidProcess.StandardOutput.ReadToEnd().Trim();
            udidProcess.WaitForExit();

            // Ask user to select a directory for backup
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string backupDirectory = folderDialog.SelectedPath;

                // Initiate the backup process
                ProcessStartInfo backupStartInfo = new ProcessStartInfo();
                backupStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevicebackup2.exe";
                backupStartInfo.Arguments = $"-u {udid} backup \"{backupDirectory}\"";
                backupStartInfo.CreateNoWindow = false;
                backupStartInfo.UseShellExecute = false;

                Process backupProcess = new Process();
                backupProcess.StartInfo = backupStartInfo;
                backupProcess.Start();
                backupProcess.WaitForExit();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Retrieve the UDID
            ProcessStartInfo udidStartInfo = new ProcessStartInfo();
            udidStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevice_id.exe";
            udidStartInfo.Arguments = "-l";
            udidStartInfo.RedirectStandardOutput = true;
            udidStartInfo.UseShellExecute = false;

            Process udidProcess = new Process();
            udidProcess.StartInfo = udidStartInfo;
            udidProcess.Start();

            string udid = udidProcess.StandardOutput.ReadToEnd().Trim();
            udidProcess.WaitForExit();

            // Ask user to select a directory for backup
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string backupDirectory = folderDialog.SelectedPath;

                // Initiate the backup process
                ProcessStartInfo backupStartInfo = new ProcessStartInfo();
                backupStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevicebackup.exe";
                backupStartInfo.Arguments = $"-u {udid} backup \"{backupDirectory}\"";
                backupStartInfo.CreateNoWindow = false;
                backupStartInfo.UseShellExecute = false;

                Process backupProcess = new Process();
                backupProcess.StartInfo = backupStartInfo;
                backupProcess.Start();
                backupProcess.WaitForExit();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Retrieve the UDID
            ProcessStartInfo udidStartInfo = new ProcessStartInfo();
            udidStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevice_id.exe";
            udidStartInfo.Arguments = "-l";
            udidStartInfo.RedirectStandardOutput = true;
            udidStartInfo.UseShellExecute = false;

            Process udidProcess = new Process();
            udidProcess.StartInfo = udidStartInfo;
            udidProcess.Start();

            string udid = udidProcess.StandardOutput.ReadToEnd().Trim();
            udidProcess.WaitForExit();

            // Restart the device
            ProcessStartInfo restartStartInfo = new ProcessStartInfo();
            restartStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevicediagnostics.exe";
            restartStartInfo.Arguments = $"restart -u {udid}";
            restartStartInfo.CreateNoWindow = false;
            restartStartInfo.UseShellExecute = false;

            Process restartProcess = new Process();
            restartProcess.StartInfo = restartStartInfo;
            restartProcess.Start();
            restartProcess.WaitForExit();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Retrieve the UDID
            ProcessStartInfo udidStartInfo = new ProcessStartInfo();
            udidStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevice_id.exe";
            udidStartInfo.Arguments = "-l";
            udidStartInfo.RedirectStandardOutput = true;
            udidStartInfo.UseShellExecute = false;

            Process udidProcess = new Process();
            udidProcess.StartInfo = udidStartInfo;
            udidProcess.Start();

            string udid = udidProcess.StandardOutput.ReadToEnd().Trim();
            udidProcess.WaitForExit();

            // Restart the device
            ProcessStartInfo restartStartInfo = new ProcessStartInfo();
            restartStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevicediagnostics.exe";
            restartStartInfo.Arguments = $"shutdown -u {udid}";
            restartStartInfo.CreateNoWindow = false;
            restartStartInfo.UseShellExecute = false;

            Process restartProcess = new Process();
            restartProcess.StartInfo = restartStartInfo;
            restartProcess.Start();
            restartProcess.WaitForExit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Initiate the deactivation process
            ProcessStartInfo deactivateStartInfo = new ProcessStartInfo();
            deactivateStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\ideviceactivation.exe";
            deactivateStartInfo.Arguments = "deactivate";
            deactivateStartInfo.CreateNoWindow = false;
            deactivateStartInfo.UseShellExecute = false;

            Process deactivateProcess = new Process();
            deactivateProcess.StartInfo = deactivateStartInfo;
            deactivateProcess.Start();
            deactivateProcess.WaitForExit();
        }

        private string PromptUserForPort(string message)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Enter Port",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label defaultPortsLabel = new Label() { Left = 50, Top = 20, Width = 300, Text = "Default ports are 2222 (local) and 22 (iDevice)." };
            Label textLabel = new Label() { Left = 50, Top = 70, Text = message };
            TextBox textBox = new TextBox() { Left = 50, Top = 100, Width = 300 };
            Button confirmation = new Button() { Text = "Ok", Left = 150, Width = 100, Top = 130, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(defaultPortsLabel);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "default";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Retrieve the UDID
            ProcessStartInfo udidStartInfo = new ProcessStartInfo();
            udidStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\idevice_id.exe";
            udidStartInfo.Arguments = "-l";
            udidStartInfo.RedirectStandardOutput = true;
            udidStartInfo.UseShellExecute = false;

            Process udidProcess = new Process();
            udidProcess.StartInfo = udidStartInfo;
            udidProcess.Start();

            string udid = udidProcess.StandardOutput.ReadToEnd().Trim();
            udidProcess.WaitForExit();

            // Prompt user for custom ports
            string localPort = PromptUserForPort("Enter local port (default: 2222):");
            string iDevicePort = PromptUserForPort("Enter iDevice port (default: 22):");

            // Use TCP/SSH on the iDevice
            ProcessStartInfo iproxyStartInfo = new ProcessStartInfo();
            iproxyStartInfo.FileName = "C:\\iCures\\Dependencies\\lim\\iproxy.exe"; // Replace with actual file path
            iproxyStartInfo.Arguments = $"-u {udid} {localPort} {iDevicePort}";
            iproxyStartInfo.CreateNoWindow = false;
            iproxyStartInfo.UseShellExecute = false;

            Process iproxyProcess = new Process();
            iproxyProcess.StartInfo = iproxyStartInfo;
            iproxyProcess.Start();
            iproxyProcess.WaitForExit();
        }
    }
    }

