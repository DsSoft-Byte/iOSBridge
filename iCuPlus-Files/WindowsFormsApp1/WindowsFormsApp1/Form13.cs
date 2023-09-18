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
using iMobileDevice;
using iMobileDevice.iDevice;
using iMobileDevice.Lockdown;

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

            if (description.Contains("Apple Mobile Device USB Composite Device"))
            {
                //MessageBox.Show($"An Apple device connected: {deviceName}");
                Thread.Sleep(10);
                System.Diagnostics.Process.Start(@"C:\\iCures\\ideviceinfopipe.bat");
                Thread.Sleep(10);
                string filePath = @"C:\iCures\Dependencies\lim\output.txt";
                Thread.Sleep(1000);
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

            if (description.Contains("Apple Mobile Device USB Composite Device"))
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

        private void button1_Click(object sender, EventArgs e)
        {

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
        }
    }

