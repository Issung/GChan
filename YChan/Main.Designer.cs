namespace YChan {
    partial class frmMain {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.msHead = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changelogToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tcApp = new System.Windows.Forms.TabControl();
            this.tpThreads = new System.Windows.Forms.TabPage();
            this.lbThreads = new System.Windows.Forms.ListBox();
            this.tpBoard = new System.Windows.Forms.TabPage();
            this.lbBoards = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.edtURL = new System.Windows.Forms.TextBox();
            this.cmThreads = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmBoards = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.nfTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmTrayOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.cmTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.msHead.SuspendLayout();
            this.tcApp.SuspendLayout();
            this.tpThreads.SuspendLayout();
            this.tpBoard.SuspendLayout();
            this.cmThreads.SuspendLayout();
            this.cmBoards.SuspendLayout();
            this.cmTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // msHead
            // 
            this.msHead.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.msHead.Location = new System.Drawing.Point(0, 0);
            this.msHead.Name = "msHead";
            this.msHead.Size = new System.Drawing.Size(582, 24);
            this.msHead.TabIndex = 0;
            this.msHead.Text = "msHead";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.openFolderToolStripMenuItem1,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.settingsToolStripMenuItem1.Text = "&Settings";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // openFolderToolStripMenuItem1
            // 
            this.openFolderToolStripMenuItem1.Image = global::YChan.Properties.Resources.OpenFolder;
            this.openFolderToolStripMenuItem1.Name = "openFolderToolStripMenuItem1";
            this.openFolderToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.openFolderToolStripMenuItem1.Text = "&Open Folder";
            this.openFolderToolStripMenuItem1.Click += new System.EventHandler(this.openFolderToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(136, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::YChan.Properties.Resources.CloseIcon;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changelogToolStripMenuItem1,
            this.aboutToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // changelogToolStripMenuItem1
            // 
            this.changelogToolStripMenuItem1.Name = "changelogToolStripMenuItem1";
            this.changelogToolStripMenuItem1.Size = new System.Drawing.Size(132, 22);
            this.changelogToolStripMenuItem1.Text = "&Changelog";
            this.changelogToolStripMenuItem1.Click += new System.EventHandler(this.changelogToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(132, 22);
            this.aboutToolStripMenuItem1.Text = "&About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tcApp
            // 
            this.tcApp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcApp.Controls.Add(this.tpThreads);
            this.tcApp.Controls.Add(this.tpBoard);
            this.tcApp.Location = new System.Drawing.Point(12, 56);
            this.tcApp.Name = "tcApp";
            this.tcApp.SelectedIndex = 0;
            this.tcApp.Size = new System.Drawing.Size(558, 281);
            this.tcApp.TabIndex = 1;
            // 
            // tpThreads
            // 
            this.tpThreads.Controls.Add(this.lbThreads);
            this.tpThreads.Location = new System.Drawing.Point(4, 22);
            this.tpThreads.Name = "tpThreads";
            this.tpThreads.Padding = new System.Windows.Forms.Padding(3);
            this.tpThreads.Size = new System.Drawing.Size(550, 255);
            this.tpThreads.TabIndex = 0;
            this.tpThreads.Text = "Threads";
            this.tpThreads.UseVisualStyleBackColor = true;
            // 
            // lbThreads
            // 
            this.lbThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbThreads.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbThreads.FormattingEnabled = true;
            this.lbThreads.Location = new System.Drawing.Point(0, 3);
            this.lbThreads.Name = "lbThreads";
            this.lbThreads.Size = new System.Drawing.Size(547, 247);
            this.lbThreads.TabIndex = 0;
            this.lbThreads.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lbThreads_KeyUp);
            this.lbThreads.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbThreads_MouseDoubleClick);
            this.lbThreads.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbThreads_MouseDown);
            // 
            // tpBoard
            // 
            this.tpBoard.Controls.Add(this.lbBoards);
            this.tpBoard.Location = new System.Drawing.Point(4, 22);
            this.tpBoard.Name = "tpBoard";
            this.tpBoard.Padding = new System.Windows.Forms.Padding(3);
            this.tpBoard.Size = new System.Drawing.Size(550, 255);
            this.tpBoard.TabIndex = 1;
            this.tpBoard.Text = "Boards";
            this.tpBoard.UseVisualStyleBackColor = true;
            // 
            // lbBoards
            // 
            this.lbBoards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbBoards.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbBoards.FormattingEnabled = true;
            this.lbBoards.Location = new System.Drawing.Point(0, 0);
            this.lbBoards.Name = "lbBoards";
            this.lbBoards.Size = new System.Drawing.Size(547, 260);
            this.lbBoards.TabIndex = 1;
            this.lbBoards.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lbBoards_KeyUp);
            this.lbBoards.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbBoards_MouseDoubleClick);
            this.lbBoards.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbBoards_MouseDown);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(414, 28);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // edtURL
            // 
            this.edtURL.AllowDrop = true;
            this.edtURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtURL.Location = new System.Drawing.Point(12, 30);
            this.edtURL.Name = "edtURL";
            this.edtURL.Size = new System.Drawing.Size(396, 20);
            this.edtURL.TabIndex = 3;
            this.edtURL.DragDrop += new System.Windows.Forms.DragEventHandler(this.edtURL_DragDrop);
            this.edtURL.DragEnter += new System.Windows.Forms.DragEventHandler(this.edtURL_DragEnter);
            // 
            // cmThreads
            // 
            this.cmThreads.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem,
            this.openInBrowserToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.cmThreads.Name = "cmThreads";
            this.cmThreads.Size = new System.Drawing.Size(162, 70);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Image = global::YChan.Properties.Resources.OpenFolder;
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Image = global::YChan.Properties.Resources.OpenInBrowserToolStrip;
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.openInBrowserToolStripMenuItem.Text = "Open in Browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::YChan.Properties.Resources.DeleteToolStrip;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.deleteToolStripMenuItem.Text = "Remove";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // cmBoards
            // 
            this.cmBoards.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.cmBoards.Name = "cmThreads";
            this.cmBoards.Size = new System.Drawing.Size(162, 70);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::YChan.Properties.Resources.OpenFolder;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem1.Text = "Open Folder";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.openBoardFolderToolTip_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::YChan.Properties.Resources.OpenInBrowserToolStrip;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem2.Text = "Open in Browser";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.openBoardURLToolTip_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Image = global::YChan.Properties.Resources.DeleteToolStrip;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem3.Text = "Remove";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.deleteBoardToolTip_Click);
            // 
            // nfTray
            // 
            this.nfTray.ContextMenuStrip = this.cmTray;
            this.nfTray.Icon = ((System.Drawing.Icon)(resources.GetObject("nfTray.Icon")));
            this.nfTray.Text = "Doubleclick to open/hide";
            this.nfTray.Visible = true;
            this.nfTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.nfTray_MouseDoubleClick);
            // 
            // cmTray
            // 
            this.cmTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmTrayOpen,
            this.cmTrayExit});
            this.cmTray.Name = "cmTray";
            this.cmTray.Size = new System.Drawing.Size(140, 48);
            // 
            // cmTrayOpen
            // 
            this.cmTrayOpen.Image = global::YChan.Properties.Resources.OpenFolder;
            this.cmTrayOpen.Name = "cmTrayOpen";
            this.cmTrayOpen.Size = new System.Drawing.Size(139, 22);
            this.cmTrayOpen.Text = "Open Folder";
            this.cmTrayOpen.Click += new System.EventHandler(this.cmTrayOpen_Click);
            // 
            // cmTrayExit
            // 
            this.cmTrayExit.Image = global::YChan.Properties.Resources.CloseIcon;
            this.cmTrayExit.Name = "cmTrayExit";
            this.cmTrayExit.Size = new System.Drawing.Size(139, 22);
            this.cmTrayExit.Text = "Exit";
            this.cmTrayExit.Click += new System.EventHandler(this.cmTrayExit_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearAll.Location = new System.Drawing.Point(495, 28);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(75, 23);
            this.btnClearAll.TabIndex = 4;
            this.btnClearAll.Text = "Clear";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 353);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.edtURL);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.tcApp);
            this.Controls.Add(this.msHead);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msHead;
            this.Name = "frmMain";
            this.Text = "YChan";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.msHead.ResumeLayout(false);
            this.msHead.PerformLayout();
            this.tcApp.ResumeLayout(false);
            this.tpThreads.ResumeLayout(false);
            this.tpBoard.ResumeLayout(false);
            this.cmThreads.ResumeLayout(false);
            this.cmBoards.ResumeLayout(false);
            this.cmTray.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip msHead;
        private System.Windows.Forms.TabControl tcApp;
        private System.Windows.Forms.TabPage tpThreads;
        private System.Windows.Forms.TabPage tpBoard;
        private System.Windows.Forms.ListBox lbThreads;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox edtURL;
        private System.Windows.Forms.ListBox lbBoards;
        private System.Windows.Forms.ContextMenuStrip cmThreads;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmBoards;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.NotifyIcon nfTray;
        private System.Windows.Forms.ContextMenuStrip cmTray;
        private System.Windows.Forms.ToolStripMenuItem cmTrayOpen;
        private System.Windows.Forms.ToolStripMenuItem cmTrayExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changelogToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnClearAll;
    }
}

