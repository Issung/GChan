namespace GChan {
    partial class SettingsForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkSaveHtml = new System.Windows.Forms.CheckBox();
            this.chkSave = new System.Windows.Forms.CheckBox();
            this.chkTray = new System.Windows.Forms.CheckBox();
            this.chkWarn = new System.Windows.Forms.CheckBox();
            this.setPathButton = new System.Windows.Forms.Button();
            this.chkStartWithWindows = new System.Windows.Forms.CheckBox();
            this.timerNumeric = new System.Windows.Forms.NumericUpDown();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.renameThreadFolderCheckBox = new System.Windows.Forms.CheckBox();
            this.chkStartWithWindowsMinimized = new System.Windows.Forms.CheckBox();
            this.concurrentDownloadsLabel = new System.Windows.Forms.Label();
            this.chkSaveThumbnails = new System.Windows.Forms.CheckBox();
            this.imageFilenameFormatLabel = new System.Windows.Forms.Label();
            this.imageFilenameFormatComboBox = new System.Windows.Forms.ComboBox();
            this.addUrlFromClipboardWhenTextboxEmpty = new System.Windows.Forms.CheckBox();
            this.threadFolderNameFormatLabel = new System.Windows.Forms.Label();
            this.threadFolderNameFormatComboBox = new System.Windows.Forms.ComboBox();
            this.checkForUpdatesOnStartCheckBox = new System.Windows.Forms.CheckBox();
            this.concurrentDownloadsNumeric = new System.Windows.Forms.NumericUpDown();
            this.userAgentLabel = new System.Windows.Forms.Label();
            this.userAgentTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.timerNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.concurrentDownloadsNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSave.Location = new System.Drawing.Point(73, 407);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(92, 23);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.Location = new System.Drawing.Point(171, 407);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(94, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Path";
            this.tooltip.SetToolTip(this.label1, "The path for scraped images, videos & HTML to be downloaded into.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Timer (sec)";
            this.tooltip.SetToolTip(this.label2, "How often to check threads/boards for updates.");
            // 
            // chkSaveHtml
            // 
            this.chkSaveHtml.AutoSize = true;
            this.chkSaveHtml.Location = new System.Drawing.Point(11, 197);
            this.chkSaveHtml.Name = "chkSaveHtml";
            this.chkSaveHtml.Size = new System.Drawing.Size(144, 17);
            this.chkSaveHtml.TabIndex = 6;
            this.chkSaveHtml.Text = "Download Thread HTML";
            this.tooltip.SetToolTip(this.chkSaveHtml, "Routinely save the HTML of each scraped thread.");
            this.chkSaveHtml.UseVisualStyleBackColor = true;
            this.chkSaveHtml.CheckedChanged += new System.EventHandler(this.chkHTML_CheckedChanged);
            // 
            // chkSave
            // 
            this.chkSave.AutoSize = true;
            this.chkSave.Location = new System.Drawing.Point(11, 261);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(115, 17);
            this.chkSave.TabIndex = 7;
            this.chkSave.Text = "Save URLs on exit";
            this.chkSave.UseVisualStyleBackColor = true;
            // 
            // chkTray
            // 
            this.chkTray.AutoSize = true;
            this.chkTray.Location = new System.Drawing.Point(11, 284);
            this.chkTray.Name = "chkTray";
            this.chkTray.Size = new System.Drawing.Size(102, 17);
            this.chkTray.TabIndex = 8;
            this.chkTray.Text = "Minimize to Tray";
            this.tooltip.SetToolTip(this.chkTray, "Minimize GChan to the system tray when minimized.");
            this.chkTray.UseVisualStyleBackColor = true;
            this.chkTray.CheckedChanged += new System.EventHandler(this.chkTray_CheckedChanged);
            // 
            // chkWarn
            // 
            this.chkWarn.AutoSize = true;
            this.chkWarn.Location = new System.Drawing.Point(11, 239);
            this.chkWarn.Name = "chkWarn";
            this.chkWarn.Size = new System.Drawing.Size(116, 17);
            this.chkWarn.TabIndex = 9;
            this.chkWarn.Text = "Show Exit Warning";
            this.chkWarn.UseVisualStyleBackColor = true;
            // 
            // setPathButton
            // 
            this.setPathButton.Location = new System.Drawing.Point(291, 42);
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
            this.chkStartWithWindows.Location = new System.Drawing.Point(11, 308);
            this.chkStartWithWindows.Name = "chkStartWithWindows";
            this.chkStartWithWindows.Size = new System.Drawing.Size(153, 17);
            this.chkStartWithWindows.TabIndex = 11;
            this.chkStartWithWindows.Text = "Start GChan with Windows";
            this.chkStartWithWindows.UseVisualStyleBackColor = true;
            this.chkStartWithWindows.CheckedChanged += new System.EventHandler(this.chkStartWithWindows_CheckedChanged);
            // 
            // timerNumeric
            // 
            this.timerNumeric.Location = new System.Drawing.Point(73, 70);
            this.timerNumeric.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.timerNumeric.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.timerNumeric.Name = "timerNumeric";
            this.timerNumeric.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.timerNumeric.Size = new System.Drawing.Size(242, 20);
            this.timerNumeric.TabIndex = 12;
            this.timerNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(73, 44);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(211, 20);
            this.directoryTextBox.TabIndex = 13;
            this.tooltip.SetToolTip(this.directoryTextBox, "The directory that GChan will save files in. Double click to open or go File->Ope" +
        "n Folder in GChan\'s main window.");
            this.directoryTextBox.DoubleClick += new System.EventHandler(this.textBox1_DoubleClick);
            // 
            // renameThreadFolderCheckBox
            // 
            this.renameThreadFolderCheckBox.AutoSize = true;
            this.renameThreadFolderCheckBox.Location = new System.Drawing.Point(11, 146);
            this.renameThreadFolderCheckBox.Name = "renameThreadFolderCheckBox";
            this.renameThreadFolderCheckBox.Size = new System.Drawing.Size(201, 17);
            this.renameThreadFolderCheckBox.TabIndex = 14;
            this.renameThreadFolderCheckBox.Text = "Rename thread folder when removed";
            this.tooltip.SetToolTip(this.renameThreadFolderCheckBox, resources.GetString("renameThreadFolderCheckBox.ToolTip"));
            this.renameThreadFolderCheckBox.UseVisualStyleBackColor = true;
            this.renameThreadFolderCheckBox.CheckedChanged += new System.EventHandler(this.renameThreadFolderCheckBox_CheckedChanged);
            // 
            // chkStartWithWindowsMinimized
            // 
            this.chkStartWithWindowsMinimized.AutoSize = true;
            this.chkStartWithWindowsMinimized.Location = new System.Drawing.Point(21, 331);
            this.chkStartWithWindowsMinimized.Name = "chkStartWithWindowsMinimized";
            this.chkStartWithWindowsMinimized.Size = new System.Drawing.Size(149, 17);
            this.chkStartWithWindowsMinimized.TabIndex = 20;
            this.chkStartWithWindowsMinimized.Text = "Start hidden in system tray";
            this.tooltip.SetToolTip(this.chkStartWithWindowsMinimized, "When starting with windows start minimized to the system tray.\r\nTo use this optio" +
        "n you must enable both \"Start GCHan with Windows\" and \"Minimize to Tray\" options" +
        ".");
            this.chkStartWithWindowsMinimized.UseVisualStyleBackColor = true;
            // 
            // concurrentDownloadsLabel
            // 
            this.concurrentDownloadsLabel.AutoSize = true;
            this.concurrentDownloadsLabel.Location = new System.Drawing.Point(8, 99);
            this.concurrentDownloadsLabel.Name = "concurrentDownloadsLabel";
            this.concurrentDownloadsLabel.Size = new System.Drawing.Size(115, 13);
            this.concurrentDownloadsLabel.TabIndex = 22;
            this.concurrentDownloadsLabel.Text = "Concurrent Downloads";
            this.tooltip.SetToolTip(this.concurrentDownloadsLabel, resources.GetString("concurrentDownloadsLabel.ToolTip"));
            // 
            // chkSaveThumbnails
            // 
            this.chkSaveThumbnails.AutoSize = true;
            this.chkSaveThumbnails.Location = new System.Drawing.Point(21, 218);
            this.chkSaveThumbnails.Name = "chkSaveThumbnails";
            this.chkSaveThumbnails.Size = new System.Drawing.Size(131, 17);
            this.chkSaveThumbnails.TabIndex = 24;
            this.chkSaveThumbnails.Text = "Download Thumbnails";
            this.tooltip.SetToolTip(this.chkSaveThumbnails, "When downloading thread HTML, also save thumbnails for images and place in \"thumb" +
        "\" folder.");
            this.chkSaveThumbnails.UseVisualStyleBackColor = true;
            // 
            // imageFilenameFormatLabel
            // 
            this.imageFilenameFormatLabel.AutoSize = true;
            this.imageFilenameFormatLabel.Location = new System.Drawing.Point(8, 122);
            this.imageFilenameFormatLabel.Name = "imageFilenameFormatLabel";
            this.imageFilenameFormatLabel.Size = new System.Drawing.Size(116, 13);
            this.imageFilenameFormatLabel.TabIndex = 15;
            this.imageFilenameFormatLabel.Text = "Image Filename Format";
            // 
            // imageFilenameFormatComboBox
            // 
            this.imageFilenameFormatComboBox.DisplayMember = "EnumDescription";
            this.imageFilenameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageFilenameFormatComboBox.DropDownWidth = 300;
            this.imageFilenameFormatComboBox.Location = new System.Drawing.Point(130, 119);
            this.imageFilenameFormatComboBox.Name = "imageFilenameFormatComboBox";
            this.imageFilenameFormatComboBox.Size = new System.Drawing.Size(185, 21);
            this.imageFilenameFormatComboBox.TabIndex = 16;
            this.imageFilenameFormatComboBox.ValueMember = "EnumValue";
            // 
            // addUrlFromClipboardWhenTextboxEmpty
            // 
            this.addUrlFromClipboardWhenTextboxEmpty.AutoSize = true;
            this.addUrlFromClipboardWhenTextboxEmpty.Location = new System.Drawing.Point(11, 354);
            this.addUrlFromClipboardWhenTextboxEmpty.Name = "addUrlFromClipboardWhenTextboxEmpty";
            this.addUrlFromClipboardWhenTextboxEmpty.Size = new System.Drawing.Size(225, 17);
            this.addUrlFromClipboardWhenTextboxEmpty.TabIndex = 17;
            this.addUrlFromClipboardWhenTextboxEmpty.Text = "Add URL from clipboard if textbox is empty";
            this.addUrlFromClipboardWhenTextboxEmpty.UseVisualStyleBackColor = true;
            // 
            // threadFolderNameFormatLabel
            // 
            this.threadFolderNameFormatLabel.AutoSize = true;
            this.threadFolderNameFormatLabel.Location = new System.Drawing.Point(18, 170);
            this.threadFolderNameFormatLabel.Name = "threadFolderNameFormatLabel";
            this.threadFolderNameFormatLabel.Size = new System.Drawing.Size(139, 13);
            this.threadFolderNameFormatLabel.TabIndex = 18;
            this.threadFolderNameFormatLabel.Text = "Thread Folder Name Format";
            // 
            // threadFolderNameFormatComboBox
            // 
            this.threadFolderNameFormatComboBox.DisplayMember = "EnumDescription";
            this.threadFolderNameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.threadFolderNameFormatComboBox.FormattingEnabled = true;
            this.threadFolderNameFormatComboBox.Location = new System.Drawing.Point(163, 168);
            this.threadFolderNameFormatComboBox.Name = "threadFolderNameFormatComboBox";
            this.threadFolderNameFormatComboBox.Size = new System.Drawing.Size(152, 21);
            this.threadFolderNameFormatComboBox.TabIndex = 19;
            this.threadFolderNameFormatComboBox.ValueMember = "EnumValue";
            // 
            // checkForUpdatesOnStartCheckBox
            // 
            this.checkForUpdatesOnStartCheckBox.AutoSize = true;
            this.checkForUpdatesOnStartCheckBox.Location = new System.Drawing.Point(11, 377);
            this.checkForUpdatesOnStartCheckBox.Name = "checkForUpdatesOnStartCheckBox";
            this.checkForUpdatesOnStartCheckBox.Size = new System.Drawing.Size(206, 17);
            this.checkForUpdatesOnStartCheckBox.TabIndex = 21;
            this.checkForUpdatesOnStartCheckBox.Text = "Check for updates when GChan starts";
            this.checkForUpdatesOnStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // concurrentDownloadsNumeric
            // 
            this.concurrentDownloadsNumeric.Location = new System.Drawing.Point(130, 95);
            this.concurrentDownloadsNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.concurrentDownloadsNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.concurrentDownloadsNumeric.Name = "concurrentDownloadsNumeric";
            this.concurrentDownloadsNumeric.Size = new System.Drawing.Size(185, 20);
            this.concurrentDownloadsNumeric.TabIndex = 23;
            this.concurrentDownloadsNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // userAgentLabel
            // 
            this.userAgentLabel.AutoSize = true;
            this.userAgentLabel.Location = new System.Drawing.Point(8, 20);
            this.userAgentLabel.Name = "userAgentLabel";
            this.userAgentLabel.Size = new System.Drawing.Size(60, 13);
            this.userAgentLabel.TabIndex = 25;
            this.userAgentLabel.Text = "User-Agent";
            this.tooltip.SetToolTip(this.userAgentLabel, resources.GetString("userAgentLabel.ToolTip"));
            // 
            // userAgentTextBox
            // 
            this.userAgentTextBox.Location = new System.Drawing.Point(73, 16);
            this.userAgentTextBox.Name = "userAgentTextBox";
            this.userAgentTextBox.Size = new System.Drawing.Size(241, 20);
            this.userAgentTextBox.TabIndex = 26;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(324, 442);
            this.ControlBox = false;
            this.Controls.Add(this.userAgentTextBox);
            this.Controls.Add(this.userAgentLabel);
            this.Controls.Add(this.chkSaveThumbnails);
            this.Controls.Add(this.concurrentDownloadsNumeric);
            this.Controls.Add(this.concurrentDownloadsLabel);
            this.Controls.Add(this.checkForUpdatesOnStartCheckBox);
            this.Controls.Add(this.chkStartWithWindowsMinimized);
            this.Controls.Add(this.threadFolderNameFormatComboBox);
            this.Controls.Add(this.threadFolderNameFormatLabel);
            this.Controls.Add(this.addUrlFromClipboardWhenTextboxEmpty);
            this.Controls.Add(this.imageFilenameFormatComboBox);
            this.Controls.Add(this.imageFilenameFormatLabel);
            this.Controls.Add(this.renameThreadFolderCheckBox);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.timerNumeric);
            this.Controls.Add(this.chkStartWithWindows);
            this.Controls.Add(this.setPathButton);
            this.Controls.Add(this.chkSaveHtml);
            this.Controls.Add(this.chkWarn);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.chkTray);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.chkSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.Settings_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.timerNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.concurrentDownloadsNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSaveHtml;
        private System.Windows.Forms.CheckBox chkSave;
        private System.Windows.Forms.CheckBox chkTray;
        private System.Windows.Forms.CheckBox chkWarn;
        private System.Windows.Forms.Button setPathButton;
        private System.Windows.Forms.CheckBox chkStartWithWindows;
        private System.Windows.Forms.NumericUpDown timerNumeric;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.CheckBox renameThreadFolderCheckBox;
        private System.Windows.Forms.Label imageFilenameFormatLabel;
        private System.Windows.Forms.ComboBox imageFilenameFormatComboBox;
        private System.Windows.Forms.CheckBox addUrlFromClipboardWhenTextboxEmpty;
        private System.Windows.Forms.Label threadFolderNameFormatLabel;
        private System.Windows.Forms.ComboBox threadFolderNameFormatComboBox;
        private System.Windows.Forms.CheckBox chkStartWithWindowsMinimized;
        private System.Windows.Forms.CheckBox checkForUpdatesOnStartCheckBox;
        private System.Windows.Forms.Label concurrentDownloadsLabel;
        private System.Windows.Forms.NumericUpDown concurrentDownloadsNumeric;
        private System.Windows.Forms.CheckBox chkSaveThumbnails;
        private System.Windows.Forms.Label userAgentLabel;
        private System.Windows.Forms.TextBox userAgentTextBox;
    }
}