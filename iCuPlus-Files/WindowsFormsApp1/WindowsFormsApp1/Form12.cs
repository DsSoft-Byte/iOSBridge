using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace WindowsFormsApp1
{
    public partial class Form12 : Form
    {
        private ManagementEventWatcher watcher;

        public Form12()
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
            }
        }

        private void Form12_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string remoteVersionURL = "https://dssoft.us.to/ver.txt";
            WebClient webClient = new WebClient();
            //webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2");
            string remoteVersionText = webClient.DownloadString(remoteVersionURL).Trim();
            MessageBox.Show(remoteVersionText);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            watcher.Stop();
            watcher.Dispose();
        }
    }
}
