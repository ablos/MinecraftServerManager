namespace ServerManager
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.memorySelection = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.useNGROKBox = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.customIPTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.localPortBox = new System.Windows.Forms.TextBox();
            this.serverDirectoryBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.serverFilenameBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ngrokDirectoryBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.selectServerDirectoryButton = new System.Windows.Forms.Button();
            this.selectServerFilenameButton = new System.Windows.Forms.Button();
            this.selectNGROKDirectoryButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.memorySelection)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Memory:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(267, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "GB";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // memorySelection
            // 
            this.memorySelection.BackColor = System.Drawing.Color.Black;
            this.memorySelection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.memorySelection.ForeColor = System.Drawing.Color.White;
            this.memorySelection.Location = new System.Drawing.Point(162, 7);
            this.memorySelection.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.memorySelection.Name = "memorySelection";
            this.memorySelection.Size = new System.Drawing.Size(99, 20);
            this.memorySelection.TabIndex = 2;
            this.memorySelection.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.memorySelection.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Use NGROK:";
            // 
            // useNGROKBox
            // 
            this.useNGROKBox.BackColor = System.Drawing.Color.Black;
            this.useNGROKBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.useNGROKBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.useNGROKBox.ForeColor = System.Drawing.Color.White;
            this.useNGROKBox.FormattingEnabled = true;
            this.useNGROKBox.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.useNGROKBox.Location = new System.Drawing.Point(162, 33);
            this.useNGROKBox.MaxDropDownItems = 2;
            this.useNGROKBox.Name = "useNGROKBox";
            this.useNGROKBox.Size = new System.Drawing.Size(127, 21);
            this.useNGROKBox.TabIndex = 5;
            this.useNGROKBox.SelectedIndexChanged += new System.EventHandler(this.UseNGROKBox_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(146, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(10, 20);
            this.textBox1.TabIndex = 6;
            this.textBox1.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(12, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Custom IP:";
            // 
            // customIPTextBox
            // 
            this.customIPTextBox.BackColor = System.Drawing.Color.Black;
            this.customIPTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.customIPTextBox.Location = new System.Drawing.Point(110, 60);
            this.customIPTextBox.Name = "customIPTextBox";
            this.customIPTextBox.Size = new System.Drawing.Size(179, 20);
            this.customIPTextBox.TabIndex = 8;
            this.customIPTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomIPTextBox_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(12, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Local port:";
            // 
            // localPortBox
            // 
            this.localPortBox.BackColor = System.Drawing.Color.Black;
            this.localPortBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.localPortBox.ForeColor = System.Drawing.Color.White;
            this.localPortBox.Location = new System.Drawing.Point(110, 85);
            this.localPortBox.Name = "localPortBox";
            this.localPortBox.Size = new System.Drawing.Size(179, 20);
            this.localPortBox.TabIndex = 10;
            this.localPortBox.Text = "25565";
            this.localPortBox.TextChanged += new System.EventHandler(this.LocalPortBox_TextChanged);
            // 
            // serverDirectoryBox
            // 
            this.serverDirectoryBox.BackColor = System.Drawing.Color.Black;
            this.serverDirectoryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverDirectoryBox.ForeColor = System.Drawing.Color.White;
            this.serverDirectoryBox.Location = new System.Drawing.Point(110, 110);
            this.serverDirectoryBox.Name = "serverDirectoryBox";
            this.serverDirectoryBox.Size = new System.Drawing.Size(151, 20);
            this.serverDirectoryBox.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(12, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Server directory:";
            // 
            // serverFilenameBox
            // 
            this.serverFilenameBox.BackColor = System.Drawing.Color.Black;
            this.serverFilenameBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverFilenameBox.ForeColor = System.Drawing.Color.White;
            this.serverFilenameBox.Location = new System.Drawing.Point(110, 136);
            this.serverFilenameBox.Name = "serverFilenameBox";
            this.serverFilenameBox.Size = new System.Drawing.Size(151, 20);
            this.serverFilenameBox.TabIndex = 16;
            this.serverFilenameBox.Text = "server.jar";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(12, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Server filename:";
            // 
            // ngrokDirectoryBox
            // 
            this.ngrokDirectoryBox.BackColor = System.Drawing.Color.Black;
            this.ngrokDirectoryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ngrokDirectoryBox.ForeColor = System.Drawing.Color.White;
            this.ngrokDirectoryBox.Location = new System.Drawing.Point(110, 162);
            this.ngrokDirectoryBox.Name = "ngrokDirectoryBox";
            this.ngrokDirectoryBox.Size = new System.Drawing.Size(151, 20);
            this.ngrokDirectoryBox.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(12, 165);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "NGROK directory:";
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.Black;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ForeColor = System.Drawing.Color.White;
            this.closeButton.Location = new System.Drawing.Point(112, 201);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 19;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // selectServerDirectoryButton
            // 
            this.selectServerDirectoryButton.BackColor = System.Drawing.Color.Black;
            this.selectServerDirectoryButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.selectServerDirectoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectServerDirectoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.selectServerDirectoryButton.ForeColor = System.Drawing.Color.White;
            this.selectServerDirectoryButton.Location = new System.Drawing.Point(268, 111);
            this.selectServerDirectoryButton.Name = "selectServerDirectoryButton";
            this.selectServerDirectoryButton.Size = new System.Drawing.Size(21, 19);
            this.selectServerDirectoryButton.TabIndex = 20;
            this.selectServerDirectoryButton.Text = "...";
            this.selectServerDirectoryButton.UseVisualStyleBackColor = false;
            this.selectServerDirectoryButton.Click += new System.EventHandler(this.SelectServerDirectoryButton_Click);
            // 
            // selectServerFilenameButton
            // 
            this.selectServerFilenameButton.BackColor = System.Drawing.Color.Black;
            this.selectServerFilenameButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.selectServerFilenameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectServerFilenameButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.selectServerFilenameButton.ForeColor = System.Drawing.Color.White;
            this.selectServerFilenameButton.Location = new System.Drawing.Point(268, 137);
            this.selectServerFilenameButton.Name = "selectServerFilenameButton";
            this.selectServerFilenameButton.Size = new System.Drawing.Size(21, 19);
            this.selectServerFilenameButton.TabIndex = 21;
            this.selectServerFilenameButton.Text = "...";
            this.selectServerFilenameButton.UseVisualStyleBackColor = false;
            this.selectServerFilenameButton.Click += new System.EventHandler(this.SelectServerFilenameButton_Click);
            // 
            // selectNGROKDirectoryButton
            // 
            this.selectNGROKDirectoryButton.BackColor = System.Drawing.Color.Black;
            this.selectNGROKDirectoryButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.selectNGROKDirectoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectNGROKDirectoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.selectNGROKDirectoryButton.ForeColor = System.Drawing.Color.White;
            this.selectNGROKDirectoryButton.Location = new System.Drawing.Point(268, 164);
            this.selectNGROKDirectoryButton.Name = "selectNGROKDirectoryButton";
            this.selectNGROKDirectoryButton.Size = new System.Drawing.Size(21, 19);
            this.selectNGROKDirectoryButton.TabIndex = 22;
            this.selectNGROKDirectoryButton.Text = "...";
            this.selectNGROKDirectoryButton.UseVisualStyleBackColor = false;
            this.selectNGROKDirectoryButton.Click += new System.EventHandler(this.SelectNGROKDirectoryButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(301, 236);
            this.Controls.Add(this.selectNGROKDirectoryButton);
            this.Controls.Add(this.selectServerFilenameButton);
            this.Controls.Add(this.selectServerDirectoryButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.ngrokDirectoryBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.serverFilenameBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.serverDirectoryBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.localPortBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.customIPTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.useNGROKBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.memorySelection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.memorySelection)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown memorySelection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox useNGROKBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox customIPTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox localPortBox;
        private System.Windows.Forms.TextBox serverDirectoryBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox serverFilenameBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ngrokDirectoryBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button selectServerDirectoryButton;
        private System.Windows.Forms.Button selectServerFilenameButton;
        private System.Windows.Forms.Button selectNGROKDirectoryButton;
    }
}