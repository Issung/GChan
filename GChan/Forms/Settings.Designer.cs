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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.btnSSave = new System.Windows.Forms.Button();
            this.btnSCan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkHTML = new System.Windows.Forms.CheckBox();
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
            this.imageFilenameFormatLabel = new System.Windows.Forms.Label();
            this.imageFilenameFormatComboBox = new System.Windows.Forms.ComboBox();
            this.addUrlFromClipboardWhenTextboxEmpty = new System.Windows.Forms.CheckBox();
            this.threadFolderNameFormatLabel = new System.Windows.Forms.Label();
            this.threadFolderNameFormatComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.timerNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSSave
            // 
            this.btnSSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSSave.Location = new System.Drawing.Point(69, 303);
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
            this.btnSCan.Location = new System.Drawing.Point(167, 303);
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
            // chkHTML
            // 
            this.chkHTML.AutoSize = true;
            this.chkHTML.Location = new System.Drawing.Point(15, 141);
            this.chkHTML.Name = "chkHTML";
            this.chkHTML.Size = new System.Drawing.Size(107, 17);
            this.chkHTML.TabIndex = 6;
            this.chkHTML.Text = "Download HTML";
            this.chkHTML.UseVisualStyleBackColor = true;
            // 
            // chkSave
            // 
            this.chkSave.AutoSize = true;
            this.chkSave.Location = new System.Drawing.Point(15, 186);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(115, 17);
            this.chkSave.TabIndex = 7;
            this.chkSave.Text = "Save URLs on exit";
            this.chkSave.UseVisualStyleBackColor = true;
            // 
            // chkTray
            // 
            this.chkTray.AutoSize = true;
            this.chkTray.Location = new System.Drawing.Point(15, 209);
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
            this.chkWarn.Location = new System.Drawing.Point(15, 164);
            this.chkWarn.Name = "chkWarn";
            this.chkWarn.Size = new System.Drawing.Size(116, 17);
            this.chkWarn.TabIndex = 9;
            this.chkWarn.Text = "Show Exit Warning";
            this.chkWarn.UseVisualStyleBackColor = true;
            // 
            // setPathButton
            // 
            this.setPathButton.Location = new System.Drawing.Point(295, 7);
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
            this.chkStartWithWindows.Location = new System.Drawing.Point(15, 233);
            this.chkStartWithWindows.Name = "chkStartWithWindows";
            this.chkStartWithWindows.Size = new System.Drawing.Size(153, 17);
            this.chkStartWithWindows.TabIndex = 11;
            this.chkStartWithWindows.Text = "Start GChan with Windows";
            this.chkStartWithWindows.UseVisualStyleBackColor = true;
            this.chkStartWithWindows.CheckedChanged += new System.EventHandler(this.chkStartWithWindows_CheckedChanged);
            // 
            // timerNumeric
            // 
            this.timerNumeric.Location = new System.Drawing.Point(77, 35);
            this.timerNumeric.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.timerNumeric.Name = "timerNumeric";
            this.timerNumeric.Size = new System.Drawing.Size(242, 20);
            this.timerNumeric.TabIndex = 12;
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(77, 9);
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
            this.renameThreadFolderCheckBox.Location = new System.Drawing.Point(15, 90);
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
            this.chkStartWithWindowsMinimized.Location = new System.Drawing.Point(25, 256);
            this.chkStartWithWindowsMinimized.Name = "chkStartWithWindowsMinimized";
            this.chkStartWithWindowsMinimized.Size = new System.Drawing.Size(149, 17);
            this.chkStartWithWindowsMinimized.TabIndex = 20;
            this.chkStartWithWindowsMinimized.Text = "Start hidden in system tray";
            this.tooltip.SetToolTip(this.chkStartWithWindowsMinimized, "When starting with windows start minimized to the system tray.\r\nTo use this optio" +
        "n you must enable both \"Start GCHan with Windows\" and \"Minimize to Tray\" options" +
        ".");
            this.chkStartWithWindowsMinimized.UseVisualStyleBackColor = true;
            // 
            // imageFilenameFormatLabel
            // 
            this.imageFilenameFormatLabel.AutoSize = true;
            this.imageFilenameFormatLabel.Location = new System.Drawing.Point(12, 65);
            this.imageFilenameFormatLabel.Name = "imageFilenameFormatLabel";
            this.imageFilenameFormatLabel.Size = new System.Drawing.Size(116, 13);
            this.imageFilenameFormatLabel.TabIndex = 15;
            this.imageFilenameFormatLabel.Text = "Image Filename Format";
            // 
            // imageFilenameFormatComboBox
            // 
            this.imageFilenameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageFilenameFormatComboBox.FormattingEnabled = true;
            this.imageFilenameFormatComboBox.Location = new System.Drawing.Point(134, 62);
            this.imageFilenameFormatComboBox.Name = "imageFilenameFormatComboBox";
            this.imageFilenameFormatComboBox.Size = new System.Drawing.Size(185, 21);
            this.imageFilenameFormatComboBox.TabIndex = 16;
            this.imageFilenameFormatComboBox.DropDown += new System.EventHandler(this.imageFilenameFormatComboBox_DropDown);
            // 
            // addUrlFromClipboardWhenTextboxEmpty
            // 
            this.addUrlFromClipboardWhenTextboxEmpty.AutoSize = true;
            this.addUrlFromClipboardWhenTextboxEmpty.Location = new System.Drawing.Point(15, 279);
            this.addUrlFromClipboardWhenTextboxEmpty.Name = "addUrlFromClipboardWhenTextboxEmpty";
            this.addUrlFromClipboardWhenTextboxEmpty.Size = new System.Drawing.Size(225, 17);
            this.addUrlFromClipboardWhenTextboxEmpty.TabIndex = 17;
            this.addUrlFromClipboardWhenTextboxEmpty.Text = "Add URL from clipboard if textbox is empty";
            this.addUrlFromClipboardWhenTextboxEmpty.UseVisualStyleBackColor = true;
            // 
            // threadFolderNameFormatLabel
            // 
            this.threadFolderNameFormatLabel.AutoSize = true;
            this.threadFolderNameFormatLabel.Location = new System.Drawing.Point(22, 114);
            this.threadFolderNameFormatLabel.Name = "threadFolderNameFormatLabel";
            this.threadFolderNameFormatLabel.Size = new System.Drawing.Size(139, 13);
            this.threadFolderNameFormatLabel.TabIndex = 18;
            this.threadFolderNameFormatLabel.Text = "Thread Folder Name Format";
            // 
            // threadFolderNameFormatComboBox
            // 
            this.threadFolderNameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.threadFolderNameFormatComboBox.FormattingEnabled = true;
            this.threadFolderNameFormatComboBox.Location = new System.Drawing.Point(167, 112);
            this.threadFolderNameFormatComboBox.Name = "threadFolderNameFormatComboBox";
            this.threadFolderNameFormatComboBox.Size = new System.Drawing.Size(152, 21);
            this.threadFolderNameFormatComboBox.TabIndex = 19;
            // 
            // Settings
            // 
            this.AcceptButton = this.btnSSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(327, 338);
            this.ControlBox = false;
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
            this.Controls.Add(this.chkHTML);
            this.Controls.Add(this.chkWarn);
            this.Controls.Add(this.btnSSave);
            this.Controls.Add(this.chkTray);
            this.Controls.Add(this.btnSCan);
            this.Controls.Add(this.chkSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.Settings_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.timerNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}