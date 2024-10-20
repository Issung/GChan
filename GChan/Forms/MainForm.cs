using GChan.Controllers;
using GChan.Data;
using GChan.Models.Trackers;
using GChan.Properties;
using GChan.ViewModels;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Type = GChan.Models.Trackers.Type;

namespace GChan.Forms
{
    public partial class MainForm : Form
    {
        internal MainFormModel Model => Controller.Model;

        private readonly MainController Controller;

        /// <summary>
        /// Get the index of the selected row in the thread grid view.
        /// </summary>
        private int ThreadGridViewSelectedRowIndex => threadGridView?.CurrentCell?.RowIndex ?? -1;

        private int BoardsListBoxSelectedRowIndex { get { return boardsListBox.SelectedIndex; } set {boardsListBox.SelectedIndex = value;} }

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public MainForm()
        {
            InitializeComponent();

            Controller = new MainController(this);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await DataController.EnsureCreatedAndMigrate();
            await Controller.LoadTrackers();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //Start minimized if program was started with Program.TRAY_CMDLINE_ARG (-tray) command line argument.
            if (Program.arguments.Contains(Program.TRAY_CMDLINE_ARG))
            {
                WindowState = FormWindowState.Minimized;
                Hide();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Program.WM_MY_MSG)
            {
                if ((m.WParam.ToInt32() == 0xCDCD) && (m.LParam.ToInt32() == 0xEFEF))
                {
                    if (WindowState == FormWindowState.Minimized)
                    {
                        WindowState = FormWindowState.Normal;
                    }

                    // Bring window to front.
                    bool temp = TopMost;
                    TopMost = true;
                    TopMost = temp;

                    // Set focus to the window.
                    Activate();
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var text = urlTextBox.Text; 
            urlTextBox.Text = string.Empty;   // Clear textbox.

            if (string.IsNullOrWhiteSpace(text) && Clipboard.ContainsText() && Settings.Default.AddUrlFromClipboardWhenTextboxEmpty)
            {
                text = Clipboard.GetText();
            }

            var urls = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Utils.PrepareURL);
            Controller.AddUrls(urls);
        }

        private void DragDropHandler(object sender, DragEventArgs e)
        {
            var textData = (string)e.Data.GetData(DataFormats.Text);

            // Get url from TextBox
            var urls = textData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Utils.PrepareURL);
            Controller.AddUrls(urls);
        }

        private void DragEnterHandler(object sender, DragEventArgs e)
        {
            // See if this is a copy and the data includes text.
            if (e.Data.GetDataPresent(DataFormats.Text) && e.AllowedEffect.HasFlag(DragDropEffects.Copy))
            {
                // Allow this.
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // Don't allow any other drop.
                e.Effect = DragDropEffects.None;
            }
        }

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                try
                {
                    await Controller.RemoveThread(Model.Threads[ThreadGridViewSelectedRowIndex], true);
                }
                catch
                {

                }
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                var path = Model.Threads[ThreadGridViewSelectedRowIndex].SaveTo;

                if (!Directory.Exists(path))
                { 
                    Directory.CreateDirectory(path);
                }

