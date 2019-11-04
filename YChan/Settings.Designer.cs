namespace GChan {
    partial class Settings {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnSSave = new System.Windows.Forms.Button();
            this.btnSCan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.edtPath = new System.Windows.Forms.TextBox();
            this.edtTimer = new System.Windows.Forms.TextBox();
            this.chkHTML = new System.Windows.Forms.CheckBox();
            this.chkSave = new System.Windows.Forms.CheckBox();
            this.chkTray = new System.Windows.Forms.CheckBox();
            this.chkWarn = new System.Windows.Forms.CheckBox();
            this.setPathButton = new System.Windows.Forms.Button();
            this.chkStartWithWindows = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSSave
            // 
            this.btnSSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSSave.Location = new System.Drawing.Point(12, 180);
            this.btnSSave.Name = "btnSSave";
            this.btnSSave.Size = new System.Drawing.Size(92, 23);
            this.btnSSave.TabIndex = 0;
            this.btnSSave.Text = "Save";
            this.btnSSave.UseVisualStyleBackColor = true;
            this.btnSSave.Click += new System.EventHandler(this.btnSSave_Click);
            // 
            // btnSCan
            // 
            this.btnSCan.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSCan.Location = new System.Drawing.Point(110, 180);
            this.btnSCan.Name = "btnSCan";
            this.btnSCan.Size = new System.Drawing.Size(94, 23);
            this.btnSCan.TabIndex = 1;
            this.btnSCan.Text = "Cancel";
            this.btnSCan.UseVisualStyleBackColor = true;
            this.btnSCan.Click += new System.EventHandler(this.btnSCan_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Timer (sec)";
            // 
            // edtPath
            // 
            this.edtPath.Location = new System.Drawing.Point(77, 9);
            this.edtPath.Name = "edtPath";
            this.edtPath.Size = new System.Drawing.Size(96, 20);
            this.edtPath.TabIndex = 4;
            // 
            // edtTimer
            // 
            this.edtTimer.Location = new System.Drawing.Point(77, 35);
            this.edtTimer.Name = "edtTimer";
            this.edtTimer.Size = new System.Drawing.Size(126, 20);
            this.edtTimer.TabIndex = 5;
            // 
            // chkHTML
            // 
            this.chkHTML.AutoSize = true;
            this.chkHTML.Location = new System.Drawing.Point(15, 61);
            this.chkHTML.Name = "chkHTML";
            this.chkHTML.Size = new System.Drawing.Size(107, 17);
            this.chkHTML.TabIndex = 6;
            this.chkHTML.Text = "Download HTML";
            this.chkHTML.UseVisualStyleBackColor = true;
            // 
            // chkSave
            // 
            this.chkSave.AutoSize = true;
            this.chkSave.Location = new System.Drawing.Point(15, 130);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(115, 17);
            this.chkSave.TabIndex = 7;
            this.chkSave.Text = "Save URLs on exit";
            this.chkSave.UseVisualStyleBackColor = true;
            // 
            // chkTray
            // 
            this.chkTray.AutoSize = true;
            this.chkTray.Location = new System.Drawing.Point(15, 107);
            this.chkTray.Name = "chkTray";
            this.chkTray.Size = new System.Drawing.Size(102, 17);
            this.chkTray.TabIndex = 8;
            this.chkTray.Text = "Minimize to Tray";
            this.chkTray.UseVisualStyleBackColor = true;
            // 
            // chkWarn
            // 
            this.chkWarn.AutoSize = true;
            this.chkWarn.Location = new System.Drawing.Point(15, 84);
            this.chkWarn.Name = "chkWarn";
            this.chkWarn.Size = new System.Drawing.Size(116, 17);
            this.chkWarn.TabIndex = 9;
            this.chkWarn.Text = "Show Exit Warning";
            this.chkWarn.UseVisualStyleBackColor = true;
            // 
            // setPathButton
            // 
            this.setPathButton.Location = new System.Drawing.Point(179, 8);
            this.setPathButton.Name = "setPathButton";
            this.setPathButton.Size = new System.Drawing.Size(25, 22);
            this.setPathButton.TabIndex = 10;
            this.setPathButton.Text = "...";
            this.setPathButton.UseVisualStyleBackColor = true;
            this.setPathButton.Click += new System.EventHandler(this.SetPathButton_Click);
            // 
            // chkStartWithWindows
            // 
            this.chkStartWithWindows.AutoSize = true;
            this.chkStartWithWindows.Location = new System.Drawing.Point(15, 153);
            this.chkStartWithWindows.Name = "chkStartWithWindows";
            this.chkStartWithWindows.Size = new System.Drawing.Size(153, 17);
            this.chkStartWithWindows.TabIndex = 11;
            this.chkStartWithWindows.Text = "Start GChan with Windows";
            this.chkStartWithWindows.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this.btnSSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(216, 213);
            this.ControlBox = false;
            this.Controls.Add(this.chkStartWithWindows);
            this.Controls.Add(this.setPathButton);
            this.Controls.Add(this.chkHTML);
            this.Controls.Add(this.chkWarn);
            this.Controls.Add(this.btnSSave);
            this.Controls.Add(this.chkTray);
            this.Controls.Add(this.btnSCan);
            this.Controls.Add(this.chkSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.edtTimer);
            this.Controls.Add(this.edtPath);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(232, 252);
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.Settings_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSSave;
        private System.Windows.Forms.Button btnSCan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edtPath;
        private System.Windows.Forms.TextBox edtTimer;
        private System.Windows.Forms.CheckBox chkHTML;
        private System.Windows.Forms.CheckBox chkSave;
        private System.Windows.Forms.CheckBox chkTray;
        private System.Windows.Forms.CheckBox chkWarn;
        private System.Windows.Forms.Button setPathButton;
        private System.Windows.Forms.CheckBox chkStartWithWindows;
    }
}