namespace GChan
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            btnSSave = new Button();
            btnSCan = new Button();
            label1 = new Label();
            label2 = new Label();
            chkHTML = new CheckBox();
            chkSave = new CheckBox();
            chkTray = new CheckBox();
            chkWarn = new CheckBox();
            setPathButton = new Button();
            chkStartWithWindows = new CheckBox();
            timerNumeric = new NumericUpDown();
            directoryTextBox = new TextBox();
            tooltip = new ToolTip(components);
            renameThreadFolderCheckBox = new CheckBox();
            chkStartWithWindowsMinimized = new CheckBox();
            imageFilenameFormatLabel = new Label();
            imageFilenameFormatComboBox = new ComboBox();
            addUrlFromClipboardWhenTextboxEmpty = new CheckBox();
            threadFolderNameFormatLabel = new Label();
            threadFolderNameFormatComboBox = new ComboBox();
            checkForUpdatesOnStartCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)timerNumeric).BeginInit();
            SuspendLayout();
            // 
            // btnSSave
            // 
            btnSSave.Anchor = AnchorStyles.Bottom;
            btnSSave.Location = new Point(74, 382);
            btnSSave.Margin = new Padding(4, 3, 4, 3);
            btnSSave.Name = "btnSSave";
            btnSSave.Size = new Size(106, 26);
            btnSSave.TabIndex = 0;
            btnSSave.Text = "Save";
            btnSSave.UseVisualStyleBackColor = true;
            btnSSave.Click += btnSSave_Click;
            // 
            // btnSCan
            // 
            btnSCan.Anchor = AnchorStyles.Bottom;
            btnSCan.Location = new Point(188, 382);
            btnSCan.Margin = new Padding(4, 3, 4, 3);
            btnSCan.Name = "btnSCan";
            btnSCan.Size = new Size(109, 26);
            btnSCan.TabIndex = 1;
            btnSCan.Text = "Cancel";
            btnSCan.UseVisualStyleBackColor = true;
            btnSCan.Click += btnSCan_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 14);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 2;
            label1.Text = "Path";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 44);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(65, 15);
            label2.TabIndex = 3;
            label2.Text = "Timer (sec)";
            // 
            // chkHTML
            // 
            chkHTML.AutoSize = true;
            chkHTML.Location = new Point(18, 163);
            chkHTML.Margin = new Padding(4, 3, 4, 3);
            chkHTML.Name = "chkHTML";
            chkHTML.Size = new Size(115, 19);
            chkHTML.TabIndex = 6;
            chkHTML.Text = "Download HTML";
            chkHTML.UseVisualStyleBackColor = true;
            // 
            // chkSave
            // 
            chkSave.AutoSize = true;
            chkSave.Location = new Point(18, 215);
            chkSave.Margin = new Padding(4, 3, 4, 3);
            chkSave.Name = "chkSave";
            chkSave.Size = new Size(118, 19);
            chkSave.TabIndex = 7;
            chkSave.Text = "Save URLs on exit";
            chkSave.UseVisualStyleBackColor = true;
            // 
            // chkTray
            // 
            chkTray.AutoSize = true;
            chkTray.Location = new Point(18, 241);
            chkTray.Margin = new Padding(4, 3, 4, 3);
            chkTray.Name = "chkTray";
            chkTray.Size = new Size(113, 19);
            chkTray.TabIndex = 8;
            chkTray.Text = "Minimize to Tray";
            tooltip.SetToolTip(chkTray, "Minimize GChan to the system tray when minimized.");
            chkTray.UseVisualStyleBackColor = true;
            chkTray.CheckedChanged += chkTray_CheckedChanged;
            // 
            // chkWarn
            // 
            chkWarn.AutoSize = true;
            chkWarn.Location = new Point(18, 189);
            chkWarn.Margin = new Padding(4, 3, 4, 3);
            chkWarn.Name = "chkWarn";
            chkWarn.Size = new Size(125, 19);
            chkWarn.TabIndex = 9;
            chkWarn.Text = "Show Exit Warning";
            chkWarn.UseVisualStyleBackColor = true;
            // 
            // setPathButton
            // 
            setPathButton.Location = new Point(344, 8);
            setPathButton.Margin = new Padding(4, 3, 4, 3);
            setPathButton.Name = "setPathButton";
            setPathButton.Size = new Size(29, 25);
            setPathButton.TabIndex = 10;
            setPathButton.Text = "...";
            setPathButton.UseVisualStyleBackColor = true;
            setPathButton.Click += SetPathButton_Click;
            // 
            // chkStartWithWindows
            // 
            chkStartWithWindows.AutoSize = true;
            chkStartWithWindows.Location = new Point(18, 269);
            chkStartWithWindows.Margin = new Padding(4, 3, 4, 3);
            chkStartWithWindows.Name = "chkStartWithWindows";
            chkStartWithWindows.Size = new Size(167, 19);
            chkStartWithWindows.TabIndex = 11;
            chkStartWithWindows.Text = "Start GChan with Windows";
            chkStartWithWindows.UseVisualStyleBackColor = true;
            chkStartWithWindows.CheckedChanged += chkStartWithWindows_CheckedChanged;
            // 
            // timerNumeric
            // 
            timerNumeric.Location = new Point(90, 40);
            timerNumeric.Margin = new Padding(4, 3, 4, 3);
            timerNumeric.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            timerNumeric.Name = "timerNumeric";
            timerNumeric.Size = new Size(282, 23);
            timerNumeric.TabIndex = 12;
            // 
            // directoryTextBox
            // 
            directoryTextBox.Location = new Point(90, 10);
            directoryTextBox.Margin = new Padding(4, 3, 4, 3);
            directoryTextBox.Name = "directoryTextBox";
            directoryTextBox.ReadOnly = true;
            directoryTextBox.Size = new Size(245, 23);
            directoryTextBox.TabIndex = 13;
            tooltip.SetToolTip(directoryTextBox, "The directory that GChan will save files in. Double click to open or go File->Open Folder in GChan's main window.");
            directoryTextBox.DoubleClick += textBox1_DoubleClick;
            // 
            // renameThreadFolderCheckBox
            // 
            renameThreadFolderCheckBox.AutoSize = true;
            renameThreadFolderCheckBox.Location = new Point(18, 104);
            renameThreadFolderCheckBox.Margin = new Padding(4, 3, 4, 3);
            renameThreadFolderCheckBox.Name = "renameThreadFolderCheckBox";
            renameThreadFolderCheckBox.Size = new Size(222, 19);
            renameThreadFolderCheckBox.TabIndex = 14;
            renameThreadFolderCheckBox.Text = "Rename thread folder when removed";
            tooltip.SetToolTip(renameThreadFolderCheckBox, resources.GetString("renameThreadFolderCheckBox.ToolTip"));
            renameThreadFolderCheckBox.UseVisualStyleBackColor = true;
            renameThreadFolderCheckBox.CheckedChanged += renameThreadFolderCheckBox_CheckedChanged;
            // 
            // chkStartWithWindowsMinimized
            // 
            chkStartWithWindowsMinimized.AutoSize = true;
            chkStartWithWindowsMinimized.Location = new Point(29, 295);
            chkStartWithWindowsMinimized.Margin = new Padding(4, 3, 4, 3);
            chkStartWithWindowsMinimized.Name = "chkStartWithWindowsMinimized";
            chkStartWithWindowsMinimized.Size = new Size(166, 19);
            chkStartWithWindowsMinimized.TabIndex = 20;
            chkStartWithWindowsMinimized.Text = "Start hidden in system tray";
            tooltip.SetToolTip(chkStartWithWindowsMinimized, "When starting with windows start minimized to the system tray.\r\nTo use this option you must enable both \"Start GCHan with Windows\" and \"Minimize to Tray\" options.");
            chkStartWithWindowsMinimized.UseVisualStyleBackColor = true;
            // 
            // imageFilenameFormatLabel
            // 
            imageFilenameFormatLabel.AutoSize = true;
            imageFilenameFormatLabel.Location = new Point(14, 75);
            imageFilenameFormatLabel.Margin = new Padding(4, 0, 4, 0);
            imageFilenameFormatLabel.Name = "imageFilenameFormatLabel";
            imageFilenameFormatLabel.Size = new Size(132, 15);
            imageFilenameFormatLabel.TabIndex = 15;
            imageFilenameFormatLabel.Text = "Image Filename Format";
            // 
            // imageFilenameFormatComboBox
            // 
            imageFilenameFormatComboBox.DisplayMember = "EnumDescription";
            imageFilenameFormatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            imageFilenameFormatComboBox.DropDownWidth = 300;
            imageFilenameFormatComboBox.Location = new Point(156, 72);
            imageFilenameFormatComboBox.Margin = new Padding(4, 3, 4, 3);
            imageFilenameFormatComboBox.Name = "imageFilenameFormatComboBox";
            imageFilenameFormatComboBox.Size = new Size(215, 23);
            imageFilenameFormatComboBox.TabIndex = 16;
            imageFilenameFormatComboBox.ValueMember = "EnumValue";
            imageFilenameFormatComboBox.DropDown += imageFilenameFormatComboBox_DropDown;
            // 
            // addUrlFromClipboardWhenTextboxEmpty
            // 
            addUrlFromClipboardWhenTextboxEmpty.AutoSize = true;
            addUrlFromClipboardWhenTextboxEmpty.Location = new Point(18, 322);
            addUrlFromClipboardWhenTextboxEmpty.Margin = new Padding(4, 3, 4, 3);
            addUrlFromClipboardWhenTextboxEmpty.Name = "addUrlFromClipboardWhenTextboxEmpty";
            addUrlFromClipboardWhenTextboxEmpty.Size = new Size(255, 19);
            addUrlFromClipboardWhenTextboxEmpty.TabIndex = 17;
            addUrlFromClipboardWhenTextboxEmpty.Text = "Add URL from clipboard if textbox is empty";
            addUrlFromClipboardWhenTextboxEmpty.UseVisualStyleBackColor = true;
            // 
            // threadFolderNameFormatLabel
            // 
            threadFolderNameFormatLabel.AutoSize = true;
            threadFolderNameFormatLabel.Location = new Point(26, 132);
            threadFolderNameFormatLabel.Margin = new Padding(4, 0, 4, 0);
            threadFolderNameFormatLabel.Name = "threadFolderNameFormatLabel";
            threadFolderNameFormatLabel.Size = new Size(155, 15);
            threadFolderNameFormatLabel.TabIndex = 18;
            threadFolderNameFormatLabel.Text = "Thread Folder Name Format";
            // 
            // threadFolderNameFormatComboBox
            // 
            threadFolderNameFormatComboBox.DisplayMember = "EnumDescription";
            threadFolderNameFormatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            threadFolderNameFormatComboBox.FormattingEnabled = true;
            threadFolderNameFormatComboBox.Location = new Point(195, 129);
            threadFolderNameFormatComboBox.Margin = new Padding(4, 3, 4, 3);
            threadFolderNameFormatComboBox.Name = "threadFolderNameFormatComboBox";
            threadFolderNameFormatComboBox.Size = new Size(177, 23);
            threadFolderNameFormatComboBox.TabIndex = 19;
            threadFolderNameFormatComboBox.ValueMember = "EnumValue";
            // 
            // checkForUpdatesOnStartCheckBox
            // 
            checkForUpdatesOnStartCheckBox.AutoSize = true;
            checkForUpdatesOnStartCheckBox.Location = new Point(18, 348);
            checkForUpdatesOnStartCheckBox.Margin = new Padding(4, 3, 4, 3);
            checkForUpdatesOnStartCheckBox.Name = "checkForUpdatesOnStartCheckBox";
            checkForUpdatesOnStartCheckBox.Size = new Size(224, 19);
            checkForUpdatesOnStartCheckBox.TabIndex = 21;
            checkForUpdatesOnStartCheckBox.Text = "Check for updates when GChan starts";
            checkForUpdatesOnStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AcceptButton = btnSSave;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(378, 414);
            ControlBox = false;
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
            Controls.Add(chkHTML);
            Controls.Add(chkWarn);
            Controls.Add(btnSSave);
            Controls.Add(chkTray);
            Controls.Add(btnSCan);
            Controls.Add(chkSave);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            Shown += Settings_Shown;
            ((System.ComponentModel.ISupportInitialize)timerNumeric).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnSSave;
        private System.Windows.Forms.Button btnSCan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkHTML;
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
    }
}