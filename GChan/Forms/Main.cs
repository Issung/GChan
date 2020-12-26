/************************************************************************
 * Copyright (C) 2015 by themirage <mirage@secure-mail.biz>             *
 *                                                                      *
 * This program is free software: you can redistribute it and/or modify *
 * it under the terms of the GNU General Public License as published by *
 * the Free Software Foundation, either version 3 of the License, or    *
 * (at your option) any later version.                                  *
 *                                                                      *
 * This program is distributed in the hope that it will be useful,      *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 * GNU General Public License for more details.                         *
 *                                                                      *
 * You should have received a copy of the GNU General Public License    *
 * along with this program.  If not, see <http://www.gnu.org/licenses/> *
 ************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using SysThread = System.Threading.Thread;
using GChan.Trackers;
using Thread = GChan.Trackers.Thread;
using Type = GChan.Trackers.Type;
using GChan.Models;
using GChan.Controllers;

namespace GChan
{
    public partial class MainForm : Form
    {
        internal MainFormModel Model;

        private SysThread Scanner = null;                                                      // thread that adds stuff

        /// <summary>
        /// Get the index of the selected row in the thread grid view.
        /// </summary>
        private int ThreadGridViewSelectedRowIndex => threadGridView?.CurrentCell?.RowIndex ?? -1;

        private int BoardsListBoxSelectedRowIndex { get { return boardsListBox.SelectedIndex; } set {boardsListBox.SelectedIndex = value;} }

        private System.Windows.Forms.Timer scanTimer = new System.Windows.Forms.Timer();     // Timer for scanning

        private object threadLock = new object();
        private object boardLock = new object();

        private enum URLType { Thread, Board };

        /// <summary>
        /// Flag for whether or not the last update check was initiated by the user.
        /// False for automatic program start-up update check.
        /// </summary>
        private bool updateCheckWasManual = false;

        public MainForm()
        {
            InitializeComponent();

            Model = new MainFormModel(this);
            mainFormModelBindingSource.DataSource = Model;

            UpdateController.Instance.UpdateCheckFinished += Instance_UpdateCheckFinished;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.MinimizeToTray)                            // If trayicon deactivated
                systemTrayNotifyIcon.Visible = false;                                                 // Hide it

            scanTimer.Enabled = false;                                                   // Disable timer
            scanTimer.Interval = Properties.Settings.Default.ScanTimer;                      // Set interval
            scanTimer.Tick += new EventHandler(Scan);                               // When Timer ticks call scan()

            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

            ///Require the save on close setting to be true to load threads on application open.
            const bool requireSaveOnCloseToBeTrueToLoadThreadsAndBoards = true;

            if (!requireSaveOnCloseToBeTrueToLoadThreadsAndBoards || Properties.Settings.Default.SaveListsOnClose)                                // If enabled load URLs from file
            {
                string boards = Utils.LoadURLs(true);
                string threads = Utils.LoadURLs(false);

                if (!String.IsNullOrWhiteSpace(boards))
                {
                    lock (boardLock)
                    {
                        string[] URLs = boards.Split('\n');
                        for (int i = 0; i < URLs.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(URLs[i]))
                            {
                                Board newBoard = (Board)Utils.CreateNewTracker(URLs[i].Trim());
                                AddURLToList(newBoard);
                            }
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(threads))
                {
                    lock (threadLock)
                    {
                        string[] URLs = threads.Split('\n');

                        new SysThread(() =>
                        {
                            // Without setting ScrollBars to None and then setting to Vertical the program will crash.
                            // It doesnt like items being added in parallel in this method...
                            // User cannot also resize columns while adding, or else program crashes.
                            // TODO: Just move these to outside of the thread call so they don't require an Invoke?

                            CheckForIllegalCrossThreadCalls = false;
                            Invoke((MethodInvoker)delegate { threadGridView.ScrollBars = ScrollBars.None; });
                            threadGridView.AllowUserToResizeColumns = false;

                            // END TODO

                            Parallel.ForEach(URLs, (url) =>
                            {
                                if (!string.IsNullOrWhiteSpace(url))
                                {
                                    Thread newThread = (Thread)Utils.CreateNewTracker(url.Trim());
                                    AddURLToList(newThread);
                                }
                            });

                            Invoke((MethodInvoker)delegate {
                                Done();
                            });
                        }).Start();
                    }
                }
                else
                {
                    Done();
                }
            }

            /// Executed once everything has finished being loaded.
            void Done()
            {
                threadGridView.ScrollBars = ScrollBars.Vertical;
                threadGridView.AllowUserToResizeColumns = true;

                scanTimer.Enabled = true;
                Scan(this, new EventArgs());

                // Check for updates.
                if (Properties.Settings.Default.CheckForUpdatesOnStart)
                { 
                    UpdateController.Instance.CheckForUpdates();
                }
            }
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

        #region Methods

        private void AddUrl(string url)
        {
            Tracker newTracker = Utils.CreateNewTracker(url);

            if (newTracker != null)
            {
                List<Tracker> trackerList = ((newTracker.Type == Type.Board) ? Model.Boards.Cast<Tracker>() : Model.Threads.Cast<Tracker>()).ToList();
                if (IsUnique(newTracker.URL, trackerList))
                {
                    AddURLToList(newTracker);

                    if (!scanTimer.Enabled)
                        scanTimer.Enabled = true;
                    if (Properties.Settings.Default.SaveListsOnClose)
                        Utils.SaveURLs(Model.Boards, Model.Threads);

                    Scan(this, new EventArgs());
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        "URL is already being tracked!\nOpen corresponding folder?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                    if (result == DialogResult.Yes)
                    {
                        string spath = newTracker.SaveTo;
                        if (!Directory.Exists(spath))
                            Directory.CreateDirectory(spath);
                        Process.Start(spath);
                    }
                }
            }
            else
            {
                MessageBox.Show("Corrupt URL, unsupported website or not a board/thread!");
                urlTextBox.Text = "";
            }
        }

        private bool AddURLToList(Tracker tracker)
        {
            if (tracker == null) 
                return false;

            if (tracker.Type == Type.Board)
            {
                lock (boardLock)
                {
                    Invoke((MethodInvoker)delegate () {
                        Model.Boards.Add((Board)tracker);
                    });
                }
            }
            else //Thread
            {
                //lock (threadLock)
                //{
                Invoke((MethodInvoker)delegate () {
                    Model.Threads.Add((Thread)tracker);
                });
                //}
            }

            return true;
        }

        private void Scan(object sender, EventArgs e)
        {
            if (Scanner == null || !Scanner.IsAlive)
            {
                Scanner = new SysThread(new ThreadStart(ScanThread))
                {
                    Name = "Scan Thread",
                    IsBackground = true
                };

                Scanner.Start();
            }
        }

        private void ScanThread()
        {
            //List<Imageboard> goneThreads = new List<Imageboard>();

            lock (threadLock)
            {
                // Removes 404'd threads
                Thread[] array = Model.Threads.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    Thread thread = array[i];
                    if (thread.Gone)
                    {
                        RemoveThread(thread);
                        //goneThreads.Add(thread);
                    }
                }

                if (Properties.Settings.Default.SaveListsOnClose)
                    Utils.SaveURLs(Model.Boards, Model.Threads.ToList());
            }

            lock (boardLock)
            {
                // Searches for new threads on the watched boards
                for (int i = 0; i < Model.Boards.Count; i++)
                {
                    string[] threads = { };

                    try
                    {
                        threads = Model.Boards[i].GetThreadLinks();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        for (int j = 0; j < threads.Length; j++)
                        {
                            Thread newThread = (Thread)Utils.CreateNewTracker(threads[j]);
                            if (newThread != null && IsUnique(newThread.URL, Model.Threads))
                            {
                                AddURLToList(newThread);
                            }
                        }
                    }
                }
            }

            lock (threadLock)
            {
                // Download threads
                for (int i = 0; i < Model.Threads.Count; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Model.Threads[i].Download));
                }
            }
        }

        /// <summary>
        /// Returns true if a url is not contained within a list of trackers. 
        /// </summary>
        private bool IsUnique(string url, IEnumerable<Tracker> list)
        {
            return !list.Any(t => t.URL == url);
        }

        private void RenameThreadSubjectPrompt(int threadBindingSourceIndex)
        {
            Thread thread = Model.Threads[threadBindingSourceIndex];
            GetStringMessageBox dialog = new GetStringMessageBox(thread.Subject);
            dialog.StartPosition = FormStartPosition.CenterParent;

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string newSubject = string.IsNullOrWhiteSpace(dialog.UserEntry) ? Thread.NO_SUBJECT : dialog.UserEntry;

                thread.SetSubject(newSubject);
            }
        }

        private void RemoveThread(Thread thread)
        {
            Program.Log(true, $"Removing thread {thread.URL}! thread.isGone: {thread.Gone}");

            if (Properties.Settings.Default.AddThreadSubjectToFolder)
            {
                string currentPath = thread.SaveTo.Replace("\r", "");

                string cleanSubject = Utils.CleanSubjectString(thread.Subject);

                // There are \r characters appearing from the custom subjects, TODO: need to get to the bottom of the cause of this.
                string destinationPath;

                if ((ThreadFolderNameFormat)Properties.Settings.Default.ThreadFolderNameFormat == ThreadFolderNameFormat.IdName)
                {
                    destinationPath = (thread.SaveTo + " - " + cleanSubject);
                }
                else //NameId
                {
                    destinationPath = Path.Combine(Path.GetDirectoryName(thread.SaveTo), $"{thread.Subject} - {thread.ID}");
                }
                
                destinationPath = destinationPath.Replace("\r", "").Trim('\\', '/');

                if (Directory.Exists(currentPath))
                {
                    int number = 0;
                    string numberText() => (number == 0) ? "" : $" ({number})";

                    //string calculatedDestination() => Path.GetDirectoryName(destinationPath) + "\\" + Path.GetFileName(destinationPath) + numberText();
                    string calculatedDestination() => destinationPath + numberText();

                    while (Directory.Exists(calculatedDestination()))
                    {
                        number++;
                    }

                    Program.Log(true, $"Directory.Moving {currentPath} to {destinationPath}");

                    Directory.Move(currentPath, calculatedDestination());
                }
                else
                {
                    Program.Log(true, $"While attempting to rename thread {thread.URL} the current folder could not be found, renaming abandoned.");
                }
            }

            Invoke((MethodInvoker)delegate () {
                Model.Threads.Remove(thread);
            });
        }

        #endregion

        #region Events

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
            // This way, it doesn't flash text during lazy entry
            string textBox = urlTextBox.Text; 

            // Clear TextBox faster
            urlTextBox.Text = "";

            if (string.IsNullOrWhiteSpace(textBox) && Clipboard.ContainsText() && Properties.Settings.Default.AddUrlFromClipboardWhenTextboxEmpty)
                textBox = Clipboard.GetText();

            // Get url from TextBox
            var urls = textBox.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < urls.Length; i++)
            {
                urls[i] = Utils.PrepareURL(urls[i]);
                AddUrl(urls[i]);
            }
        }

        private void urlTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string entry = (string)e.Data.GetData(DataFormats.Text);               // Get url from drag and drop

            // Get url from TextBox
            var urls = entry.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < urls.Length; i++)
            {
                urls[i] = Utils.PrepareURL(urls[i]);
                AddUrl(urls[i]);
            }
        }

        private void urlTextBox_DragEnter(object sender, DragEventArgs e)
        {
            // See if this is a copy and the data includes text.
            if (e.Data.GetDataPresent(DataFormats.Text) && (e.AllowedEffect & DragDropEffects.Copy) != 0)
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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                lock (threadLock)
                {
                    try
                    {
                        RemoveThread(Model.Threads[ThreadGridViewSelectedRowIndex]);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                string spath = Model.Threads[ThreadGridViewSelectedRowIndex].SaveTo;
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                string spath = Model.Threads[ThreadGridViewSelectedRowIndex].URL;
                Process.Start(spath);
            }
        }

        private void copyURLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                string spath = Model.Threads[ThreadGridViewSelectedRowIndex].URL;
                Clipboard.SetText(spath);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox tAbout = new AboutBox();
            tAbout.ShowDialog();
            tAbout.Dispose();
        }

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Changelog tvInf = new Changelog();
            tvInf.ShowDialog();
            tvInf.Dispose();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings tSettings = new Settings();
            tSettings.ShowDialog();
            tSettings.Dispose();
            if (Properties.Settings.Default.MinimizeToTray)
                systemTrayNotifyIcon.Visible = true;
            else
                systemTrayNotifyIcon.Visible = false;

            scanTimer.Interval = Properties.Settings.Default.ScanTimer;
        }

        private void openBoardFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BoardsListBoxSelectedRowIndex != -1)
            {
                string spath = (Model.Boards[BoardsListBoxSelectedRowIndex]).SaveTo;
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openBoardURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BoardsListBoxSelectedRowIndex != -1)
            {
                string spath = boardsListBox.Items[BoardsListBoxSelectedRowIndex].ToString();
                Process.Start(spath);
            }
        }

        private void deleteBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BoardsListBoxSelectedRowIndex != -1)
            {
                lock (boardLock)
                {
                    Model.Boards.RemoveAt(BoardsListBoxSelectedRowIndex);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.SavePath);
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            if (listsTabControl.SelectedIndex > 1) 
                return;
            
            bool thread = (listsTabControl.SelectedIndex == 0);                             // Board Tab is open -> board=true; Thread tab -> board=false

            string type;

            if (thread)
                type = "threads";
            else
                type = "boards";

            DialogResult dialogResult = MessageBox.Show(
                "Are you sure you want to clear all " + type + "?",
                "Clear all " + type,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);    // Confirmation prompt

            if (dialogResult == DialogResult.Yes)
            {
                if (thread)
                {
                    lock (threadLock)
                    {
                        while (Model.Threads.Count > 0)
                        {
                            RemoveThread(Model.Threads[0]);
                        }
                    
                    }
                }
                else
                {
                    lock (boardLock)
                    { 
                        Model.Boards.Clear();
                    }
                }

                if (Properties.Settings.Default.SaveListsOnClose)
                    Utils.SaveURLs(Model.Boards, Model.Threads.ToList());
            }
        }

        private void boardsListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int pos;

            if (e.Button == MouseButtons.Left)
            {
                if ((pos = boardsListBox.IndexFromPoint(e.Location)) != -1)
                {
                    string spath = Model.Boards[pos].SaveTo;
                    if (!Directory.Exists(spath))
                        Directory.CreateDirectory(spath);
                    Process.Start(spath);
                }
            }
        }

        /// <summary>
        /// Quality of life features.
        ///     Left click allows user to deselect all rows by clicking white space. 
        ///     Right click highlights the clicked row (otherwise it would not be done, just looks nicer with the context menu).
        ///         But this is also 100% necessary because for the context menu click events we must know which row is selected.
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
                if (e.Button == MouseButtons.Right)
                {
                    if (hitInfo.RowIndex != -1)
                    {
                        threadGridView.ClearSelection();
                        threadGridView.Rows[hitInfo.RowIndex].Selected = true;
                        threadGridView.CurrentCell = threadGridView.Rows[hitInfo.RowIndex].Cells[0];
                        threadsContextMenu.Show(Cursor.Position);
                    }
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
                text = String.Join(",", Model.Threads.Select(thread => thread.URL)).Replace("\n", "").Replace("\r", "");
            }
            else
            {
                text = String.Join(",", Model.Boards.Select(board => board.URL)).Replace("\n", "").Replace("\r", "");
            }

            Clipboard.SetText(text);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                RenameThreadSubjectPrompt(ThreadGridViewSelectedRowIndex);
            }
        }

        private void openProgramDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.CommonAppDataPath);
        }

        /// <summary>
        /// Have to use this event because BalloonTipShown event doesn't fire for some reason.
        /// </summary>
        private void systemTrayNotifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            //systemTrayNotifyIcon.Text = Model.NotificationTrayTooltip;
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
            string spath = Properties.Settings.Default.SavePath;
            if (!Directory.Exists(spath))
                Directory.CreateDirectory(spath);
            Process.Start(spath);
        }

        private void systemTrayExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                // When minimized hide from taskbar if trayicon enabled
                Hide();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = DialogResult.OK;

            if (Properties.Settings.Default.WarnOnClose && Model.Threads.Count > 0)
            {
                CloseWarn clw = new CloseWarn();
                result = clw.ShowDialog();
                clw.Dispose();
                e.Cancel = (result == DialogResult.Cancel);
            }

            if (result == DialogResult.OK)
            {
                systemTrayNotifyIcon.Visible = false;
                systemTrayNotifyIcon.Dispose();
                scanTimer.Enabled = false;

                if (Properties.Settings.Default.SaveListsOnClose)
                    Utils.SaveURLs(Model.Boards, Model.Threads.ToList());
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateCheckWasManual = true;
            UpdateController.Instance.CheckForUpdates();
        }

        private void Instance_UpdateCheckFinished(object sender, Onova.Models.CheckForUpdatesResult result)
        {
            if (result.CanUpdate)
            {
                updateAvailableToolStripMenuItem.Visible = true;
            }
            else
            {
                // Only show notification if the update check was initiated by the user.
                if (updateCheckWasManual)
                    toolTip.Show("No Updates Available.", menuStrip, 70, 20, 1750);

                updateCheckWasManual = false;
            }
        }

        private void updateAvailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open UpdateInfoForm as Dialog.
        }

        #endregion
    }
}