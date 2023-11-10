using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Label label1;
        private Panel panel1;
        private Label label2;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private Panel panel2;
        private Label label4;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private Label label3;
        private Panel panel3;
        private Label label5;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private Label label6;
        private Panel panel4;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button17;
        private Label label7;
        private Panel panel5;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button button20;
        private Label label8;
        private Panel panel6;
        private System.Windows.Forms.Button button24;
        private System.Windows.Forms.Button button21;
        private System.Windows.Forms.Button button22;
        private Label label9;
        private Label label11;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button26;
        private System.Windows.Forms.Button button16;
        private SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button31;
        private Label label13;
        private Label label15;
        private LinkLabel linkLabel1;
        private Label label10;
        private Label label12;
        private Label label16;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Process.Start("cmd.exe");
        }

        private void button22_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form2 = new Form1();
            form2.ShowDialog();
        }

        class Program
        {
            static void Lolhamburgir(string[] args)
            {
                string startPath = @".\start";
                string zipPath = @".\result.zip";
                string extractPath = @".\extract";

                ZipFile.CreateFromDirectory(startPath, zipPath);

                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
        }
        private void button21_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\extract\iCures\CB-A-config.txt"))
            {
                MessageBox.Show("Update files are present. ERROR CODE 2");
            }
            else
            {
                // Download Zip file using WebClient from github link, save to Updater.zip, Extract to extracted. Pass onto updater then.
                WebClient webClient = new WebClient();
                webClient.DownloadFile(new Uri("https://raw.githubusercontent.com/DsSoft-Byte/iCu/main/iCures_Updater.zip"), @"C:\Updater.zip");
                string zipPath = @"C:\Updater.zip";
                string extractPath = @"C:\extract";

                ZipFile.ExtractToDirectory(zipPath, extractPath);
                //MessageBox.Show("Downloaded and Extracted!");
                //Start Standalone Updater
            }

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.button12 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button26 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button31 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button18 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button24 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.button22 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label12 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(377, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "iOSBridge - The Ultimate iOS Multitool";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.panel1.Controls.Add(this.button8);
            this.panel1.Controls.Add(this.button7);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(16, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(294, 155);
            this.panel1.TabIndex = 7;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button8.Location = new System.Drawing.Point(13, 108);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(269, 28);
            this.button8.TabIndex = 11;
            this.button8.Text = "Run gaster";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button7.Location = new System.Drawing.Point(13, 74);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(269, 28);
            this.button7.TabIndex = 10;
            this.button7.Text = "Use Zadig";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button6.Location = new System.Drawing.Point(13, 38);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(269, 31);
            this.button6.TabIndex = 9;
            this.button6.Text = "Install MinGW (Optional)";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label2.Location = new System.Drawing.Point(87, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Pwned DFU mode A7-A11";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.button12);
            this.panel2.Controls.Add(this.button10);
            this.panel2.Controls.Add(this.button11);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(344, 91);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(294, 155);
            this.panel2.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label4.Location = new System.Drawing.Point(47, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(207, 32);
            this.label4.TabIndex = 13;
            this.label4.Text = "iPad 2 Only on 9.3.5/6. All other A5\r\nare UNSUPPORTED on iOS 8+";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button12.Location = new System.Drawing.Point(145, 40);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(137, 28);
            this.button12.TabIndex = 12;
            this.button12.Text = "Bypa$$ x64";
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button10.Location = new System.Drawing.Point(15, 73);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(267, 28);
            this.button10.TabIndex = 10;
            this.button10.Text = "Run UDID/ECID scan";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button11.Location = new System.Drawing.Point(15, 40);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(133, 28);
            this.button11.TabIndex = 9;
            this.button11.Text = "Bypa$$ x86";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label3.Location = new System.Drawing.Point(56, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(177, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Tethered PHP Bypa$$ (iPad 2 Only)";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.panel3.Controls.Add(this.button26);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.button9);
            this.panel3.Controls.Add(this.button13);
            this.panel3.Controls.Add(this.button14);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Location = new System.Drawing.Point(670, 91);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(294, 155);
            this.panel3.TabIndex = 14;
            // 
            // button26
            // 
            this.button26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button26.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button26.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button26.Location = new System.Drawing.Point(145, 73);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(137, 28);
            this.button26.TabIndex = 14;
            this.button26.Text = "Run Sideloadly i8";
            this.button26.UseVisualStyleBackColor = false;
            this.button26.Click += new System.EventHandler(this.button26_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label5.Location = new System.Drawing.Point(43, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(185, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "All iOS 7/8 Devices supported";
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button9.Location = new System.Drawing.Point(145, 40);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(137, 28);
            this.button9.TabIndex = 12;
            this.button9.Text = "Bypa$$ x64";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button13.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button13.Location = new System.Drawing.Point(15, 73);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(133, 28);
            this.button13.TabIndex = 10;
            this.button13.Text = "Run Pangu7";
            this.button13.UseVisualStyleBackColor = false;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button14
            // 
            this.button14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button14.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button14.Location = new System.Drawing.Point(15, 40);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(133, 28);
            this.button14.TabIndex = 9;
            this.button14.Text = "Bypa$$ x86";
            this.button14.UseVisualStyleBackColor = false;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label6.Location = new System.Drawing.Point(39, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(209, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "iOS 7/8 exclusive PHP untethered Bypa$$";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.panel4.Controls.Add(this.button31);
            this.panel4.Controls.Add(this.button16);
            this.panel4.Controls.Add(this.button15);
            this.panel4.Controls.Add(this.button17);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Location = new System.Drawing.Point(20, 267);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(294, 155);
            this.panel4.TabIndex = 12;
            // 
            // button31
            // 
            this.button31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button31.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button31.ForeColor = System.Drawing.Color.IndianRed;
            this.button31.Location = new System.Drawing.Point(16, 40);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(132, 28);
            this.button31.TabIndex = 13;
            this.button31.Text = "Down. Sideloadly";
            this.button31.UseVisualStyleBackColor = false;
            this.button31.Click += new System.EventHandler(this.button31_Click);
            // 
            // button16
            // 
            this.button16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button16.ForeColor = System.Drawing.Color.IndianRed;
            this.button16.Location = new System.Drawing.Point(15, 73);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(267, 28);
            this.button16.TabIndex = 12;
            this.button16.Text = "Download Etas0nJB";
            this.button16.UseVisualStyleBackColor = false;
            this.button16.Click += new System.EventHandler(this.button16_Click_1);
            // 
            // button15
            // 
            this.button15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button15.ForeColor = System.Drawing.Color.IndianRed;
            this.button15.Location = new System.Drawing.Point(15, 107);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(267, 28);
            this.button15.TabIndex = 11;
            this.button15.Text = "Control Windows Defender";
            this.button15.UseVisualStyleBackColor = false;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button17
            // 
            this.button17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button17.ForeColor = System.Drawing.Color.IndianRed;
            this.button17.Location = new System.Drawing.Point(145, 40);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(137, 28);
            this.button17.TabIndex = 9;
            this.button17.Text = "Down. XAMPP";
            this.button17.UseVisualStyleBackColor = false;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label7.Location = new System.Drawing.Point(62, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(171, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Bypa$$ Useful resources and tools";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.panel5.Controls.Add(this.button18);
            this.panel5.Controls.Add(this.button19);
            this.panel5.Controls.Add(this.button20);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Location = new System.Drawing.Point(344, 267);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(294, 155);
            this.panel5.TabIndex = 13;
            // 
            // button18
            // 
            this.button18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button18.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button18.Location = new System.Drawing.Point(15, 107);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(267, 28);
            this.button18.TabIndex = 11;
            this.button18.Text = "Limera1n DFU";
            this.button18.UseVisualStyleBackColor = false;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // button19
            // 
            this.button19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button19.Enabled = false;
            this.button19.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button19.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button19.Location = new System.Drawing.Point(15, 73);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(267, 28);
            this.button19.TabIndex = 10;
            this.button19.Text = "FCE365 Firmware Manager for CFW";
            this.button19.UseVisualStyleBackColor = false;
            this.button19.Click += new System.EventHandler(this.button19_Click);
            // 
            // button20
            // 
            this.button20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button20.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button20.Location = new System.Drawing.Point(15, 40);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(267, 28);
            this.button20.TabIndex = 9;
            this.button20.Text = "Download and Install Old iTunes";
            this.button20.UseVisualStyleBackColor = false;
            this.button20.Click += new System.EventHandler(this.button20_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label8.Location = new System.Drawing.Point(74, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(158, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Legacy tools | use on Win 7 x64";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.panel6.Controls.Add(this.label10);
            this.panel6.Controls.Add(this.button3);
            this.panel6.Controls.Add(this.button24);
            this.panel6.Controls.Add(this.button21);
            this.panel6.Controls.Add(this.button22);
            this.panel6.Controls.Add(this.label9);
            this.panel6.Location = new System.Drawing.Point(670, 267);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(294, 155);
            this.panel6.TabIndex = 14;
            this.panel6.Paint += new System.Windows.Forms.PaintEventHandler(this.panel6_Paint);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label10.Location = new System.Drawing.Point(28, 138);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(225, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "If your ISP allows open ports, host and dm me!";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button3.Location = new System.Drawing.Point(145, 73);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(137, 28);
            this.button3.TabIndex = 13;
            this.button3.Text = "Command Prompt";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // button24
            // 
            this.button24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button24.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button24.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button24.Location = new System.Drawing.Point(14, 73);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(134, 28);
            this.button24.TabIndex = 12;
            this.button24.Text = "Experimental";
            this.button24.UseVisualStyleBackColor = false;
            this.button24.Click += new System.EventHandler(this.button24_Click);
            // 
            // button21
            // 
            this.button21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button21.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button21.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button21.Location = new System.Drawing.Point(14, 107);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(268, 28);
            this.button21.TabIndex = 11;
            this.button21.Text = "Info/Help";
            this.button21.UseVisualStyleBackColor = false;
            this.button21.Click += new System.EventHandler(this.button21_Click_1);
            // 
            // button22
            // 
            this.button22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(5)))), ((int)(((byte)(126)))));
            this.button22.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button22.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button22.Location = new System.Drawing.Point(14, 40);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(268, 28);
            this.button22.TabIndex = 10;
            this.button22.Text = "Settings and Updates";
            this.button22.UseVisualStyleBackColor = false;
            this.button22.Click += new System.EventHandler(this.button22_Click_1);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label9.Location = new System.Drawing.Point(43, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(205, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Settings | Misc tools | Experimental Bypass";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label11.Location = new System.Drawing.Point(893, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 25);
            this.label11.TabIndex = 16;
            this.label11.Text = "10B0D7";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label13.Location = new System.Drawing.Point(16, 434);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(771, 20);
            this.label13.TabIndex = 14;
            this.label13.Text = "Please Download and Install all of these programms to their default locations and" +
    " disable Windows Defender";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label15.Location = new System.Drawing.Point(14, 45);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "Source Code:";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.DarkKhaki;
            this.linkLabel1.Location = new System.Drawing.Point(88, 45);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(177, 13);
            this.linkLabel1.TabIndex = 23;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "DsSoft Github iCu-X/iOSBridge repo";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label12.Location = new System.Drawing.Point(831, 267);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 20);
            this.label12.TabIndex = 24;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label16.Location = new System.Drawing.Point(955, 40);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(27, 25);
            this.label16.TabIndex = 26;
            this.label16.Text = "Y";
            this.label16.Click += new System.EventHandler(this.label16_Click);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(991, 465);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "iOSBridge V10.0.7";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();

            form3.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var client = new WebClient())
                if (File.Exists(@"C:\iCuPlus.zip"))
                    MessageBox.Show("Update file detected Error 003");
            else

            {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile("https://raw.githubusercontent.com/DsSoft-Byte/iCu/main/iCures_Updater.zip", @"C:\iCuPlus.zip");
                    string zipPath = @"C:\iCuPlus.zip";
                    string extractPath = @"C:\UpdateData";
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);

                }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\iCU\bat1.bat");
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Process.Start(@"C:\iCures\mingw-inst.exe");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\iCures\zadig27.exe");
        }

        private void button8_Click(object sender, EventArgs e)
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

        private void button11_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\idact.bat");
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

        private void button12_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\idact-x64.bat");
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

        private void button10_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\prelaunch.bat");
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

        private void button14_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\idact.bat");
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

        private void button9_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\idact-x64.bat");
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

        private void button13_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\iCures\Dependencies\Pangu\pangu_1.0-en.exe");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://sourceforge.net/projects/xampp/files/latest/download");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open the PDF in your iCures folder, its there :)");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("C:/iCures/Defendercontrol.bat");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            // Trust me, only this exec works
            Process.Start("https://www.filehorse.com/download-itunes-64/16353/download/");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Process.Start(@"");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //Process.Start(@"C:\iCures\idpwn.bat");
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

        private void button22_Click_1(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nothin' here yet.");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            MessageBox.Show("I dont know yet if this will be used");
            // Code converted from iCu, works well.
            //string sfileReader;
            //sfileReader = System.IO.File.ReadAllText(@"C:\\iCures\\CB-X-config.txt");
            //Process.Start(sfileReader);
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Debugging and Developer mode activated, to return to Production state restart the app, Developer code -10");
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            // iCu Code, VB.NET to C# extension carried me here
            string fileReader;
            fileReader = System.IO.File.ReadAllText(@"C:\iCures\CB-A-config.txt");
            Process.Start(fileReader);
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Everything is in the PDF file in your iCu X installation folder, have a read.");
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            Process.Start("https://etasonjb.tihmstar.net/ipa/etasonJB-RC5.ipa");
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void button26_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.LocalApplicationData), "sideloadly\\sideloadly.exe");
            //MessageBox.Show(fileName);
            System.Diagnostics.Process.Start(fileName);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://sideloadly.io/SideloadlySetup64.exe");
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/DsSoft-Byte/iCu-X");
        }

        private void button29_Click(object sender, EventArgs e)
        {

        }

        private void button28_Click(object sender, EventArgs e)
        {

        }

        private void button30_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A Green Y means that the bypass server is working as intended, a Red N means that the bypass server is offline or broken.");
        }
    }


}
