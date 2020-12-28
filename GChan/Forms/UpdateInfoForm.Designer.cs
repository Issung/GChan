
namespace GChan.Forms
{
    partial class UpdateInfoForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateInfoForm));
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.viewReleasesButton = new System.Windows.Forms.Button();
            this.doNotUpdateButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.downloadingPanel = new System.Windows.Forms.Panel();
            this.downloadingUpdateLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.flowLayoutPanel.SuspendLayout();
            this.buttonsPanel.SuspendLayout();
            this.downloadingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Controls.Add(this.infoLabel);
            this.flowLayoutPanel.Controls.Add(this.buttonsPanel);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(7, 7);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(330, 247);
            this.flowLayoutPanel.TabIndex = 0;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.Location = new System.Drawing.Point(25, 25);
            this.infoLabel.Margin = new System.Windows.Forms.Padding(25);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(283, 99);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "Info";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Controls.Add(this.viewReleasesButton);
            this.buttonsPanel.Controls.Add(this.doNotUpdateButton);
            this.buttonsPanel.Controls.Add(this.updateButton);
            this.buttonsPanel.Location = new System.Drawing.Point(3, 152);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(326, 88);
            this.buttonsPanel.TabIndex = 1;
            // 
            // viewReleasesButton
            // 
            this.viewReleasesButton.Location = new System.Drawing.Point(3, 3);
            this.viewReleasesButton.Name = "viewReleasesButton";
            this.viewReleasesButton.Size = new System.Drawing.Size(320, 23);
            this.viewReleasesButton.TabIndex = 0;
            this.viewReleasesButton.Text = "View Releases on GitHub";
            this.toolTip.SetToolTip(this.viewReleasesButton, "View the releases on GChan\'s official GitHub page at https://github.com/Issung/GC" +
        "han/releases.");
            this.viewReleasesButton.UseVisualStyleBackColor = true;
            this.viewReleasesButton.Click += new System.EventHandler(this.viewReleasesButton_Click);
            // 
            // doNotUpdateButton
            // 
            this.doNotUpdateButton.Location = new System.Drawing.Point(3, 32);
            this.doNotUpdateButton.Name = "doNotUpdateButton";
            this.doNotUpdateButton.Size = new System.Drawing.Size(320, 23);
            this.doNotUpdateButton.TabIndex = 1;
            this.doNotUpdateButton.Text = "Later";
            this.doNotUpdateButton.UseVisualStyleBackColor = true;
            this.doNotUpdateButton.Click += new System.EventHandler(this.doNotUpdateButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(3, 61);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(320, 23);
            this.updateButton.TabIndex = 2;
            this.updateButton.Text = "Download and Install Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // downloadingPanel
            // 
            this.downloadingPanel.Controls.Add(this.downloadingUpdateLabel);
            this.downloadingPanel.Controls.Add(this.progressBar);
            this.downloadingPanel.Location = new System.Drawing.Point(10, 157);
            this.downloadingPanel.Name = "downloadingPanel";
            this.downloadingPanel.Size = new System.Drawing.Size(327, 90);
            this.downloadingPanel.TabIndex = 1;
            this.downloadingPanel.Visible = false;
            // 
            // downloadingUpdateLabel
            // 
            this.downloadingUpdateLabel.Location = new System.Drawing.Point(12, 18);
            this.downloadingUpdateLabel.Name = "downloadingUpdateLabel";
            this.downloadingUpdateLabel.Size = new System.Drawing.Size(301, 23);
            this.downloadingUpdateLabel.TabIndex = 1;
            this.downloadingUpdateLabel.Text = "Downloading update... Please wait...";
            this.downloadingUpdateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 52);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(301, 23);
            this.progressBar.TabIndex = 0;
            // 
            // UpdateInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 261);
            this.ControlBox = false;
            this.Controls.Add(this.downloadingPanel);
            this.Controls.Add(this.flowLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(360, 300);
            this.MinimumSize = new System.Drawing.Size(360, 300);
            this.Name = "UpdateInfoForm";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update Information";
            this.flowLayoutPanel.ResumeLayout(false);
            this.buttonsPanel.ResumeLayout(false);
            this.downloadingPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button viewReleasesButton;
        private System.Windows.Forms.Button doNotUpdateButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label downloadingUpdateLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.FlowLayoutPanel buttonsPanel;
        private System.Windows.Forms.Panel downloadingPanel;
    }
}