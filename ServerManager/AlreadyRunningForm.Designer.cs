namespace ServerManager
{
    partial class AlreadyRunningForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlreadyRunningForm));
            this.warningIconBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ipLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.forceButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.warningIconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // warningIconBox
            // 
            this.warningIconBox.Location = new System.Drawing.Point(20, 20);
            this.warningIconBox.Name = "warningIconBox";
            this.warningIconBox.Size = new System.Drawing.Size(40, 40);
            this.warningIconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.warningIconBox.TabIndex = 0;
            this.warningIconBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(66, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(253, 40);
            this.label1.TabIndex = 1;
            this.label1.Text = "The server is already running somewhere else. If you are sure this is not the cas" +
    "e, please press the \'FORCE\' button to force run it over here.";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(17, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(302, 43);
            this.label2.TabIndex = 2;
            this.label2.Text = "Please be aware that if you force run the server and it is running somewhere else" +
    ", datacorruption may occur and you will lose your server files.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(17, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "IP: ";
            // 
            // ipLabel
            // 
            this.ipLabel.AutoEllipsis = true;
            this.ipLabel.Location = new System.Drawing.Point(47, 106);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(272, 17);
            this.ipLabel.TabIndex = 4;
            this.ipLabel.Text = "IP Missing!";
            this.ipLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ipLabel.Click += new System.EventHandler(this.IpLabel_Click);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(89, 135);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // forceButton
            // 
            this.forceButton.Location = new System.Drawing.Point(170, 135);
            this.forceButton.Name = "forceButton";
            this.forceButton.Size = new System.Drawing.Size(75, 23);
            this.forceButton.TabIndex = 6;
            this.forceButton.Text = "Force";
            this.forceButton.UseVisualStyleBackColor = true;
            this.forceButton.Click += new System.EventHandler(this.ForceButton_Click);
            // 
            // AlreadyRunningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 170);
            this.Controls.Add(this.forceButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.ipLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.warningIconBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlreadyRunningForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server already running!";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AlreadyRunningForm_FormClosing);
            this.Shown += new System.EventHandler(this.AlreadyRunningForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.warningIconBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox warningIconBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button forceButton;
    }
}