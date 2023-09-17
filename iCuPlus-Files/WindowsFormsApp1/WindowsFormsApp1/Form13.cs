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

            if (description.Contains("Apple Mobile Device USB Composite Device"))
            {
                MessageBox.Show($"An Apple device connected: {deviceName}");
                Thread.Sleep(10);
                System.Diagnostics.Process.Start(@"C:\\iCures\\ideviceinfopipe.bat");
                Thread.Sleep(10);
                string filePath = @"C:\iCures\Dependencies\lim\output.txt";
                Thread.Sleep(1000);
                string fileContent = File.ReadAllText(filePath);

                this.Invoke((MethodInvoker)delegate
                {
                    label2.Text = (fileContent);
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
                    label2.Text = ("Libimobiledevice Feed");
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
    }
}
