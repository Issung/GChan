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
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            buttonSave = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            chkSaveHtml = new System.Windows.Forms.CheckBox();
            chkSave = new System.Windows.Forms.CheckBox();
            chkTray = new System.Windows.Forms.CheckBox();
            chkWarn = new System.Windows.Forms.CheckBox();
            setPathButton = new System.Windows.Forms.Button();
            chkStartWithWindows = new System.Windows.Forms.CheckBox();
            timerNumeric = new System.Windows.Forms.NumericUpDown();
            directoryTextBox = new System.Windows.Forms.TextBox();
            tooltip = new System.Windows.Forms.ToolTip(components);
            renameThreadFolderCheckBox = new System.Windows.Forms.CheckBox();
            chkStartWithWindowsMinimized = new System.Windows.Forms.CheckBox();
            concurrentDownloadsLabel = new System.Windows.Forms.Label();
            chkSaveThumbnails = new System.Windows.Forms.CheckBox();
            userAgentLabel = new System.Windows.Forms.Label();
            max1RequestPerSecondCheckBox = new System.Windows.Forms.CheckBox();
            imageFilenameFormatLabel = new System.Windows.Forms.Label();
            imageFilenameFormatComboBox = new System.Windows.Forms.ComboBox();
            addUrlFromClipboardWhenTextboxEmpty = new System.Windows.Forms.CheckBox();
            threadFolderNameFormatLabel = new System.Windows.Forms.Label();
            threadFolderNameFormatComboBox = new System.Windows.Forms.ComboBox();
            checkForUpdatesOnStartCheckBox = new System.Windows.Forms.CheckBox();
            concurrentDownloadsNumeric = new System.Windows.Forms.NumericUpDown();
            userAgentTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)timerNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)concurrentDownloadsNumeric).BeginInit();
            SuspendLayout();
            // 
            // buttonSave
            // 
            buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            buttonSave.Location = new System.Drawing.Point(75, 504);
            buttonSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(107, 27);
            buttonSave.TabIndex = 0;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            buttonCancel.Location = new System.Drawing.Point(190, 504);
            buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(110, 27);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 54);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(31, 15);
            label1.TabIndex = 2;
            label1.Text = "Path";
            tooltip.SetToolTip(label1, "The path for scraped images, videos & HTML to be downloaded into.");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(9, 84);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 15);
            label2.TabIndex = 3;
            label2.Text = "Timer (sec)";
            tooltip.SetToolTip(label2, "How often to check threads/boards for updates.");
            // 
            // chkSaveHtml
            // 
            chkSaveHtml.AutoSize = true;
            chkSaveHtml.Location = new System.Drawing.Point(13, 257);
            chkSaveHtml.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkSaveHtml.Name = "chkSaveHtml";
            chkSaveHtml.Size = new System.Drawing.Size(154, 19);
            chkSaveHtml.TabIndex = 6;
            chkSaveHtml.Text = "Download Thread HTML";
            tooltip.SetToolTip(chkSaveHtml, "Routinely save the HTML of each scraped thread.");
            chkSaveHtml.UseVisualStyleBackColor = true;
            chkSaveHtml.CheckedChanged += chkHTML_CheckedChanged;
            // 
            // chkSave
            // 
            chkSave.AutoSize = true;
            chkSave.Location = new System.Drawing.Point(13, 331);
            chkSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkSave.Name = "chkSave";
            chkSave.Size = new System.Drawing.Size(118, 19);
            chkSave.TabIndex = 7;
            chkSave.Text = "Save URLs on exit";
            chkSave.UseVisualStyleBackColor = true;
            // 
            // chkTray
            // 
            chkTray.AutoSize = true;
            chkTray.Location = new System.Drawing.Point(13, 358);
            chkTray.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkTray.Name = "chkTray";
            chkTray.Size = new System.Drawing.Size(113, 19);
            chkTray.TabIndex = 8;
            chkTray.Text = "Minimize to Tray";
            tooltip.SetToolTip(chkTray, "Minimize GChan to the system tray when minimized.");
            chkTray.UseVisualStyleBackColor = true;
            chkTray.CheckedChanged += chkTray_CheckedChanged;
            // 
            // chkWarn
            // 
            chkWarn.AutoSize = true;
            chkWarn.Location = new System.Drawing.Point(13, 306);
            chkWarn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkWarn.Name = "chkWarn";
            chkWarn.Size = new System.Drawing.Size(125, 19);
            chkWarn.TabIndex = 9;
            chkWarn.Text = "Show Exit Warning";
            chkWarn.UseVisualStyleBackColor = true;
            // 
            // setPathButton
            // 
            setPathButton.Location = new System.Drawing.Point(340, 48);
            setPathButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            setPathButton.Name = "setPathButton";
            setPathButton.Size = new System.Drawing.Size(29, 25);
            setPathButton.TabIndex = 10;
            setPathButton.Text = "...";
            setPathButton.UseVisualStyleBackColor = true;
            setPathButton.Click += SetPathButton_Click;
            // 
            // chkStartWithWindows
            // 
            chkStartWithWindows.AutoSize = true;
            chkStartWithWindows.Location = new System.Drawing.Point(13, 385);
            chkStartWithWindows.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkStartWithWindows.Name = "chkStartWithWindows";
            chkStartWithWindows.Size = new System.Drawing.Size(167, 19);
            chkStartWithWindows.TabIndex = 11;
            chkStartWithWindows.Text = "Start GChan with Windows";
            chkStartWithWindows.UseVisualStyleBackColor = true;
            chkStartWithWindows.CheckedChanged += chkStartWithWindows_CheckedChanged;
            // 
            // timerNumeric
            // 
            timerNumeric.Location = new System.Drawing.Point(85, 81);
            timerNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            timerNumeric.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            timerNumeric.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            timerNumeric.Name = "timerNumeric";
            timerNumeric.RightToLeft = System.Windows.Forms.RightToLeft.No;
            timerNumeric.Size = new System.Drawing.Size(282, 23);
            timerNumeric.TabIndex = 12;
            timerNumeric.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // directoryTextBox
            // 
            directoryTextBox.Location = new System.Drawing.Point(85, 51);
            directoryTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            directoryTextBox.Name = "directoryTextBox";
            directoryTextBox.ReadOnly = true;
            directoryTextBox.Size = new System.Drawing.Size(245, 23);
            directoryTextBox.TabIndex = 13;
            tooltip.SetToolTip(directoryTextBox, "The directory that GChan will save files in. Double click to open or go File->Open Folder in GChan's main window.");
            directoryTextBox.DoubleClick += textBox1_DoubleClick;
            // 
            // renameThreadFolderCheckBox
            // 
            renameThreadFolderCheckBox.AutoSize = true;
            renameThreadFolderCheckBox.Location = new System.Drawing.Point(13, 198);
            renameThreadFolderCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            renameThreadFolderCheckBox.Name = "renameThreadFolderCheckBox";
            renameThreadFolderCheckBox.Size = new System.Drawing.Size(222, 19);
            renameThreadFolderCheckBox.TabIndex = 14;
            renameThreadFolderCheckBox.Text = "Rename thread folder when removed";
            tooltip.SetToolTip(renameThreadFolderCheckBox, resources.GetString("renameThreadFolderCheckBox.ToolTip"));
            renameThreadFolderCheckBox.UseVisualStyleBackColor = true;
            renameThreadFolderCheckBox.CheckedChanged += renameThreadFolderCheckBox_CheckedChanged;
            // 
            // chkStartWithWindowsMinimized
            // 
            chkStartWithWindowsMinimized.AutoSize = true;
            chkStartWithWindowsMinimized.Location = new System.Drawing.Point(24, 412);
            chkStartWithWindowsMinimized.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkStartWithWindowsMinimized.Name = "chkStartWithWindowsMinimized";
            chkStartWithWindowsMinimized.Size = new System.Drawing.Size(166, 19);
            chkStartWithWindowsMinimized.TabIndex = 20;
            chkStartWithWindowsMinimized.Text = "Start hidden in system tray";
            tooltip.SetToolTip(chkStartWithWindowsMinimized, "When starting with windows start minimized to the system tray.\r\nTo use this option you must enable both \"Start GCHan with Windows\" and \"Minimize to Tray\" options.");
            chkStartWithWindowsMinimized.UseVisualStyleBackColor = true;
            // 
            // concurrentDownloadsLabel
            // 
            concurrentDownloadsLabel.AutoSize = true;
            concurrentDownloadsLabel.Location = new System.Drawing.Point(9, 114);
            concurrentDownloadsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            concurrentDownloadsLabel.Name = "concurrentDownloadsLabel";
            concurrentDownloadsLabel.Size = new System.Drawing.Size(129, 15);
            concurrentDownloadsLabel.TabIndex = 22;
            concurrentDownloadsLabel.Text = "Concurrent Downloads";
            tooltip.SetToolTip(concurrentDownloadsLabel, resources.GetString("concurrentDownloadsLabel.ToolTip"));
            // 
            // chkSaveThumbnails
            // 
            chkSaveThumbnails.AutoSize = true;
            chkSaveThumbnails.Location = new System.Drawing.Point(24, 282);
            chkSaveThumbnails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkSaveThumbnails.Name = "chkSaveThumbnails";
            chkSaveThumbnails.Size = new System.Drawing.Size(145, 19);
            chkSaveThumbnails.TabIndex = 24;
            chkSaveThumbnails.Text = "Download Thumbnails";
            tooltip.SetToolTip(chkSaveThumbnails, "When downloading thread HTML, also save thumbnails for images and place in \"thumb\" folder.");
            chkSaveThumbnails.UseVisualStyleBackColor = true;
            // 
            // userAgentLabel
            // 
            userAgentLabel.AutoSize = true;
            userAgentLabel.Location = new System.Drawing.Point(9, 23);
            userAgentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            userAgentLabel.Name = "userAgentLabel";
            userAgentLabel.Size = new System.Drawing.Size(67, 15);
            userAgentLabel.TabIndex = 25;
            userAgentLabel.Text = "User-Agent";
            tooltip.SetToolTip(userAgentLabel, resources.GetString("userAgentLabel.ToolTip"));
            // 
            // max1RequestPerSecondCheckBox
            // 
            max1RequestPerSecondCheckBox.AutoSize = true;
            max1RequestPerSecondCheckBox.Location = new System.Drawing.Point(13, 141);
            max1RequestPerSecondCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            max1RequestPerSecondCheckBox.Name = "max1RequestPerSecondCheckBox";
            max1RequestPerSecondCheckBox.Size = new System.Drawing.Size(259, 19);
            max1RequestPerSecondCheckBox.TabIndex = 27;
            max1RequestPerSecondCheckBox.Text = "Limit Requests to maximum of 1 per second";
            tooltip.SetToolTip(max1RequestPerSecondCheckBox, "Allow a maximum of 1 request per second to 4chan, this is highly recommended to avoid getting rate limited or banned.");
            max1RequestPerSecondCheckBox.UseVisualStyleBackColor = true;
            // 
            // imageFilenameFormatLabel
            // 
            imageFilenameFormatLabel.AutoSize = true;
            imageFilenameFormatLabel.Location = new System.Drawing.Point(9, 171);
            imageFilenameFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            imageFilenameFormatLabel.Name = "imageFilenameFormatLabel";
            imageFilenameFormatLabel.Size = new System.Drawing.Size(132, 15);
            imageFilenameFormatLabel.TabIndex = 15;
            imageFilenameFormatLabel.Text = "Image Filename Format";
            // 
            // imageFilenameFormatComboBox
            // 
            imageFilenameFormatComboBox.DisplayMember = "EnumDescription";
            imageFilenameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            imageFilenameFormatComboBox.DropDownWidth = 300;
            imageFilenameFormatComboBox.Location = new System.Drawing.Point(152, 167);
            imageFilenameFormatComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            imageFilenameFormatComboBox.Name = "imageFilenameFormatComboBox";
            imageFilenameFormatComboBox.Size = new System.Drawing.Size(215, 23);
            imageFilenameFormatComboBox.TabIndex = 16;
            imageFilenameFormatComboBox.ValueMember = "EnumValue";
            // 
            // addUrlFromClipboardWhenTextboxEmpty
            // 
            addUrlFromClipboardWhenTextboxEmpty.AutoSize = true;
            addUrlFromClipboardWhenTextboxEmpty.Location = new System.Drawing.Point(13, 438);
            addUrlFromClipboardWhenTextboxEmpty.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            addUrlFromClipboardWhenTextboxEmpty.Name = "addUrlFromClipboardWhenTextboxEmpty";
            addUrlFromClipboardWhenTextboxEmpty.Size = new System.Drawing.Size(255, 19);
            addUrlFromClipboardWhenTextboxEmpty.TabIndex = 17;
            addUrlFromClipboardWhenTextboxEmpty.Text = "Add URL from clipboard if textbox is empty";
            addUrlFromClipboardWhenTextboxEmpty.UseVisualStyleBackColor = true;
            // 
            // threadFolderNameFormatLabel
            // 
            threadFolderNameFormatLabel.AutoSize = true;
            threadFolderNameFormatLabel.Location = new System.Drawing.Point(21, 226);
            threadFolderNameFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            threadFolderNameFormatLabel.Name = "threadFolderNameFormatLabel";
            threadFolderNameFormatLabel.Size = new System.Drawing.Size(155, 15);
            threadFolderNameFormatLabel.TabIndex = 18;
            threadFolderNameFormatLabel.Text = "Thread Folder Name Format";
            // 
            // threadFolderNameFormatComboBox
            // 
            threadFolderNameFormatComboBox.DisplayMember = "EnumDescription";
            threadFolderNameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            threadFolderNameFormatComboBox.FormattingEnabled = true;
            threadFolderNameFormatComboBox.Location = new System.Drawing.Point(190, 224);
            threadFolderNameFormatComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            threadFolderNameFormatComboBox.Name = "threadFolderNameFormatComboBox";
            threadFolderNameFormatComboBox.Size = new System.Drawing.Size(177, 23);
            threadFolderNameFormatComboBox.TabIndex = 19;
            threadFolderNameFormatComboBox.ValueMember = "EnumValue";
            // 
            // checkForUpdatesOnStartCheckBox
            // 
            checkForUpdatesOnStartCheckBox.AutoSize = true;
            checkForUpdatesOnStartCheckBox.Location = new System.Drawing.Point(13, 465);
            checkForUpdatesOnStartCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkForUpdatesOnStartCheckBox.Name = "checkForUpdatesOnStartCheckBox";
            checkForUpdatesOnStartCheckBox.Size = new System.Drawing.Size(224, 19);
            checkForUpdatesOnStartCheckBox.TabIndex = 21;
            checkForUpdatesOnStartCheckBox.Text = "Check for updates when GChan starts";
            checkForUpdatesOnStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // concurrentDownloadsNumeric
            // 
            concurrentDownloadsNumeric.Location = new System.Drawing.Point(152, 110);
            concurrentDownloadsNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            concurrentDownloadsNumeric.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            concurrentDownloadsNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            concurrentDownloadsNumeric.Name = "concurrentDownloadsNumeric";
            concurrentDownloadsNumeric.Size = new System.Drawing.Size(216, 23);
            concurrentDownloadsNumeric.TabIndex = 23;
            concurrentDownloadsNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // userAgentTextBox
            // 
            userAgentTextBox.Location = new System.Drawing.Point(85, 18);
            userAgentTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            userAgentTextBox.Name = "userAgentTextBox";
            userAgentTextBox.Size = new System.Drawing.Size(280, 23);
            userAgentTextBox.TabIndex = 26;
            // 
            // SettingsForm
            // 
            AcceptButton = buttonSave;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(379, 543);
            ControlBox = false;
            Controls.Add(max1RequestPerSecondCheckBox);
            Controls.Add(userAgentTextBox);
            Controls.Add(userAgentLabel);
            Controls.Add(chkSaveThumbnails);
            Controls.Add(concurrentDownloadsNumeric);
            Controls.Add(concurrentDownloadsLabel);
            Controls.Add(checkForUpdatesOnStartCheckBox);
            Controls.Add(chkStartWithWindowsMinimized);
            Controls.Add(threadFolderNameFormatComboBox);
            Controls.Add(threadFolderNameFormatLabel);
            Controls.Add(addUrlFromClipboardWhenTextboxEmpty);
            Controls.Add(imageFilenameFormatComboBox);
            Controls.Add(imageFilenameFormatLabel);
            Controls.Add(renameThreadFolderCheckBox);
            Controls.Add(directoryTextBox);
            Controls.Add(timerNumeric);
            Controls.Add(chkStartWithWindows);
            Controls.Add(setPathButton);
            Controls.Add(chkSaveHtml);
            Controls.Add(chkWarn);
            Controls.Add(buttonSave);
            Controls.Add(chkTray);
            Controls.Add(buttonCancel);
            Controls.Add(chkSave);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Settings";
            Shown += Settings_Shown;
            ((System.ComponentModel.ISupportInitialize)timerNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)concurrentDownloadsNumeric).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.CheckBox max1RequestPerSecondCheckBox;
    }
}