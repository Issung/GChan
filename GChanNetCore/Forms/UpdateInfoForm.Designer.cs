
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateInfoForm));
            flowLayoutPanel = new FlowLayoutPanel();
            infoLabel = new Label();
            buttonsPanel = new FlowLayoutPanel();
            viewReleasesButton = new Button();
            doNotUpdateButton = new Button();
            updateButton = new Button();
            toolTip = new ToolTip(components);
            downloadingPanel = new Panel();
            downloadingUpdateLabel = new Label();
            progressBar = new ProgressBar();
            flowLayoutPanel.SuspendLayout();
            buttonsPanel.SuspendLayout();
            downloadingPanel.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel.Controls.Add(infoLabel);
            flowLayoutPanel.Controls.Add(buttonsPanel);
            flowLayoutPanel.Location = new Point(8, 8);
            flowLayoutPanel.Margin = new Padding(4, 3, 4, 3);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(385, 242);
            flowLayoutPanel.TabIndex = 0;
            // 
            // infoLabel
            // 
            infoLabel.Location = new Point(18, 17);
            infoLabel.Margin = new Padding(18, 17, 18, 17);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new Size(350, 97);
            infoLabel.TabIndex = 0;
            infoLabel.Text = resources.GetString("infoLabel.Text");
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonsPanel
            // 
            buttonsPanel.Controls.Add(viewReleasesButton);
            buttonsPanel.Controls.Add(doNotUpdateButton);
            buttonsPanel.Controls.Add(updateButton);
            buttonsPanel.Location = new Point(4, 134);
            buttonsPanel.Margin = new Padding(4, 3, 4, 3);
            buttonsPanel.Name = "buttonsPanel";
            buttonsPanel.Size = new Size(380, 102);
            buttonsPanel.TabIndex = 1;
            // 
            // viewReleasesButton
            // 
            viewReleasesButton.Location = new Point(4, 3);
            viewReleasesButton.Margin = new Padding(4, 3, 4, 3);
            viewReleasesButton.Name = "viewReleasesButton";
            viewReleasesButton.Size = new Size(373, 27);
            viewReleasesButton.TabIndex = 0;
            viewReleasesButton.Text = "View Releases on GitHub";
            toolTip.SetToolTip(viewReleasesButton, "View the releases on GChan's official GitHub page at https://github.com/Issung/GChan/releases.");
            viewReleasesButton.UseVisualStyleBackColor = true;
            viewReleasesButton.Click += viewReleasesButton_Click;
            // 
            // doNotUpdateButton
            // 
            doNotUpdateButton.Location = new Point(4, 36);
            doNotUpdateButton.Margin = new Padding(4, 3, 4, 3);
            doNotUpdateButton.Name = "doNotUpdateButton";
            doNotUpdateButton.Size = new Size(373, 27);
            doNotUpdateButton.TabIndex = 1;
            doNotUpdateButton.Text = "Later";
            doNotUpdateButton.UseVisualStyleBackColor = true;
            doNotUpdateButton.Click += doNotUpdateButton_Click;
            // 
            // updateButton
            // 
            updateButton.Location = new Point(4, 69);
            updateButton.Margin = new Padding(4, 3, 4, 3);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(373, 27);
            updateButton.TabIndex = 2;
            updateButton.Text = "Download and Install Update";
            updateButton.UseVisualStyleBackColor = true;
            updateButton.Click += updateButton_Click;
            // 
            // downloadingPanel
            // 
            downloadingPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            downloadingPanel.Controls.Add(downloadingUpdateLabel);
            downloadingPanel.Controls.Add(progressBar);
            downloadingPanel.Location = new Point(10, 143);
            downloadingPanel.Margin = new Padding(4, 3, 4, 3);
            downloadingPanel.MaximumSize = new Size(382, 103);
            downloadingPanel.MinimumSize = new Size(382, 103);
            downloadingPanel.Name = "downloadingPanel";
            downloadingPanel.Size = new Size(382, 103);
            downloadingPanel.TabIndex = 1;
            downloadingPanel.Visible = false;
            // 
            // downloadingUpdateLabel
            // 
            downloadingUpdateLabel.Location = new Point(14, 21);
            downloadingUpdateLabel.Margin = new Padding(4, 0, 4, 0);
            downloadingUpdateLabel.Name = "downloadingUpdateLabel";
            downloadingUpdateLabel.Size = new Size(351, 27);
            downloadingUpdateLabel.TabIndex = 1;
            downloadingUpdateLabel.Text = "Downloading update... Please wait...";
            downloadingUpdateLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(14, 60);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(351, 27);
            progressBar.TabIndex = 0;
            // 
            // UpdateInfoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(401, 258);
            ControlBox = false;
            Controls.Add(downloadingPanel);
            Controls.Add(flowLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(417, 340);
            Name = "UpdateInfoForm";
            Padding = new Padding(8);
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Update Information";
            flowLayoutPanel.ResumeLayout(false);
            buttonsPanel.ResumeLayout(false);
            downloadingPanel.ResumeLayout(false);
            ResumeLayout(false);
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