using GChan.ViewModels;

namespace GChan.Forms
{
    partial class MainForm {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changelogToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateAvailableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listsTabControl = new System.Windows.Forms.TabControl();
            this.threadsTabPage = new System.Windows.Forms.TabPage();
            this.threadGridView = new GChan.Controls.PreferencesDataGridView();
            this.threadsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyURLToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.threadsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mainFormModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.boardsTabPage = new System.Windows.Forms.TabPage();
            this.boardsListBox = new System.Windows.Forms.ListBox();
            this.boardsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.boardsContextMenuOpenFolderButton = new System.Windows.Forms.ToolStripMenuItem();
            this.boardsContextMenuOpenInBrowserButton = new System.Windows.Forms.ToolStripMenuItem();
            this.boardsContextMenuRemoveButton = new System.Windows.Forms.ToolStripMenuItem();
            this.boardsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.addButton = new System.Windows.Forms.Button();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.systemTrayNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.systemTrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.systemTrayOpenFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemTrayExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllButton = new System.Windows.Forms.Button();
            this.clipboardButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.threadGridSubjectColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.threadGridSiteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.threadGridBoardCodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.threadGridIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.threadGridFileCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip.SuspendLayout();
            this.listsTabControl.SuspendLayout();
            this.threadsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadGridView)).BeginInit();
            this.threadsContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainFormModelBindingSource)).BeginInit();
            this.boardsTabPage.SuspendLayout();
            this.boardsContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boardsBindingSource)).BeginInit();
            this.systemTrayContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.updateAvailableToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(582, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "msHead";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.openFolderToolStripMenuItem1,
            this.openLogsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Image = global::GChan.Properties.Resources.settings_wrench;
            this.settingsToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(179, 22);
            this.settingsToolStripMenuItem1.Text = "&Settings";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // openFolderToolStripMenuItem1
            // 
            this.openFolderToolStripMenuItem1.Image = global::GChan.Properties.Resources.folder;
            this.openFolderToolStripMenuItem1.Name = "openFolderToolStripMenuItem1";
            this.openFolderToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.openFolderToolStripMenuItem1.Size = new System.Drawing.Size(179, 22);
            this.openFolderToolStripMenuItem1.Text = "Open &Folder";
            this.openFolderToolStripMenuItem1.Click += new System.EventHandler(this.openFolderToolStripMenuItem1_Click);
            // 
            // openLogsToolStripMenuItem
            // 
            this.openLogsToolStripMenuItem.Image = global::GChan.Properties.Resources.file;
            this.openLogsToolStripMenuItem.Name = "openLogsToolStripMenuItem";
            this.openLogsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.openLogsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.openLogsToolStripMenuItem.Text = "Open Logs";
            this.openLogsToolStripMenuItem.ToolTipText = "The ProgramData folder contains the files used to store saved threads & boards, a" +
    "nd also crash logs to help with debugging.";
            this.openLogsToolStripMenuItem.Click += new System.EventHandler(this.openLogsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::GChan.Properties.Resources.close;
            this.exitToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changelogToolStripMenuItem1,
            this.aboutToolStripMenuItem1,
            this.checkForUpdatesToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // changelogToolStripMenuItem1
            // 
            this.changelogToolStripMenuItem1.BackColor = System.Drawing.SystemColors.Control;
            this.changelogToolStripMenuItem1.Image = global::GChan.Properties.Resources.alert;
            this.changelogToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.changelogToolStripMenuItem1.Name = "changelogToolStripMenuItem1";
            this.changelogToolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
            this.changelogToolStripMenuItem1.Text = "&Changelog";
            this.changelogToolStripMenuItem1.Click += new System.EventHandler(this.changelogToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Image = global::GChan.Properties.Resources.question;
            this.aboutToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
            this.aboutToolStripMenuItem1.Text = "&About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Image = global::GChan.Properties.Resources.download;
            this.checkForUpdatesToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for Updates";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // updateAvailableToolStripMenuItem
            // 
            this.updateAvailableToolStripMenuItem.Image = global::GChan.Properties.Resources.alert;
            this.updateAvailableToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.updateAvailableToolStripMenuItem.Name = "updateAvailableToolStripMenuItem";
            this.updateAvailableToolStripMenuItem.Size = new System.Drawing.Size(126, 20);
            this.updateAvailableToolStripMenuItem.Text = "Update Available!";
            this.updateAvailableToolStripMenuItem.ToolTipText = "An update is available for GChan! Click here for more info.";
            this.updateAvailableToolStripMenuItem.Visible = false;
            this.updateAvailableToolStripMenuItem.Click += new System.EventHandler(this.updateAvailableToolStripMenuItem_Click);
            // 
            // listsTabControl
            // 
            this.listsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listsTabControl.Controls.Add(this.threadsTabPage);
            this.listsTabControl.Controls.Add(this.boardsTabPage);
            this.listsTabControl.Location = new System.Drawing.Point(12, 56);
            this.listsTabControl.Name = "listsTabControl";
            this.listsTabControl.SelectedIndex = 0;
            this.listsTabControl.Size = new System.Drawing.Size(558, 222);
            this.listsTabControl.TabIndex = 1;
            // 
            // threadsTabPage
            // 
            this.threadsTabPage.Controls.Add(this.threadGridView);
            this.threadsTabPage.Location = new System.Drawing.Point(4, 22);
            this.threadsTabPage.Name = "threadsTabPage";
            this.threadsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.threadsTabPage.Size = new System.Drawing.Size(550, 196);
            this.threadsTabPage.TabIndex = 0;
            this.threadsTabPage.Text = "Threads (0)";
            this.threadsTabPage.UseVisualStyleBackColor = true;
            // 
            // threadGridView
            // 
            this.threadGridView.AllowUserToAddRows = false;
            this.threadGridView.AllowUserToDeleteRows = false;
            this.threadGridView.AllowUserToOrderColumns = true;
            this.threadGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle1.NullValue = " ";
            this.threadGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.threadGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.threadGridView.AutoGenerateColumns = false;
            this.threadGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.threadGridView.BackgroundColor = System.Drawing.Color.White;
            this.threadGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.threadGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.threadGridSubjectColumn,
            this.threadGridSiteColumn,
            this.threadGridBoardCodeColumn,
            this.threadGridIdColumn,
            this.threadGridFileCountColumn});
            this.threadGridView.ContextMenuStrip = this.threadsContextMenu;
            this.threadGridView.DataSource = this.threadsBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.NullValue = " ";
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.threadGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.threadGridView.GridColor = System.Drawing.SystemColors.Window;
            this.threadGridView.Location = new System.Drawing.Point(-1, -1);
            this.threadGridView.Margin = new System.Windows.Forms.Padding(0);
            this.threadGridView.MultiSelect = false;
            this.threadGridView.Name = "threadGridView";
            this.threadGridView.ReadOnly = true;
            this.threadGridView.RowHeadersVisible = false;
            this.threadGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.threadGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.threadGridView.ShowCellErrors = false;
            this.threadGridView.ShowEditingIcon = false;
            this.threadGridView.ShowRowErrors = false;
            this.threadGridView.Size = new System.Drawing.Size(552, 198);
            this.threadGridView.TabIndex = 1;
            this.threadGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.threadGridView_DataError);
            this.threadGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.threadGridView_MouseDown);
            // 
            // threadsContextMenu
            // 
            this.threadsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem,
            this.openInBrowserToolStripMenuItem,
            this.copyURLToClipboardToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.threadsContextMenu.Name = "cmThreads";
            this.threadsContextMenu.Size = new System.Drawing.Size(238, 114);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Image = global::GChan.Properties.Resources.folder;
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.openFolderToolStripMenuItem.Text = "Open &Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.AutoSize = false;
            this.openInBrowserToolStripMenuItem.Image = global::GChan.Properties.Resources.world;
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.openInBrowserToolStripMenuItem.Text = "Open in &Browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // copyURLToClipboardToolStripMenuItem
            // 
            this.copyURLToClipboardToolStripMenuItem.Image = global::GChan.Properties.Resources.clipboard;
            this.copyURLToClipboardToolStripMenuItem.Name = "copyURLToClipboardToolStripMenuItem";
            this.copyURLToClipboardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyURLToClipboardToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.copyURLToClipboardToolStripMenuItem.Text = "Copy &URL to Clipboard";
            this.copyURLToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyURLToClipboardToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Image = global::GChan.Properties.Resources.Rename;
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.renameToolStripMenuItem.Text = "&Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::GChan.Properties.Resources.close;
            this.deleteToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.deleteToolStripMenuItem.Text = "R&emove";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // threadsBindingSource
            // 
            this.threadsBindingSource.DataMember = "Threads";
            this.threadsBindingSource.DataSource = this.mainFormModelBindingSource;
            // 
            // mainFormModelBindingSource
            // 
            this.mainFormModelBindingSource.DataSource = typeof(MainFormModel);
            // 
            // boardsTabPage
            // 
            this.boardsTabPage.Controls.Add(this.boardsListBox);
            this.boardsTabPage.Location = new System.Drawing.Point(4, 22);
            this.boardsTabPage.Name = "boardsTabPage";
            this.boardsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.boardsTabPage.Size = new System.Drawing.Size(550, 196);
            this.boardsTabPage.TabIndex = 1;
            this.boardsTabPage.Text = "Boards (0)";
            this.boardsTabPage.UseVisualStyleBackColor = true;
            // 
            // boardsListBox
            // 
            this.boardsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.boardsListBox.ContextMenuStrip = this.boardsContextMenu;
            this.boardsListBox.DataSource = this.boardsBindingSource;
            this.boardsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boardsListBox.FormattingEnabled = true;
            this.boardsListBox.Location = new System.Drawing.Point(3, 3);
            this.boardsListBox.Name = "boardsListBox";
            this.boardsListBox.Size = new System.Drawing.Size(544, 190);
            this.boardsListBox.TabIndex = 1;
            this.boardsListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.boardsListBox_MouseDoubleClick);
            // 
            // boardsContextMenu
            // 
            this.boardsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boardsContextMenuOpenFolderButton,
            this.boardsContextMenuOpenInBrowserButton,
            this.boardsContextMenuRemoveButton});
            this.boardsContextMenu.Name = "cmThreads";
            this.boardsContextMenu.Size = new System.Drawing.Size(203, 70);
            // 
            // boardsContextMenuOpenFolderButton
            // 
            this.boardsContextMenuOpenFolderButton.Image = global::GChan.Properties.Resources.folder;
            this.boardsContextMenuOpenFolderButton.Name = "boardsContextMenuOpenFolderButton";
            this.boardsContextMenuOpenFolderButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.boardsContextMenuOpenFolderButton.Size = new System.Drawing.Size(202, 22);
            this.boardsContextMenuOpenFolderButton.Text = "Open Folder";
            this.boardsContextMenuOpenFolderButton.Click += new System.EventHandler(this.openBoardFolderToolStripMenuItem_Click);
            // 
            // boardsContextMenuOpenInBrowserButton
            // 
            this.boardsContextMenuOpenInBrowserButton.Image = global::GChan.Properties.Resources.world;
            this.boardsContextMenuOpenInBrowserButton.Name = "boardsContextMenuOpenInBrowserButton";
            this.boardsContextMenuOpenInBrowserButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.boardsContextMenuOpenInBrowserButton.Size = new System.Drawing.Size(202, 22);
            this.boardsContextMenuOpenInBrowserButton.Text = "Open in Browser";
            this.boardsContextMenuOpenInBrowserButton.Click += new System.EventHandler(this.openBoardURLToolStripMenuItem_Click);
            // 
            // boardsContextMenuRemoveButton
            // 
            this.boardsContextMenuRemoveButton.Image = global::GChan.Properties.Resources.close;
            this.boardsContextMenuRemoveButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.boardsContextMenuRemoveButton.Name = "boardsContextMenuRemoveButton";
            this.boardsContextMenuRemoveButton.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.boardsContextMenuRemoveButton.Size = new System.Drawing.Size(202, 22);
            this.boardsContextMenuRemoveButton.Text = "Remove";
            this.boardsContextMenuRemoveButton.Click += new System.EventHandler(this.deleteBoardToolStripMenuItem_Click);
            // 
            // boardsBindingSource
            // 
            this.boardsBindingSource.DataMember = "Boards";
            this.boardsBindingSource.DataSource = this.mainFormModelBindingSource;
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(384, 28);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(74, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.toolTip.SetToolTip(this.addButton, "Add the entered thread/board to tracking.");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.Location = new System.Drawing.Point(12, 30);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(366, 20);
            this.urlTextBox.TabIndex = 3;
            // 
            // systemTrayNotifyIcon
            // 
            this.systemTrayNotifyIcon.ContextMenuStrip = this.systemTrayContextMenu;
            this.systemTrayNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("systemTrayNotifyIcon.Icon")));
            this.systemTrayNotifyIcon.Text = "Click to open/hide";
            this.systemTrayNotifyIcon.Visible = true;
            this.systemTrayNotifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.systemTrayNotifyIcon_MouseDown);
            this.systemTrayNotifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.systemTrayNotifyIcon_MouseMove);
            // 
            // systemTrayContextMenu
            // 
            this.systemTrayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemTrayOpenFolderToolStripMenuItem,
            this.systemTrayExitToolStripMenuItem});
            this.systemTrayContextMenu.Name = "cmTray";
            this.systemTrayContextMenu.Size = new System.Drawing.Size(140, 48);
            // 
            // systemTrayOpenFolderToolStripMenuItem
            // 
            this.systemTrayOpenFolderToolStripMenuItem.Image = global::GChan.Properties.Resources.folder;
            this.systemTrayOpenFolderToolStripMenuItem.Name = "systemTrayOpenFolderToolStripMenuItem";
            this.systemTrayOpenFolderToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.systemTrayOpenFolderToolStripMenuItem.Text = "Open &Folder";
            this.systemTrayOpenFolderToolStripMenuItem.Click += new System.EventHandler(this.systemTrayOpenFolderToolStripMenuItem_Click);
            // 
            // systemTrayExitToolStripMenuItem
            // 
            this.systemTrayExitToolStripMenuItem.Image = global::GChan.Properties.Resources.close;
            this.systemTrayExitToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.systemTrayExitToolStripMenuItem.Name = "systemTrayExitToolStripMenuItem";
            this.systemTrayExitToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.systemTrayExitToolStripMenuItem.Text = "&Exit";
            this.systemTrayExitToolStripMenuItem.Click += new System.EventHandler(this.systemTrayExitToolStripMenuItem_Click);
            // 
            // clearAllButton
            // 
            this.clearAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearAllButton.Location = new System.Drawing.Point(464, 28);
            this.clearAllButton.Name = "clearAllButton";
            this.clearAllButton.Size = new System.Drawing.Size(74, 23);
            this.clearAllButton.TabIndex = 4;
            this.clearAllButton.Text = "Clear";
            this.clearAllButton.UseVisualStyleBackColor = true;
            this.clearAllButton.Click += new System.EventHandler(this.clearAllButton_Click);
            // 
            // clipboardButton
            // 
            this.clipboardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clipboardButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clipboardButton.Location = new System.Drawing.Point(544, 28);
            this.clipboardButton.Name = "clipboardButton";
            this.clipboardButton.Size = new System.Drawing.Size(26, 23);
            this.clipboardButton.TabIndex = 5;
            this.clipboardButton.Text = "📋";
            this.toolTip.SetToolTip(this.clipboardButton, "Copy Thread URLs to Clipboard (Delimited by commas)");
            this.clipboardButton.UseVisualStyleBackColor = true;
            this.clipboardButton.Click += new System.EventHandler(this.clipboardButton_Click);
            // 
            // threadGridSubjectColumn
            // 
            this.threadGridSubjectColumn.DataPropertyName = "Subject";
            this.threadGridSubjectColumn.FillWeight = 25F;
            this.threadGridSubjectColumn.HeaderText = "Subject";
            this.threadGridSubjectColumn.Name = "threadGridSubjectColumn";
            this.threadGridSubjectColumn.ReadOnly = true;
            // 
            // threadGridSiteColumn
            // 
            this.threadGridSiteColumn.DataPropertyName = "SiteDisplayName";
            this.threadGridSiteColumn.FillWeight = 8F;
            this.threadGridSiteColumn.HeaderText = "Site";
            this.threadGridSiteColumn.Name = "threadGridSiteColumn";
            this.threadGridSiteColumn.ReadOnly = true;
            this.threadGridSiteColumn.ToolTipText = "The website the thread is hosted on.";
            // 
            // threadGridBoardCodeColumn
            // 
            this.threadGridBoardCodeColumn.DataPropertyName = "BoardCode";
            this.threadGridBoardCodeColumn.FillWeight = 8.387236F;
            this.threadGridBoardCodeColumn.HeaderText = "Board";
            this.threadGridBoardCodeColumn.Name = "threadGridBoardCodeColumn";
            this.threadGridBoardCodeColumn.ReadOnly = true;
            // 
            // threadGridIdColumn
            // 
            this.threadGridIdColumn.DataPropertyName = "ID";
            this.threadGridIdColumn.FillWeight = 8.387236F;
            this.threadGridIdColumn.HeaderText = "ID";
            this.threadGridIdColumn.Name = "threadGridIdColumn";
            this.threadGridIdColumn.ReadOnly = true;
            // 
            // threadGridFileCountColumn
            // 
            this.threadGridFileCountColumn.DataPropertyName = "FileCount";
            this.threadGridFileCountColumn.FillWeight = 8.387236F;
            this.threadGridFileCountColumn.HeaderText = "File Count";
            this.threadGridFileCountColumn.Name = "threadGridFileCountColumn";
            this.threadGridFileCountColumn.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AcceptButton = this.addButton;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 294);
            this.Controls.Add(this.clipboardButton);
            this.Controls.Add(this.clearAllButton);
            this.Controls.Add(this.urlTextBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.listsTabControl);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "GChan";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropHandler);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterHandler);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.listsTabControl.ResumeLayout(false);
            this.threadsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.threadGridView)).EndInit();
            this.threadsContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.threadsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainFormModelBindingSource)).EndInit();
            this.boardsTabPage.ResumeLayout(false);
            this.boardsContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.boardsBindingSource)).EndInit();
            this.systemTrayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl listsTabControl;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ListBox boardsListBox;
        private System.Windows.Forms.ContextMenuStrip threadsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip boardsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem boardsContextMenuOpenFolderButton;
        private System.Windows.Forms.ToolStripMenuItem boardsContextMenuOpenInBrowserButton;
        private System.Windows.Forms.ToolStripMenuItem boardsContextMenuRemoveButton;
        private System.Windows.Forms.ContextMenuStrip systemTrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem systemTrayOpenFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem systemTrayExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changelogToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem1;
        private System.Windows.Forms.Button clearAllButton;
        private System.Windows.Forms.Button clipboardButton;
        private System.Windows.Forms.ToolStripMenuItem copyURLToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogsToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadsTabTextDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn boardsTabTextDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn notificationTrayTooltipDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource threadsBindingSource;
        private System.Windows.Forms.BindingSource boardsBindingSource;
        internal System.Windows.Forms.TabPage threadsTabPage;
        internal System.Windows.Forms.TabPage boardsTabPage;
        internal Controls.PreferencesDataGridView threadGridView;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        internal System.Windows.Forms.MenuStrip menuStrip;
        internal System.Windows.Forms.ToolTip toolTip;
        internal System.Windows.Forms.BindingSource mainFormModelBindingSource;
        internal System.Windows.Forms.NotifyIcon systemTrayNotifyIcon;
        internal System.Windows.Forms.TextBox urlTextBox;
        internal System.Windows.Forms.ToolStripMenuItem updateAvailableToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadGridSubjectColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadGridSiteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadGridBoardCodeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadGridIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadGridFileCountColumn;
    }
}

