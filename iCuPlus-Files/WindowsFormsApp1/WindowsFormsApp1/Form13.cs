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

namespace WindowsFormsApp1
{
    public partial class Form13 : Form
    {
        private ManagementEventWatcher watcher;



        public Form13()
        {
            InitializeComponent();
            watcher = new ManagementEventWatcher();
            watcher.EventArrived += new EventArrivedEventHandler(HandleEvent);
            watcher.Query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
            watcher.Start();
        }

        private void HandleEvent(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string deviceName = instance["Name"].ToString(); //OG Code
            string description = instance["Description"].ToString();

            if (description.Contains("Apple Mobile Device USB Composite Device"))
            {
                MessageBox.Show($"An Apple device connected: {deviceName}");
                System.Diagnostics.Process.Start(@"C:\\iCures\\ideviceinfopipe.bat");
                string filePath = @"C:\iCures\Dependencies\lim\output.txt";
                string fileContent = File.ReadAllText(filePath);

                this.Invoke((MethodInvoker)delegate
                {
                    label2.Text = (fileContent);
                });
            }
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
            watcher.Stop();
            watcher.Dispose();
        }
    }
}
