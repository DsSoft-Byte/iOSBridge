namespace iCuplusupdater
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.ControlLight;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(231, 25);
            label1.TabIndex = 0;
            label1.Text = "iOSBridge product update";
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = SystemColors.ControlLight;
            button1.Location = new Point(12, 392);
            button1.Name = "button1";
            button1.Size = new Size(304, 39);
            button1.TabIndex = 1;
            button1.Text = "I'll Update later";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.Flat;
            button2.ForeColor = SystemColors.ControlLight;
            button2.Location = new Point(322, 392);
            button2.Name = "button2";
            button2.Size = new Size(310, 39);
            button2.TabIndex = 2;
            button2.Text = "Yes, Update now";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = SystemColors.ControlLight;
            label2.Location = new Point(12, 60);
            label2.Name = "label2";
            label2.Size = new Size(532, 17);
            label2.TabIndex = 3;
            label2.Text = "iOSBridge Has found Updates and downloaded them! Would you like to install them now?";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = SystemColors.ControlLight;
            label3.Location = new Point(21, 131);
            label3.Name = "label3";
            label3.Size = new Size(179, 17);
            label3.TabIndex = 4;
            label3.Text = "You can always roll back later";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = SystemColors.ControlLight;
            label4.Location = new Point(21, 165);
            label4.Name = "label4";
            label4.Size = new Size(162, 17);
            label4.TabIndex = 5;
            label4.Text = "We always add, never take";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label5.ForeColor = SystemColors.ControlLight;
            label5.Location = new Point(21, 199);
            label5.Name = "label5";
            label5.Size = new Size(184, 17);
            label5.TabIndex = 6;
            label5.Text = "This ensures working software";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label6.ForeColor = SystemColors.ControlLight;
            label6.Location = new Point(21, 232);
            label6.Name = "label6";
            label6.Size = new Size(292, 17);
            label6.TabIndex = 7;
            label6.Text = "We only provide support for the newest versions";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label7.ForeColor = SystemColors.ControlLight;
            label7.Location = new Point(21, 265);
            label7.Name = "label7";
            label7.Size = new Size(355, 17);
            label7.TabIndex = 8;
            label7.Text = "Updates always provide performance/speed improvements";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label8.ForeColor = SystemColors.ControlLight;
            label8.Location = new Point(21, 296);
            label8.Name = "label8";
            label8.Size = new Size(207, 17);
            label8.TabIndex = 9;
            label8.Text = "Time needed estimate: 5 Seconds.";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(44, 44, 44);
            ClientSize = new Size(644, 443);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "DsSoft Standalone Updater 1.5";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
        private Button button2;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
    }
}