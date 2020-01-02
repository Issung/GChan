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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GChan.Controls;

namespace GChan
{
    public partial class MainForm : Form
    {
        public SortableBindingList<Imageboard> ThreadListBindingSource;

        public List<Imageboard> BoardList { get; set; } = new List<Imageboard>();
        public BindingSource BoardListBindingSource;

        private Thread Scanner = null;                                                      // thread that adds stuff

        //private int threadIndex = -1;     // Item position in threadGridView

        /// <summary>
        /// Get the index of the selected row in the thread grid view.
        /// </summary>
        private int ThreadGridViewSelectedRowIndex { get { return threadGridView.CurrentCell.RowIndex; } }

        private int bPos = -1;                                                               // Item position in lbBoards
        private System.Windows.Forms.Timer scanTimer = new System.Windows.Forms.Timer();     // Timer for scanning

        private object threadLock = new object();
        private object boardLock = new object();

        private enum URLType { Thread, Board };

        public MainForm()
        {
            InitializeComponent();

            threadGridView.AutoGenerateColumns = false;

            ThreadListBindingSource = new SortableBindingList<Imageboard>();
            ThreadListBindingSource.ListChanged += ThreadListBindingSource_ListChanged;
            threadGridView.DataSource = ThreadListBindingSource;

            BoardListBindingSource = new BindingSource();
            BoardListBindingSource.DataSource = BoardList;
            BoardListBindingSource.ListChanged += BoardListBindingSource_ListChanged;
            boardsListBox.DataSource = BoardListBindingSource;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.minimizeToTray)                            // If trayicon deactivated
                nfTray.Visible = false;                                                 // Hide it

            scanTimer.Enabled = false;                                                   // Disable timer
            scanTimer.Interval = Properties.Settings.Default.timer;                      // Set interval
            scanTimer.Tick += new EventHandler(this.Scan);                               // When Timer ticks call scan()

            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

            if (Properties.Settings.Default.firstStart)
            {
                FirstStart tFirstStart = new FirstStart();                              // If first start, show first start message
                tFirstStart.ShowDialog();
                Properties.Settings.Default.firstStart = false;
                Properties.Settings.Default.Save();
            }

            ///Require the save on close setting to be true to load threads on application open.
            const bool requireSaveOnCloseToBeTrueToLoadThreadsAndBoards = true;

