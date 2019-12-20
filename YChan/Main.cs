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
using System.Threading;
using System.Windows.Forms;

namespace GChan
{
    public partial class frmMain : Form
    {
        public List<Imageboard> ThreadList { get; set; } = new List<Imageboard>();
        public List<Imageboard> ListBoards { get; set; } = new List<Imageboard>();
        private Thread Scanner = null;                                                      // thread that adds stuff

        private int threadIndex = -1;                                                              // Item position in threadGridView
        private int bPos = -1;                                                              // Item position in lbBoards
        private System.Windows.Forms.Timer scnTimer = new System.Windows.Forms.Timer();     // Timer for scanning

        private object threadLock = new object();
        private object boardLock = new object();

        private enum URLType { Thread, Board };

        public frmMain()
        {
            InitializeComponent();

            threadGridView.AutoGenerateColumns = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.minimizeToTray)                            // If trayicon deactivated
                nfTray.Visible = false;                                                 // Hide it

            scnTimer.Enabled = false;                                                   // Disable timer
            scnTimer.Interval = Properties.Settings.Default.timer;                      // Set interval
            scnTimer.Tick += new EventHandler(this.Scan);                               // When Timer ticks call scan()

            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

            if (Properties.Settings.Default.firstStart)
            {
                FirstStart tFirstStart = new FirstStart();                              // If first start, show first start message
                tFirstStart.ShowDialog();
                Properties.Settings.Default.firstStart = false;
                Properties.Settings.Default.Save();
            }

            ///Require the save on close setting to be true to load threads on application open.
            const bool requireSaveOnCloseToBeTrueToLoadThreadsAndBoards = false;

