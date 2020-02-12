namespace ServerManager
{
    partial class SyncForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SyncForm));
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.subProgressBar = new System.Windows.Forms.ProgressBar();
            this.subStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 9);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(320, 17);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Starting sync...";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 29);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(320, 19);
            this.progressBar.TabIndex = 1;
            // 
            // subProgressBar
            // 
            this.subProgressBar.Location = new System.Drawing.Point(12, 73);
            this.subProgressBar.Name = "subProgressBar";
            this.subProgressBar.Size = new System.Drawing.Size(320, 19);
            this.subProgressBar.TabIndex = 3;
            // 
            // subStatusLabel
            // 
            this.subStatusLabel.AutoEllipsis = true;
            this.subStatusLabel.Location = new System.Drawing.Point(12, 53);
            this.subStatusLabel.Name = "subStatusLabel";
            this.subStatusLabel.Size = new System.Drawing.Size(320, 17);
            this.subStatusLabel.TabIndex = 2;
            // 
            // SyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 107);
            this.Controls.Add(this.subProgressBar);
            this.Controls.Add(this.subStatusLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SyncForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sync Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SyncForm_FormClosing);
            this.Load += new System.EventHandler(this.SyncForm_Load);
            this.Shown += new System.EventHandler(this.SyncForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ProgressBar subProgressBar;
        private System.Windows.Forms.Label subStatusLabel;
    }
}