            if (!requireSaveOnCloseToBeTrueToLoadThreadsAndBoards || Properties.Settings.Default.saveOnClose)                                // If enabled load URLs from file
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
                                Imageboard newImageboard = Utils.CreateNewImageboard(URLs[i].Trim());
                                AddURLToList(newImageboard);
                            }
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(threads))
                {
                    lock (threadLock)
                    {
                        string[] URLs = threads.Split('\n');

                        new Thread(() =>
                        {
                            // Without setting ScrollBars to None and then setting to Vertical the program will crash.
                            // It doesnt like items being added in parallel in this method...
                            // User cannot also resize columns while adding, or else program crashes.

                            CheckForIllegalCrossThreadCalls = false;
                            this.Invoke((MethodInvoker)delegate { threadGridView.ScrollBars = ScrollBars.None; });
                            threadGridView.AllowUserToResizeColumns = false;

                            ParallelOptions options = new ParallelOptions { 
                                MaxDegreeOfParallelism = Environment.ProcessorCount 
                            };

                            Parallel.ForEach(URLs, options, (url) =>
                            {
                                if (!string.IsNullOrWhiteSpace(url))
                                {
                                    Imageboard newImageboard = Utils.CreateNewImageboard(url.Trim());
                                    AddURLToList(newImageboard);
                                }
                            });

                            this.Invoke((MethodInvoker)delegate {
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

            ///Executed once everything has finished being loaded.
            void Done()
            {
                threadGridView.ScrollBars = ScrollBars.Vertical;
                threadGridView.AllowUserToResizeColumns = true;

                scanTimer.Enabled = true;
                Scan(this, new EventArgs());
            }
        }

        #region Methods

        private void AddUrl(string url)
        {
            Imageboard newImageboard = Utils.CreateNewImageboard(url);

            if (newImageboard != null)
            {
                if (isUnique(newImageboard.getURL(), newImageboard.isBoard() ? BoardList : ThreadListBindingSource.ToList()))
                {
                    AddURLToList(newImageboard);

                    if (!scanTimer.Enabled)
                        scanTimer.Enabled = true;
                    if (Properties.Settings.Default.saveOnClose)
                        Utils.SaveURLs(BoardList, ThreadListBindingSource.ToList());

                    Scan(this, new EventArgs());
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        "URL is already being tracked!\nOpen corresponding folder?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                    if (result == DialogResult.Yes)
                    {
                        string spath = newImageboard.GetPath();
                        if (!Directory.Exists(spath))
                            Directory.CreateDirectory(spath);
                        Process.Start(spath);
                    }
                }
            }
            else
            {
                MessageBox.Show("Corrupt URL, unsupported website or not a board/thread!");
                URLTextBox.Text = "";
            }
        }

        private bool AddURLToList(Imageboard imageboard)
        {
            if (imageboard == null) return false;

            if (imageboard.isBoard())
            {
                lock (boardLock)
                {
                    this.Invoke((MethodInvoker)delegate () {
                        BoardListBindingSource.Add(imageboard);
                    });
                }
            }
            else
            {
                //lock (threadLock)
                //{
                this.Invoke((MethodInvoker)delegate () {
                    ThreadListBindingSource.Add(imageboard);
                });
                //}
            }

            return true;
        }

        private void Scan(object sender, EventArgs e)
        {
            if (Scanner == null || !Scanner.IsAlive)
            {
                Scanner = new Thread(new ThreadStart(ScanThread))
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
                foreach (Imageboard thread in ThreadListBindingSource.ToArray())
                {
                    if (thread.isGone())
                    {
                        RemoveThread(thread);
                        //goneThreads.Add(thread);
                    }
                }

                if (Properties.Settings.Default.saveOnClose)
                    Utils.SaveURLs(BoardList, ThreadListBindingSource.ToList());
            }

            lock (boardLock)
            {
                // Searches for new threads on the watched boards
                foreach (Imageboard board in BoardList)
                {
                    string[] threads = { };

                    try
                    {
                        threads = board.getThreads();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        foreach (string thread in threads)
                        {
                            Imageboard newImageboard = Utils.CreateNewImageboard(thread);
                            if (newImageboard != null && isUnique(newImageboard.getURL(), ThreadListBindingSource.ToList()))
                            {
                                AddURLToList(newImageboard);
                            }
                        }
                    }
                }
            }

            lock (threadLock)
            {
                // Download threads
                for (int i = 0; i < ThreadListBindingSource.Count; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadListBindingSource[i].download));
                }
            }
        }

        private bool isUnique(string url, List<Imageboard> List)
        {
            bool flag = true;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].getURL() == url)
                    flag = false;
            }
            return flag;
        }

        private int getPlace(string url)
        {
            int plc = -1;
            for (int i = 0; i < ThreadListBindingSource.Count; i++)
            {
                if (ThreadListBindingSource[i].getURL() == url)
                    plc = i;
            }
            return plc;
        }

        private void RenameThreadSubjectPrompt(int threadBindingSourceIndex)
        {
            lock (threadLock)
            {
                string currentSubject = (ThreadListBindingSource[threadBindingSourceIndex] as Imageboard).Subject;
                string entry = Utils.MessageBoxGetString(currentSubject, Left + 50, Top + 50);

                if (entry.Length < 1)
                {
                    (ThreadListBindingSource[threadBindingSourceIndex] as Imageboard).SetCustomSubject(Imageboard.NO_SUBJECT);
                }
                else
                {
                    (ThreadListBindingSource[threadBindingSourceIndex] as Imageboard).SetCustomSubject(entry);
                }
            }
        }

        private void RemoveThread(Imageboard thread)
        {
            Program.Log(true, $"Removing thread {thread.getURL()}! thread.isGone: {thread.isGone()}");

            if (Properties.Settings.Default.addThreadSubjectToFolder)
            {
                string currentPath = thread.GetPath().Replace("\r", "");

                string cleanSubject = Utils.CleanThreadSubjectForFolderName(thread.Subject);
                 
                // There are \r characters appearing from the custom subjects, TODO: need to get to the bottom of the cause of this.
                string destinationPath = (thread.GetPath() + " - " + cleanSubject).Replace("\r", "");
                destinationPath = destinationPath.Trim('\\', '/');

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
                    Program.Log(true, $"While attempting to rename thread {thread.getURL()} the current folder could not be found, renaming abandoned.");
                }
            }

            ThreadListBindingSource.Remove(thread);
        }

        #endregion

        #region Events

        private void ThreadListBindingSource_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            tpThreads.Text = $"Threads ({ThreadListBindingSource.Count})";
        }

        private void BoardListBindingSource_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            tpBoard.Text = $"Boards ({BoardList.Count})";
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // Get url from TextBox
            var urls = URLTextBox.Text.Split(',');

            for (int i = 0; i < urls.Length; i++)
            {
                urls[i] = Utils.PrepareURL(urls[i]);
                AddUrl(urls[i]);
            }

            // Clear TextBox
            URLTextBox.Text = "";
        }

        private void lbBoards_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                bPos = -1;
                if (boardsListBox.IndexFromPoint(e.Location) != -1)
                {
                    bPos = boardsListBox.IndexFromPoint(e.Location);
                    cmBoards.Show(boardsListBox, new Point(e.X, e.Y));
                }
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
                        RemoveThread(ThreadListBindingSource[ThreadGridViewSelectedRowIndex] as Imageboard);
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
                string spath = (ThreadListBindingSource[ThreadGridViewSelectedRowIndex] as Imageboard).GetPath();
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                string spath = (ThreadListBindingSource[ThreadGridViewSelectedRowIndex] as Imageboard).getURL();
                Process.Start(spath);
            }
        }

        private void copyURLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                string spath = ThreadListBindingSource[ThreadGridViewSelectedRowIndex].getURL();
                Clipboard.SetText(spath);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox tAbout = new AboutBox();
            tAbout.ShowDialog();
        }

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Changelog tvInf = new Changelog();
            tvInf.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings tSettings = new Settings();
            tSettings.ShowDialog();
            if (Properties.Settings.Default.minimizeToTray)
                nfTray.Visible = true;
            else
                nfTray.Visible = false;

            scanTimer.Interval = Properties.Settings.Default.timer;
        }

        private void openBoardFolderToolTip_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                string spath = (BoardListBindingSource[bPos] as Imageboard).GetPath();
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openBoardURLToolTip_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                string spath = boardsListBox.Items[bPos].ToString();
                Process.Start(spath);
            }
        }

        private void deleteBoardToolTip_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                lock (boardLock)
                {
                    BoardListBindingSource.RemoveAt(bPos);
                }
            }
        }

        private void nfTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
                this.Hide();
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                Activate();
            }
        }

        private void cmTrayExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmTrayOpen_Click(object sender, EventArgs e)
        {
            string spath = Properties.Settings.Default.path;
            if (!Directory.Exists(spath))
                Directory.CreateDirectory(spath);
            Process.Start(spath);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.minimizeToTray && this.WindowState == FormWindowState.Minimized)
            {
                // When minimized hide from taskbar if trayicon enabled
                this.Hide();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.path);
        }

        private void edtURL_DragEnter(object sender, DragEventArgs e)
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

        private void edtURL_DragDrop(object sender, DragEventArgs e)
        {
            string url = (string)e.Data.GetData(DataFormats.Text);               // Get url from drag and drop
            AddUrl(url);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (tcApp.SelectedIndex > 1) return;
            bool board = (tcApp.SelectedIndex == 1);                             // Board Tab is open -> board=true; Thread tab -> board=false

            string type = "threads";
            if (board) type = "boards";

            DialogResult dialogResult = MessageBox.Show(
                "Are you sure you want to clear all " + type + "?",
                "Clear all " + type,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);    // Confirmation prompt

            if (dialogResult == DialogResult.Yes)
            {
                if (board)
                {
                    BoardListBindingSource.Clear();
                }
                else
                {
                    ThreadListBindingSource.Clear();
                }

                if (Properties.Settings.Default.saveOnClose)
                    Utils.SaveURLs(BoardList, ThreadListBindingSource.ToList());
            }
        }

        private void lbBoards_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int pos;
            if (e.Button == MouseButtons.Left)
            {
                if ((pos = boardsListBox.IndexFromPoint(e.Location)) != -1)
                {
                    string spath = (BoardListBindingSource[pos] as Imageboard).GetPath();
                    if (!Directory.Exists(spath))
                        Directory.CreateDirectory(spath);
                    Process.Start(spath);
                }
            }
        }

        private void lbBoards_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int pos = boardsListBox.SelectedIndex;

                if (pos > -1)
                {
                    lock (boardLock)
                    {
                        BoardListBindingSource.RemoveAt(pos);
                    }
                }
            }
        }

        private void threadGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DataGridView.HitTestInfo hit = threadGridView.HitTest(e.X, e.Y);

                if (hit.Type == DataGridViewHitTestType.None)
                {
                    threadGridView.ClearSelection();
                    threadGridView.CurrentCell = null;
                }
            }
        }

        private void threadGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex != -1)
                {
                    threadGridView.ClearSelection();
                    threadGridView.Rows[e.RowIndex].Selected = true;
                    threadGridView.CurrentCell = threadGridView.Rows[e.RowIndex].Cells[0];
                    //threadIndex = e.RowIndex;
                    cmThreads.Show(Cursor.Position);
                }
            }
        }

        private void clipboardButton_Click(object sender, EventArgs e)
        {
            string text = String.Join(",", ThreadListBindingSource.Select(thread => thread.getURL())).Replace("\n", "").Replace("\r", "");
            Clipboard.SetText(text);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ThreadGridViewSelectedRowIndex != -1)
            {
                RenameThreadSubjectPrompt(ThreadGridViewSelectedRowIndex);
            }
        }

        private void threadGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                RenameThreadSubjectPrompt(threadGridView.CurrentCell.RowIndex);
            }
        }

        private void openProgramDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.CommonAppDataPath);
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = DialogResult.OK;

            if (Properties.Settings.Default.warnOnClose && ThreadListBindingSource.Count > 0)
            {
                CloseWarn clw = new CloseWarn();
                result = clw.ShowDialog();
                e.Cancel = (result == DialogResult.Cancel);
            }

            if (result == DialogResult.OK)
            {
                nfTray.Visible = false;
                nfTray.Dispose();
                scanTimer.Enabled = false;

                if (Properties.Settings.Default.saveOnClose)
                    Utils.SaveURLs(BoardList, ThreadListBindingSource.ToList());
            }
        }

        #endregion
    }
}