            if (!requireSaveOnCloseToBeTrueToLoadThreadsAndBoards || Properties.Settings.Default.saveOnClose)                                // If enabled load URLs from file
            {
                string boards = General.LoadURLs(true);
                string threads = General.LoadURLs(false);                               // Load threads

                if (!String.IsNullOrWhiteSpace(boards))
                {
                    lock (boardLock)
                    {
                        string[] URLs = boards.Split('\n');
                        for (int i = 0; i < URLs.Length - 1; i++)
                        {
                            Imageboard newImageboard = General.CreateNewImageboard(URLs[i]);    // and add them
                            AddURLToList(newImageboard);
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(threads))
                {
                    lock (threadLock)
                    {
                        string[] URLs = threads.Split('\n');
                        for (int i = 0; i < URLs.Length - 1; i++)
                        {
                            Imageboard newImageboard = General.CreateNewImageboard(URLs[i]);
                            AddURLToList(newImageboard);
                        }
                    }
                }

                lbBoards.DataSource = ListBoards;
                threadGridView.DataSource = ThreadList;

                scnTimer.Enabled = true;                                        // Activate the timer
                Scan(this, new EventArgs());                                    // and start scanning
            }
        }

        private bool AddURLToList(Imageboard imageboard)
        {
            if (imageboard == null) return false;

            if (imageboard.isBoard())
            {
                lock (boardLock)
                {
                    ListBoards.Add(imageboard);
                    updateDataSource(URLType.Board);
                }
            }
            else
            {
                lock (threadLock)
                {
                    ThreadList.Add(imageboard);
                    updateDataSource(URLType.Thread);
                }
            }

            return true;
        }

        private void AddUrl(string url)
        {
            Imageboard newImageboard = General.CreateNewImageboard(url);

            if (newImageboard != null)
            {
                if (isUnique(newImageboard.getURL(), newImageboard.isBoard() ? ListBoards : ThreadList))
                {
                    AddURLToList(newImageboard);

                    if (!scnTimer.Enabled)
                        scnTimer.Enabled = true;
                    if (Properties.Settings.Default.saveOnClose)
                        General.SaveURLs(ListBoards, ThreadList);

                    Scan(this, new EventArgs());
                }
                else
                {
                    DialogResult result = MessageBox.Show("URL is already in queue!\nOpen corresponding folder?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Get url from TextBox
            var urls = URLTextBox.Text.Split(',');

            for (int i = 0; i < urls.Length; i++)
            { 
                urls[i] = General.PrepareURL(urls[i]);
                AddUrl(urls[i]);
            }

            // Clear TextBox
            URLTextBox.Text = "";
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
            for (int i = 0; i < ThreadList.Count; i++)
            {
                if (ThreadList[i].getURL() == url)
                    plc = i;
            }
            return plc;
        }

        private void lbBoards_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                bPos = -1;
                if (lbBoards.IndexFromPoint(e.Location) != -1)
                {
                    bPos = lbBoards.IndexFromPoint(e.Location);
                    cmBoards.Show(lbBoards, new Point(e.X, e.Y));
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadIndex != -1)
            {
                lock (threadLock)
                {
                    try
                    {
                        /*if (Properties.Settings.Default.addThreadSubjectToFolder)
                            Directory.Move(ThreadList[threadIndex].GetPath(), ThreadList[threadIndex].GetPath() + " - " + ThreadList[threadIndex].Subject);*/

                        /*if (Properties.Settings.Default.addThreadSubjectToFolder)
                        {
                            string currentPath = ThreadList[threadIndex].GetPath();
                            string destPath = ThreadList[threadIndex].GetPath() + " " + ThreadList[threadIndex].Subject;
                            Program.Log(true, $"Attempting to rename folder because addThreadSubjectToFolder is enabled.",
                                $"Moving {currentPath} to {destPath}");
                            Directory.Move(currentPath, destPath);
                        }*/
                        RemoveThread(ThreadList[threadIndex]);
                    }
                    catch
                    {

                    }
                    updateDataSource(URLType.Thread);
                }
            }
        }

        private void updateDataSource(URLType type)
        {
            if (type == URLType.Board)
            {
                lbBoards.Invoke((MethodInvoker)(() =>
                {
                    tpBoard.Text = $"Boards ({ListBoards.Count})";
                    lbBoards.DataSource = null;
                    lbBoards.DataSource = ListBoards;
                }));
            }
            else
            {
                threadGridView.Invoke((MethodInvoker)(() =>
                {
                    tpThreads.Text = $"Threads ({ThreadList.Count})";
                    threadGridView.DataSource = null;
                    threadGridView.DataSource = ThreadList;
                }));
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadIndex != -1)
            {
                string spath = ThreadList[threadIndex].GetPath();
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadIndex != -1)
            {
                string spath = ((Imageboard)ThreadList[threadIndex]).getURL();
                Process.Start(spath);
            }
        }

        private void copyURLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadIndex != -1)
            {
                string spath = ((Imageboard)ThreadList[threadIndex]).getURL();
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

            scnTimer.Interval = Properties.Settings.Default.timer;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = DialogResult.OK;

            if (Properties.Settings.Default.warnOnClose && ThreadList.Count > 0)
            {
                CloseWarn clw = new CloseWarn();
                result = clw.ShowDialog();
                e.Cancel = (result == DialogResult.Cancel);
            }

            if (result == DialogResult.OK)
            {
                nfTray.Visible = false;
                nfTray.Dispose();
                scnTimer.Enabled = false;

                if (Properties.Settings.Default.saveOnClose)
                    General.SaveURLs(ListBoards, ThreadList);
            }
        }

        private void openBoardFolderToolTip_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                string spath = ListBoards[bPos].GetPath();
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openBoardURLToolTip_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                string spath = lbBoards.Items[bPos].ToString();
                Process.Start(spath);
            }
        }

