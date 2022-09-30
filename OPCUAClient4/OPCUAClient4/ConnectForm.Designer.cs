namespace OPCUAClient
{
    partial class ConnectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.Password = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Username = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ConnDisButton = new System.Windows.Forms.Button();
            this.useSecurity = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Password);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.Username);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(40, 89);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(233, 88);
            this.panel1.TabIndex = 17;
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(107, 52);
            this.Password.Name = "Password";
            this.Password.PasswordChar = '*';
            this.Password.Size = new System.Drawing.Size(100, 22);
            this.Password.TabIndex = 10;
            this.Password.Text = "872236878644433284323226437trantrunghieuheadbanger";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(208)))), ((int)(((byte)(44)))));
            this.label4.Location = new System.Drawing.Point(3, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 20);
            this.label4.TabIndex = 11;
            this.label4.Text = "Username";
            // 
            // Username
            // 
            this.Username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Username.Location = new System.Drawing.Point(107, 18);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(100, 22);
            this.Username.TabIndex = 9;
            this.Username.Text = "codeflac";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(208)))), ((int)(((byte)(44)))));
            this.label5.Location = new System.Drawing.Point(3, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 25);
            this.label5.TabIndex = 12;
            this.label5.Text = "Password";
            this.label5.UseCompatibleTextRendering = true;
            // 
            // ConnDisButton
            // 
            this.ConnDisButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnDisButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(208)))), ((int)(((byte)(44)))));
            this.ConnDisButton.Location = new System.Drawing.Point(211, 183);
            this.ConnDisButton.Name = "ConnDisButton";
            this.ConnDisButton.Size = new System.Drawing.Size(104, 37);
            this.ConnDisButton.TabIndex = 16;
            this.ConnDisButton.Text = "Connect";
            this.ConnDisButton.UseVisualStyleBackColor = false;
            this.ConnDisButton.Click += new System.EventHandler(this.ConnDisButton_Click);
            // 
            // useSecurity
            // 
            this.useSecurity.AutoSize = true;
            this.useSecurity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.useSecurity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(208)))), ((int)(((byte)(44)))));
            this.useSecurity.Location = new System.Drawing.Point(40, 59);
            this.useSecurity.Name = "useSecurity";
            this.useSecurity.Size = new System.Drawing.Size(127, 24);
            this.useSecurity.TabIndex = 15;
            this.useSecurity.Text = "Use Security";
            this.useSecurity.UseVisualStyleBackColor = true;
            this.useSecurity.CheckedChanged += new System.EventHandler(this.useSecurity_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "opc.tcp://DESKTOP-ITCRIDT:53530/OPCUA/SimulationServer",
            "opc.tcp://127.0.0.1:49320"});
            this.comboBox1.Location = new System.Drawing.Point(40, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(275, 24);
            this.comboBox1.TabIndex = 14;
            // 
            // ConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(54)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(361, 233);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ConnDisButton);
            this.Controls.Add(this.useSecurity);
            this.Controls.Add(this.comboBox1);
            this.Name = "ConnectForm";
            this.ShowIcon = false;
            this.Text = "Connection Configuration";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MaskedTextBox Password;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox Username;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ConnDisButton;
        private System.Windows.Forms.CheckBox useSecurity;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}