                Utils.OpenDirectory(path);
            }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                var url = Model.Threads[ThreadGridViewSelectedRowIndex].Url;
                Utils.OpenWebpage(url);
            }
        }

        private void copyURLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                string spath = Model.Threads[ThreadGridViewSelectedRowIndex].Url;
                Clipboard.SetText(spath);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutBox();
            about.ShowDialog();
            about.Dispose();
        }

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var changelog = new Changelog();
            changelog.ShowDialog();
            changelog.Dispose();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new SettingsForm();
            settings.ShowDialog();
            settings.Dispose();

            systemTrayNotifyIcon.Visible = Settings.Default.MinimizeToTray;

            Controller.SettingsUpdated();
        }

        private void openBoardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BoardsListBoxSelectedRowIndex != -1)
            {
                var path = (Model.Boards[BoardsListBoxSelectedRowIndex]).SaveTo;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                Utils.OpenDirectory(path);
            }
        }

        private void openBoardURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BoardsListBoxSelectedRowIndex != -1)
            {
                var url = boardsListBox.Items[BoardsListBoxSelectedRowIndex].ToString();
                Utils.OpenWebpage(url);
            }
        }

        private void deleteBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BoardsListBoxSelectedRowIndex != -1)
            {
                Controller.RemoveBoard((Board)boardsListBox.SelectedItem);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Utils.OpenDirectory(Settings.Default.SavePath);
        }

        private async void clearAllButton_Click(object sender, EventArgs e)
        {
            if (listsTabControl.SelectedIndex > 1)
            { 
                return;
            }

            await Controller.ClearTrackers(listsTabControl.SelectedIndex == 0 ? Type.Thread : Type.Board);
        }

        private void boardsListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int pos;

            if (e.Button == MouseButtons.Left)
            {
                if ((pos = boardsListBox.IndexFromPoint(e.Location)) != -1)
                {
                    var path = Model.Boards[pos].SaveTo;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    Utils.OpenDirectory(path);
                }
            }
        }

        /// <summary>
        /// Left click: Allows user to deselect all rows by clicking white space.
        /// Right click: Select the clicked row (not normally done with right click). Is necessary for future events so we know which row to operate on (e.g. remove tracker).
        /// </summary>
        private void threadGridView_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitInfo = threadGridView.HitTest(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (hitInfo.Type == DataGridViewHitTestType.None)
                {
                    threadGridView.ClearSelection();
                    threadGridView.CurrentCell = null;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (hitInfo.RowIndex != -1)
                {
                    threadGridView.ClearSelection();
                    threadGridView.Rows[hitInfo.RowIndex].Selected = true;
                    threadGridView.CurrentCell = threadGridView.Rows[hitInfo.RowIndex].Cells[0];
                }
            }
        }

        /// <summary>
        /// Copies threads/boards to clipboard depending on currently selected tab.
        /// </summary>
        private void clipboardButton_Click(object sender, EventArgs e)
        {
            string text;

            if (listsTabControl.SelectedIndex == 0)
            {
                text = string.Join(",", Model.Threads.Select(thread => thread.Url)).Replace("\n", "").Replace("\r", "");
            }
            else
            {
                text = string.Join(",", Model.Boards.Select(board => board.Url)).Replace("\n", "").Replace("\r", "");
            }

            Clipboard.SetText(text);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                Controller.RenameThreadSubjectPrompt(ThreadGridViewSelectedRowIndex);
            }
        }

        private void openLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utils.OpenDirectory(Program.LOGS_PATH);
        }

        /// <summary>
        /// Have to use this event because BalloonTipShown event doesn't fire for some reason.
        /// </summary>
        private void systemTrayNotifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            Utils.SetNotifyIconText(systemTrayNotifyIcon, Model.NotificationTrayTooltip);
        }

        private void systemTrayNotifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Visible)
                    Hide();
                else
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                    Activate();
                }
            }
        }

        private void systemTrayOpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string spath = Settings.Default.SavePath;

            if (!Directory.Exists(spath))
            {
                Directory.CreateDirectory(spath);
            }

            Utils.OpenDirectory(spath);
        }

        private void systemTrayExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (Settings.Default.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                // When minimized hide from taskbar if trayicon enabled
                Hide();
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateController.Instance.CheckForUpdates(true);
        }

        private void updateAvailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateController.Instance.ShowUpdateDialog();
        }

        internal async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = await Controller.Closing();
        }

        private void threadGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            logger.Error(e.Exception, $"A data error occured on the threadGridView. RowIndex: {e.RowIndex}, ColumnIndex: {e.ColumnIndex}, Context: {e.Context}.");
        }
    }
}