        private void deleteBoardToolTip_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                lock (boardLock)
                {
                    ListBoards.RemoveAt(bPos);
                    updateDataSource(URLType.Board);
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
            List<Imageboard> goneThreads = new List<Imageboard>();

            lock (threadLock)
            {
                // Removes 404'd threads
                foreach (Imageboard thread in ThreadList.ToArray())
                { 
                    if (thread.isGone())
                    {
                        goneThreads.Add(thread);
                    }
                }

                if (Properties.Settings.Default.saveOnClose)
                    General.SaveURLs(ListBoards, ThreadList);
            }

            foreach (Imageboard thread in goneThreads)
            {
                RemoveThread(thread);
            }

            updateDataSource(URLType.Thread);

            lock (boardLock)
            {
                // Searches for new threads on the watched boards
                foreach (Imageboard board in ListBoards)
                {
                    string[] Threads = { };

                    try
                    {
                        Threads = board.getThreads();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        foreach (string thread in Threads)
                        {
                            Imageboard newImageboard = General.CreateNewImageboard(thread);
                            if (newImageboard != null && isUnique(newImageboard.getURL(), ThreadList))
                            {
                                AddURLToList(newImageboard);
                            }
                        }
                    }
                }
            }

            updateDataSource(URLType.Board);

            lock (threadLock)
            {
                // Download threads
                for (int i = 0; i < ThreadList.Count; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadList[i].download));
                }
            }
        }

        private void RemoveThread(Imageboard thread)
        {
            Program.Log(true, $"Removing thread! {thread.BoardName}/{thread.ID}/{thread.Subject} Gone: {thread.isGone()}");

            if (Properties.Settings.Default.addThreadSubjectToFolder)
            {
                string currentPath = thread.GetPath();

                // There are \r characters appearing from the custom subjects, TODO: need to get to the bottom of the cause of this.
                string destPath = (thread.GetPath() + " - " + thread.Subject).Replace("\r", ""); 

                Program.Log(true, "Removing thread and attempting to rename folder because addThreadSubjectToFolder is enabled.",
                    $"Directory.Moving {currentPath} to {destPath}");

                Directory.Move(currentPath, destPath);
            }

            ThreadList.Remove(thread);
            updateDataSource(URLType.Thread);
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
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
                    ListBoards.Clear();
                    updateDataSource(URLType.Board);
                }
                else
                {
                    ThreadList.Clear();
                    updateDataSource(URLType.Thread);
                }

                if (Properties.Settings.Default.saveOnClose)
                    General.SaveURLs(ListBoards, ThreadList);
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

        /*private void lbThreads_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int pos;
            if (e.Button == MouseButtons.Left)
            {
                if ((pos = lbThreads.IndexFromPoint(e.Location)) != -1)
                {
                    string spath = ListThreads[pos].getPath();
                    if (!Directory.Exists(spath))
                        Directory.CreateDirectory(spath);
                    Process.Start(spath);
                }
            }
        }*/

        private void lbBoards_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int pos;
            if (e.Button == MouseButtons.Left)
            {
                if ((pos = lbBoards.IndexFromPoint(e.Location)) != -1)
                {
                    string spath = ListBoards[pos].GetPath();
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
                int pos = lbBoards.SelectedIndex;

                if (pos > -1)
                {
                    lock (boardLock)
                    {
                        ListBoards.RemoveAt(pos);
                        updateDataSource(URLType.Board);
                    }
                }
            }
        }

        /*private void ThreadGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int pos = threadGridView.SelectedRows;

                if (pos > -1)
                {
                    lock (threadLock)
                    {
                        ListThreads.RemoveAt(pos);
                        updateDataSource(URLType.Thread);
                    }
                }
            }
        }*/

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
                    threadIndex = e.RowIndex;
                    cmThreads.Show(Cursor.Position);
                }
            }
        }

        private void clipboardButton_Click(object sender, EventArgs e)
        {
            string text = String.Join(",", ThreadList.Select(thread => thread.getURL())).Replace("\n", "").Replace("\r", "");
            Clipboard.SetText(text);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadIndex != -1)
            {
                lock (threadLock)
                {
                    string currentSubject = ThreadList[threadIndex].Subject;
                    string entry = General.MessageBoxGetString(currentSubject, Left + 50, Top + 50);

                    if (entry.Length < 1)
                    {
                        ThreadList[threadIndex].SetCustomSubject(Imageboard.NO_SUBJECT);
                    }
                    else
                    {
                        ThreadList[threadIndex].SetCustomSubject(entry);
                    }

                    updateDataSource(URLType.Thread);
                }
            }
        }

        private void openProgramDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.CommonAppDataPath);
        }

        private void downloadNowToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}