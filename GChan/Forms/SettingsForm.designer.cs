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
            chkMinimiseToTray = new System.Windows.Forms.CheckBox();
            chkWarn = new System.Windows.Forms.CheckBox();
            setPathButton = new System.Windows.Forms.Button();
            chkStartWithWindows = new System.Windows.Forms.CheckBox();
            minSecondsBetweenScrapesNumeric = new System.Windows.Forms.NumericUpDown();
            directoryTextBox = new System.Windows.Forms.TextBox();
            tooltip = new System.Windows.Forms.ToolTip(components);
            concurrentDownloadsLabel = new System.Windows.Forms.Label();
            chkSaveThumbnails = new System.Windows.Forms.CheckBox();
            userAgentLabel = new System.Windows.Forms.Label();
            headerRateLimiting = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            renameThreadFolderCheckBox = new System.Windows.Forms.CheckBox();
            max1RequestPerSecondCheckBox = new System.Windows.Forms.CheckBox();
            imageFilenameFormatLabel = new System.Windows.Forms.Label();
            imageFilenameFormatComboBox = new System.Windows.Forms.ComboBox();
            threadFolderNameFormatLabel = new System.Windows.Forms.Label();
            threadFolderNameFormatComboBox = new System.Windows.Forms.ComboBox();
            checkForUpdatesOnStartCheckBox = new System.Windows.Forms.CheckBox();
            concurrentDownloadsNumeric = new System.Windows.Forms.NumericUpDown();
            userAgentTextBox = new System.Windows.Forms.TextBox();
            settingsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            label12 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            aboutRateLimitingButton = new System.Windows.Forms.Button();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            label15 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            saveCloseLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)minSecondsBetweenScrapesNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)concurrentDownloadsNumeric).BeginInit();
            settingsLayoutPanel.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            saveCloseLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // buttonSave
            // 
            buttonSave.Dock = System.Windows.Forms.DockStyle.Right;
            buttonSave.Location = new System.Drawing.Point(147, 3);
            buttonSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(130, 29);
            buttonSave.TabIndex = 0;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Dock = System.Windows.Forms.DockStyle.Left;
            buttonCancel.Location = new System.Drawing.Point(285, 3);
            buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(130, 29);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Close";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(4, 192);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(192, 32);
            label1.TabIndex = 2;
            label1.Text = "Path";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label1, "The path for scraped files & HTML to be downloaded into.");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = System.Windows.Forms.DockStyle.Fill;
            label2.Location = new System.Drawing.Point(4, 64);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(192, 32);
            label2.TabIndex = 3;
            label2.Text = "Seconds Between Scrapes";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label2, "The minimum amount of time (in seconds) between board and thread scrapes.\r\nE.g. When a thread is scraped, leave it in queue for a minimum of 60 seconds before scraping again.");
            // 
            // chkSaveHtml
            // 
            chkSaveHtml.AutoSize = true;
            chkSaveHtml.Dock = System.Windows.Forms.DockStyle.Fill;
            chkSaveHtml.Location = new System.Drawing.Point(204, 323);
            chkSaveHtml.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkSaveHtml.Name = "chkSaveHtml";
            chkSaveHtml.Size = new System.Drawing.Size(354, 26);
            chkSaveHtml.TabIndex = 6;
            chkSaveHtml.UseVisualStyleBackColor = true;
            chkSaveHtml.CheckedChanged += chkHTML_CheckedChanged;
            // 
            // chkSave
            // 
            chkSave.AutoSize = true;
            chkSave.Dock = System.Windows.Forms.DockStyle.Fill;
            chkSave.Location = new System.Drawing.Point(204, 451);
            chkSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkSave.Name = "chkSave";
            chkSave.Size = new System.Drawing.Size(354, 26);
            chkSave.TabIndex = 7;
            chkSave.UseVisualStyleBackColor = true;
            // 
            // chkMinimiseToTray
            // 
            chkMinimiseToTray.AutoSize = true;
            chkMinimiseToTray.Dock = System.Windows.Forms.DockStyle.Fill;
            chkMinimiseToTray.Location = new System.Drawing.Point(204, 483);
            chkMinimiseToTray.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMinimiseToTray.Name = "chkMinimiseToTray";
            chkMinimiseToTray.Size = new System.Drawing.Size(354, 26);
            chkMinimiseToTray.TabIndex = 8;
            chkMinimiseToTray.UseVisualStyleBackColor = true;
            // 
            // chkWarn
            // 
            chkWarn.AutoSize = true;
            chkWarn.Dock = System.Windows.Forms.DockStyle.Fill;
            chkWarn.Location = new System.Drawing.Point(204, 419);
            chkWarn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkWarn.Name = "chkWarn";
            chkWarn.Size = new System.Drawing.Size(354, 26);
            chkWarn.TabIndex = 9;
            chkWarn.UseVisualStyleBackColor = true;
            // 
            // setPathButton
            // 
            setPathButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            setPathButton.Location = new System.Drawing.Point(328, 3);
            setPathButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            setPathButton.Name = "setPathButton";
            setPathButton.Size = new System.Drawing.Size(24, 20);
            setPathButton.TabIndex = 10;
            setPathButton.Text = "...";
            setPathButton.UseVisualStyleBackColor = true;
            setPathButton.Click += SetPathButton_Click;
            // 
            // chkStartWithWindows
            // 
            chkStartWithWindows.AutoSize = true;
            chkStartWithWindows.Dock = System.Windows.Forms.DockStyle.Fill;
            chkStartWithWindows.Location = new System.Drawing.Point(204, 387);
            chkStartWithWindows.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkStartWithWindows.Name = "chkStartWithWindows";
            chkStartWithWindows.Size = new System.Drawing.Size(354, 26);
            chkStartWithWindows.TabIndex = 11;
            chkStartWithWindows.UseVisualStyleBackColor = true;
            // 
            // minSecondsBetweenScrapesNumeric
            // 
            minSecondsBetweenScrapesNumeric.Dock = System.Windows.Forms.DockStyle.Fill;
            minSecondsBetweenScrapesNumeric.Location = new System.Drawing.Point(204, 67);
            minSecondsBetweenScrapesNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            minSecondsBetweenScrapesNumeric.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            minSecondsBetweenScrapesNumeric.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            minSecondsBetweenScrapesNumeric.Name = "minSecondsBetweenScrapesNumeric";
            minSecondsBetweenScrapesNumeric.RightToLeft = System.Windows.Forms.RightToLeft.No;
            minSecondsBetweenScrapesNumeric.Size = new System.Drawing.Size(354, 23);
            minSecondsBetweenScrapesNumeric.TabIndex = 12;
            minSecondsBetweenScrapesNumeric.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // directoryTextBox
            // 
            directoryTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            directoryTextBox.Location = new System.Drawing.Point(4, 3);
            directoryTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            directoryTextBox.Name = "directoryTextBox";
            directoryTextBox.ReadOnly = true;
            directoryTextBox.Size = new System.Drawing.Size(316, 23);
            directoryTextBox.TabIndex = 13;
            tooltip.SetToolTip(directoryTextBox, "The directory that GChan will save files in. Double click to open or go File->Open Folder in GChan's main window.");
            directoryTextBox.DoubleClick += textBox1_DoubleClick;
            // 
            // concurrentDownloadsLabel
            // 
            concurrentDownloadsLabel.AutoSize = true;
            concurrentDownloadsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            concurrentDownloadsLabel.Location = new System.Drawing.Point(4, 128);
            concurrentDownloadsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            concurrentDownloadsLabel.Name = "concurrentDownloadsLabel";
            concurrentDownloadsLabel.Size = new System.Drawing.Size(192, 32);
            concurrentDownloadsLabel.TabIndex = 22;
            concurrentDownloadsLabel.Text = "Max Concurrent Downloads";
            concurrentDownloadsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(concurrentDownloadsLabel, "The maximum amount of files allowed to be downloading at once.\r\nA high amount can effect your network (e.g. gaming latency).");
            // 
            // chkSaveThumbnails
            // 
            chkSaveThumbnails.AutoSize = true;
            chkSaveThumbnails.Dock = System.Windows.Forms.DockStyle.Fill;
            chkSaveThumbnails.Location = new System.Drawing.Point(204, 355);
            chkSaveThumbnails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkSaveThumbnails.Name = "chkSaveThumbnails";
            chkSaveThumbnails.Size = new System.Drawing.Size(354, 26);
            chkSaveThumbnails.TabIndex = 24;
            tooltip.SetToolTip(chkSaveThumbnails, "When downloading thread HTML, also save thumbnails for images and place in \"thumb\" folder.");
            chkSaveThumbnails.UseVisualStyleBackColor = true;
            // 
            // userAgentLabel
            // 
            userAgentLabel.AutoSize = true;
            userAgentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            userAgentLabel.Location = new System.Drawing.Point(4, 32);
            userAgentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            userAgentLabel.Name = "userAgentLabel";
            userAgentLabel.Size = new System.Drawing.Size(192, 32);
            userAgentLabel.TabIndex = 25;
            userAgentLabel.Text = "User-Agent Header";
            userAgentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(userAgentLabel, resources.GetString("userAgentLabel.ToolTip"));
            // 
            // headerRateLimiting
            // 
            headerRateLimiting.AutoSize = true;
            headerRateLimiting.Dock = System.Windows.Forms.DockStyle.Fill;
            headerRateLimiting.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            headerRateLimiting.Location = new System.Drawing.Point(3, 0);
            headerRateLimiting.Name = "headerRateLimiting";
            headerRateLimiting.Size = new System.Drawing.Size(194, 32);
            headerRateLimiting.TabIndex = 0;
            headerRateLimiting.Text = "Rate Limiting";
            headerRateLimiting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(headerRateLimiting, "Settings relating to rate limiting, please press the button to the right to learn more.");
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = System.Windows.Forms.DockStyle.Fill;
            label3.Location = new System.Drawing.Point(3, 96);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(194, 32);
            label3.TabIndex = 28;
            label3.Text = "Limit Requests to 1 per second";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label3, "Allow a maximum of 1 request per second to 4chan, this is highly recommended to avoid getting rate limited or banned.");
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = System.Windows.Forms.DockStyle.Fill;
            label5.Location = new System.Drawing.Point(4, 384);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(192, 32);
            label5.TabIndex = 31;
            label5.Text = "Start GChan with Windows";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label5, "Will start in system tray if \"Minimize to Tray\" setting is enabled.");
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = System.Windows.Forms.DockStyle.Fill;
            label8.Location = new System.Drawing.Point(3, 448);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(194, 32);
            label8.TabIndex = 34;
            label8.Text = "Save URLs on exit";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label8, "Persist scraping threads, boards & saved files/thumbnails across application startups / system reboots.");
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Dock = System.Windows.Forms.DockStyle.Fill;
            label11.Location = new System.Drawing.Point(3, 480);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(194, 32);
            label11.TabIndex = 37;
            label11.Text = "Minimize to Tray";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label11, "When minimised, GChan will be hidden from taskbar and appear in the system tray (bottom right).\r\nAllowing you to declutter your desktop while it scrapes in the background.");
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Dock = System.Windows.Forms.DockStyle.Fill;
            label16.Location = new System.Drawing.Point(3, 256);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(194, 32);
            label16.TabIndex = 42;
            label16.Text = "Rename thread folder on removal";
            label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label16, "When a thread is automatically or manually removed from GChan's tracking list, if this option ");
            // 
            // renameThreadFolderCheckBox
            // 
            renameThreadFolderCheckBox.AutoSize = true;
            renameThreadFolderCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            renameThreadFolderCheckBox.Location = new System.Drawing.Point(204, 259);
            renameThreadFolderCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            renameThreadFolderCheckBox.Name = "renameThreadFolderCheckBox";
            renameThreadFolderCheckBox.Size = new System.Drawing.Size(354, 26);
            renameThreadFolderCheckBox.TabIndex = 14;
            renameThreadFolderCheckBox.UseVisualStyleBackColor = true;
            renameThreadFolderCheckBox.CheckedChanged += renameThreadFolderCheckBox_CheckedChanged;
            // 
            // max1RequestPerSecondCheckBox
            // 
            max1RequestPerSecondCheckBox.AutoSize = true;
            max1RequestPerSecondCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            max1RequestPerSecondCheckBox.Location = new System.Drawing.Point(204, 99);
            max1RequestPerSecondCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            max1RequestPerSecondCheckBox.Name = "max1RequestPerSecondCheckBox";
            max1RequestPerSecondCheckBox.Size = new System.Drawing.Size(354, 26);
            max1RequestPerSecondCheckBox.TabIndex = 27;
            max1RequestPerSecondCheckBox.UseVisualStyleBackColor = true;
            // 
            // imageFilenameFormatLabel
            // 
            imageFilenameFormatLabel.AutoSize = true;
            imageFilenameFormatLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            imageFilenameFormatLabel.Location = new System.Drawing.Point(4, 224);
            imageFilenameFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            imageFilenameFormatLabel.Name = "imageFilenameFormatLabel";
            imageFilenameFormatLabel.Size = new System.Drawing.Size(192, 32);
            imageFilenameFormatLabel.TabIndex = 15;
            imageFilenameFormatLabel.Text = "Files Filename Format";
            imageFilenameFormatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(imageFilenameFormatLabel, "Filename format for saved files.");
            // 
            // imageFilenameFormatComboBox
            // 
            imageFilenameFormatComboBox.DisplayMember = "EnumDescription";
            imageFilenameFormatComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            imageFilenameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            imageFilenameFormatComboBox.DropDownWidth = 300;
            imageFilenameFormatComboBox.Location = new System.Drawing.Point(204, 227);
            imageFilenameFormatComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            imageFilenameFormatComboBox.Name = "imageFilenameFormatComboBox";
            imageFilenameFormatComboBox.Size = new System.Drawing.Size(354, 23);
            imageFilenameFormatComboBox.TabIndex = 16;
            imageFilenameFormatComboBox.ValueMember = "EnumValue";
            // 
            // threadFolderNameFormatLabel
            // 
            threadFolderNameFormatLabel.AutoSize = true;
            threadFolderNameFormatLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            threadFolderNameFormatLabel.Location = new System.Drawing.Point(4, 288);
            threadFolderNameFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            threadFolderNameFormatLabel.Name = "threadFolderNameFormatLabel";
            threadFolderNameFormatLabel.Size = new System.Drawing.Size(192, 32);
            threadFolderNameFormatLabel.TabIndex = 18;
            threadFolderNameFormatLabel.Text = "     Thread Folder Name Format";
            threadFolderNameFormatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(threadFolderNameFormatLabel, "Format for thread folder names once removed.");
            // 
            // threadFolderNameFormatComboBox
            // 
            threadFolderNameFormatComboBox.DisplayMember = "EnumDescription";
            threadFolderNameFormatComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            threadFolderNameFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            threadFolderNameFormatComboBox.FormattingEnabled = true;
            threadFolderNameFormatComboBox.Location = new System.Drawing.Point(204, 291);
            threadFolderNameFormatComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            threadFolderNameFormatComboBox.Name = "threadFolderNameFormatComboBox";
            threadFolderNameFormatComboBox.Size = new System.Drawing.Size(354, 23);
            threadFolderNameFormatComboBox.TabIndex = 19;
            threadFolderNameFormatComboBox.ValueMember = "EnumValue";
            // 
            // checkForUpdatesOnStartCheckBox
            // 
            checkForUpdatesOnStartCheckBox.AutoSize = true;
            checkForUpdatesOnStartCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            checkForUpdatesOnStartCheckBox.Location = new System.Drawing.Point(204, 515);
            checkForUpdatesOnStartCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkForUpdatesOnStartCheckBox.Name = "checkForUpdatesOnStartCheckBox";
            checkForUpdatesOnStartCheckBox.Size = new System.Drawing.Size(354, 27);
            checkForUpdatesOnStartCheckBox.TabIndex = 21;
            checkForUpdatesOnStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // concurrentDownloadsNumeric
            // 
            concurrentDownloadsNumeric.Dock = System.Windows.Forms.DockStyle.Fill;
            concurrentDownloadsNumeric.Location = new System.Drawing.Point(204, 131);
            concurrentDownloadsNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            concurrentDownloadsNumeric.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            concurrentDownloadsNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            concurrentDownloadsNumeric.Name = "concurrentDownloadsNumeric";
            concurrentDownloadsNumeric.Size = new System.Drawing.Size(354, 23);
            concurrentDownloadsNumeric.TabIndex = 23;
            concurrentDownloadsNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // userAgentTextBox
            // 
            userAgentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            userAgentTextBox.Location = new System.Drawing.Point(204, 35);
            userAgentTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            userAgentTextBox.Name = "userAgentTextBox";
            userAgentTextBox.Size = new System.Drawing.Size(354, 23);
            userAgentTextBox.TabIndex = 26;
            // 
            // settingsLayoutPanel
            // 
            settingsLayoutPanel.ColumnCount = 2;
            settingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            settingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            settingsLayoutPanel.Controls.Add(headerRateLimiting, 0, 0);
            settingsLayoutPanel.Controls.Add(userAgentLabel, 0, 1);
            settingsLayoutPanel.Controls.Add(chkSave, 1, 14);
            settingsLayoutPanel.Controls.Add(chkStartWithWindows, 1, 12);
            settingsLayoutPanel.Controls.Add(chkSaveHtml, 1, 10);
            settingsLayoutPanel.Controls.Add(label12, 0, 10);
            settingsLayoutPanel.Controls.Add(chkSaveThumbnails, 1, 11);
            settingsLayoutPanel.Controls.Add(label6, 0, 11);
            settingsLayoutPanel.Controls.Add(max1RequestPerSecondCheckBox, 1, 3);
            settingsLayoutPanel.Controls.Add(concurrentDownloadsNumeric, 1, 4);
            settingsLayoutPanel.Controls.Add(userAgentTextBox, 1, 1);
            settingsLayoutPanel.Controls.Add(threadFolderNameFormatComboBox, 1, 9);
            settingsLayoutPanel.Controls.Add(concurrentDownloadsLabel, 0, 4);
            settingsLayoutPanel.Controls.Add(threadFolderNameFormatLabel, 0, 9);
            settingsLayoutPanel.Controls.Add(label2, 0, 2);
            settingsLayoutPanel.Controls.Add(minSecondsBetweenScrapesNumeric, 1, 2);
            settingsLayoutPanel.Controls.Add(renameThreadFolderCheckBox, 1, 8);
            settingsLayoutPanel.Controls.Add(imageFilenameFormatComboBox, 1, 7);
            settingsLayoutPanel.Controls.Add(label3, 0, 3);
            settingsLayoutPanel.Controls.Add(imageFilenameFormatLabel, 0, 7);
            settingsLayoutPanel.Controls.Add(label4, 0, 5);
            settingsLayoutPanel.Controls.Add(label1, 0, 6);
            settingsLayoutPanel.Controls.Add(label16, 0, 8);
            settingsLayoutPanel.Controls.Add(aboutRateLimitingButton, 1, 0);
            settingsLayoutPanel.Controls.Add(tableLayoutPanel2, 1, 6);
            settingsLayoutPanel.Controls.Add(label15, 0, 16);
            settingsLayoutPanel.Controls.Add(checkForUpdatesOnStartCheckBox, 1, 16);
            settingsLayoutPanel.Controls.Add(label11, 0, 15);
            settingsLayoutPanel.Controls.Add(label8, 0, 14);
            settingsLayoutPanel.Controls.Add(label7, 0, 13);
            settingsLayoutPanel.Controls.Add(label5, 0, 12);
            settingsLayoutPanel.Controls.Add(chkWarn, 1, 13);
            settingsLayoutPanel.Controls.Add(chkMinimiseToTray, 1, 15);
            settingsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            settingsLayoutPanel.Location = new System.Drawing.Point(0, 0);
            settingsLayoutPanel.Name = "settingsLayoutPanel";
            settingsLayoutPanel.RowCount = 17;
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            settingsLayoutPanel.Size = new System.Drawing.Size(562, 545);
            settingsLayoutPanel.TabIndex = 29;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Dock = System.Windows.Forms.DockStyle.Fill;
            label12.Location = new System.Drawing.Point(3, 320);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(194, 32);
            label12.TabIndex = 38;
            label12.Text = "Download Thread HTML";
            label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label12, "Save HTML of each scraped thread within each thread folder.");
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = System.Windows.Forms.DockStyle.Fill;
            label6.Location = new System.Drawing.Point(3, 352);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(194, 32);
            label6.TabIndex = 32;
            label6.Text = "     Download Thumbnails";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label6, "If saving thread HTML, save thumbnails for file types that require them for improved offline viewing.");
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = System.Windows.Forms.DockStyle.Fill;
            label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label4.Location = new System.Drawing.Point(3, 160);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(194, 32);
            label4.TabIndex = 29;
            label4.Text = "Preferences";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // aboutRateLimitingButton
            // 
            aboutRateLimitingButton.Dock = System.Windows.Forms.DockStyle.Right;
            aboutRateLimitingButton.Location = new System.Drawing.Point(436, 3);
            aboutRateLimitingButton.Name = "aboutRateLimitingButton";
            aboutRateLimitingButton.Size = new System.Drawing.Size(123, 26);
            aboutRateLimitingButton.TabIndex = 43;
            aboutRateLimitingButton.Text = "About Rate Limiting";
            tooltip.SetToolTip(aboutRateLimitingButton, "Read more information about 4chan rate limiting.");
            aboutRateLimitingButton.UseVisualStyleBackColor = true;
            aboutRateLimitingButton.Click += aboutRateLimitingButton_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel2.Controls.Add(setPathButton, 1, 0);
            tableLayoutPanel2.Controls.Add(directoryTextBox, 0, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(203, 195);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(356, 26);
            tableLayoutPanel2.TabIndex = 44;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Dock = System.Windows.Forms.DockStyle.Fill;
            label15.Location = new System.Drawing.Point(3, 512);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(194, 33);
            label15.TabIndex = 41;
            label15.Text = "Check for updates on startup";
            label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label15, "Check for new stable versions of GChan when starting the application.");
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = System.Windows.Forms.DockStyle.Fill;
            label7.Location = new System.Drawing.Point(3, 416);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(194, 32);
            label7.TabIndex = 33;
            label7.Text = "Show Exit Warning";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tooltip.SetToolTip(label7, "Display warning upon exit if scrape list is not empty.");
            // 
            // saveCloseLayoutPanel
            // 
            saveCloseLayoutPanel.ColumnCount = 2;
            saveCloseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            saveCloseLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            saveCloseLayoutPanel.Controls.Add(buttonSave, 0, 0);
            saveCloseLayoutPanel.Controls.Add(buttonCancel, 1, 0);
            saveCloseLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            saveCloseLayoutPanel.Location = new System.Drawing.Point(0, 554);
            saveCloseLayoutPanel.Name = "saveCloseLayoutPanel";
            saveCloseLayoutPanel.RowCount = 1;
            saveCloseLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            saveCloseLayoutPanel.Size = new System.Drawing.Size(562, 35);
            saveCloseLayoutPanel.TabIndex = 30;
            // 
            // SettingsForm
            // 
            AcceptButton = buttonSave;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(562, 589);
            ControlBox = false;
            Controls.Add(saveCloseLayoutPanel);
            Controls.Add(settingsLayoutPanel);
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
            ((System.ComponentModel.ISupportInitialize)minSecondsBetweenScrapesNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)concurrentDownloadsNumeric).EndInit();
            settingsLayoutPanel.ResumeLayout(false);
            settingsLayoutPanel.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            saveCloseLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSaveHtml;
        private System.Windows.Forms.CheckBox chkSave;
        private System.Windows.Forms.CheckBox chkMinimiseToTray;
        private System.Windows.Forms.CheckBox chkWarn;
        private System.Windows.Forms.Button setPathButton;
        private System.Windows.Forms.CheckBox chkStartWithWindows;
        private System.Windows.Forms.NumericUpDown minSecondsBetweenScrapesNumeric;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.CheckBox renameThreadFolderCheckBox;
        private System.Windows.Forms.Label imageFilenameFormatLabel;
        private System.Windows.Forms.ComboBox imageFilenameFormatComboBox;
        private System.Windows.Forms.Label threadFolderNameFormatLabel;
        private System.Windows.Forms.ComboBox threadFolderNameFormatComboBox;
        private System.Windows.Forms.CheckBox checkForUpdatesOnStartCheckBox;
        private System.Windows.Forms.Label concurrentDownloadsLabel;
        private System.Windows.Forms.NumericUpDown concurrentDownloadsNumeric;
        private System.Windows.Forms.CheckBox chkSaveThumbnails;
        private System.Windows.Forms.Label userAgentLabel;
        private System.Windows.Forms.TextBox userAgentTextBox;
        private System.Windows.Forms.CheckBox max1RequestPerSecondCheckBox;
        private System.Windows.Forms.TableLayoutPanel settingsLayoutPanel;
        private System.Windows.Forms.Label headerRateLimiting;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button aboutRateLimitingButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel saveCloseLayoutPanel;
    }
}