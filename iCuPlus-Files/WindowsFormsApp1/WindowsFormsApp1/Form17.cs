﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form17 : Form
    {
        public Form17()
        {
            InitializeComponent();
        }

        private void Form17_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string machineId = GetMachineId();

            Clipboard.SetText(machineId);

            MessageBox.Show($"Your Machine ID has been copied to the clipboard: {machineId}", "Machine ID", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
            private string GetMachineId()
            {
                string cpuId = GetHardwareIdentifier("Win32_Processor", "ProcessorId");
                string biosId = GetHardwareIdentifier("Win32_BIOS", "SerialNumber");

                string combinedId = cpuId + biosId;

                // Hash the ID for more simplicity
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedId));
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }

        private string GetHardwareIdentifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT {wmiProperty} FROM {wmiClass}");
                foreach (ManagementObject obj in searcher.Get())
                {
                    result = obj[wmiProperty]?.ToString();
                    break;
                }
            }
            catch
            {
                result = "Unknown";
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string machineId = MachineIDGenerator.GetMachineId();
            string enteredCode = textBox1.Text;

            if (VerifyActivationCode(machineId, enteredCode))
            {
                Properties.Settings.Default.IsActivated = true;
                Properties.Settings.Default.Save();
                MessageBox.Show("Activation Successful! Thank you for your support. Enjoy!");
            }
            else
            {
                MessageBox.Show("Invalid Activation Code. Please try again.");
            }
        }
        private bool VerifyActivationCode(string machineId, string activationCode)
        {
            string correctCode = GenerateActivationCode(machineId);
            return correctCode.Equals(activationCode, StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateActivationCode(string machineId)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineId + "0000078463478653465xui"));
                return BitConverter.ToString(hash).Replace("-", "").ToLower().Substring(0, 16);
            }
        }

             public static class MachineIDGenerator
        {
            public static string GetMachineId()
            {
                string cpuId = GetHardwareIdentifier("Win32_Processor", "ProcessorId");
                string biosId = GetHardwareIdentifier("Win32_BIOS", "SerialNumber");

                string combinedId = cpuId + biosId;

                // Hash the ID for simplicity
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedId));
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }

            private static string GetHardwareIdentifier(string wmiClass, string wmiProperty)
            {
                string result = "";
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT {wmiProperty} FROM {wmiClass}");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        result = obj[wmiProperty]?.ToString();
                        break;
                    }
                }
                catch
                {
                    result = "Unknown";
                }
                return result;
            }
        }
    }
}
