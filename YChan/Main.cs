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
using System.Threading;
using System.Windows.Forms;

namespace YChan
{
    public partial class frmMain : Form
    {
        public List<Imageboard> ListThreads { get; set; } = new List<Imageboard>();
        public List<Imageboard> ListBoards { get; set; } = new List<Imageboard>();
        private Thread Scanner = null;                                                      // thread that addes stuff

        private int tPos = -1;                                                              // Item position in lbThreads
        private int bPos = -1;                                                              // Item position in lbBoards
        private System.Windows.Forms.Timer scnTimer = new System.Windows.Forms.Timer();     // Timmer for scanning

        private object threadLock = new object();
        private object boardLock = new object();

        private enum typeURL
        { thread, board };

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.minimizeToTray)                            // if trayicon deactivated
                nfTray.Visible = false;                                                 // hide it
            scnTimer.Enabled = false;                                                   // disable timer
            scnTimer.Interval = Properties.Settings.Default.timer;                      // set interval
            scnTimer.Tick += new EventHandler(this.scan);                               // when Timer ticks call scan()
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

            if (Properties.Settings.Default.firstStart)
            {
                FirstStart tFirstStart = new FirstStart();                              // if first start, show first start message
                tFirstStart.ShowDialog();
                Properties.Settings.Default.firstStart = false;
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.saveOnClose)                                // if enabled load URLs from file
            {
                string boards = General.LoadURLs(true);
                string threads = General.LoadURLs(false);                               // load threads

                if (!String.IsNullOrWhiteSpace(boards))
                {
                    lock (boardLock)
                    {
                        string[] URLs = boards.Split('\n');
                        for (int i = 0; i < URLs.Length - 1; i++)
                        {
                            Imageboard newImageboard = General.CreateNewImageboard(URLs[i]); // and add them
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
                lbThreads.DataSource = ListThreads;

                scnTimer.Enabled = true;                                        // activate the timer
                scan(this, new EventArgs());                                    // and start scanning
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
                    updateDataSource(typeURL.board);
                }
            }
            else
            {
                lock (threadLock)
                {
                    ListThreads.Add(imageboard);
                    updateDataSource(typeURL.thread);
                }
            }

            return true;
        }

        private void AddUrl(string url)
        {
            Imageboard newImageboard = General.CreateNewImageboard(url);

            if (newImageboard != null)
            {
                if (isUnique(newImageboard.getURL(), newImageboard.isBoard() ? ListBoards : ListThreads))
                {
                    AddURLToList(newImageboard);

                    if (!scnTimer.Enabled)
                        scnTimer.Enabled = true;
                    if (Properties.Settings.Default.saveOnClose)
                        General.WriteURLs(ListBoards, ListThreads);

                    scan(this, new EventArgs());
                }
                else
                {
                    DialogResult result = MessageBox.Show("URL is already in queue!\nOpen corresponding folder?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        string spath = newImageboard.getPath();
                        if (!Directory.Exists(spath))
                            Directory.CreateDirectory(spath);
                        Process.Start(spath);
                    }
                }
            }
            else
            {
                MessageBox.Show("Corrupt URL, unsupported website or not a board/thread!");
                edtURL.Text = "";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string url = edtURL.Text.Trim();                                     // get url from TextBox

            AddUrl(url);

            edtURL.Text = "";                                                    // clear TextBox
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
            for (int i = 0; i < ListThreads.Count; i++)
            {
                if (ListThreads[i].getURL() == url)
                    plc = i;
            }
            return plc;
        }

        private void lbThreads_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tPos = -1;
                if (lbThreads.IndexFromPoint(e.Location) != -1)
                {
                    tPos = lbThreads.IndexFromPoint(e.Location);
                    cmThreads.Show(lbThreads, new Point(e.X, e.Y));
                }
            }
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
            if (tPos != -1)
            {
                lock (threadLock)
                {
                    ListThreads.RemoveAt(tPos);
                    updateDataSource(typeURL.thread);
                }
            }
        }

        private void updateDataSource(typeURL type)
        {
            switch (type)
            {
                case typeURL.board:
                    lbBoards.Invoke((MethodInvoker)(() =>
                    {
                        lbBoards.DataSource = null;
                        lbBoards.DataSource = ListBoards;
                    }));
                    break;

                case typeURL.thread:
                    lbThreads.Invoke((MethodInvoker)(() =>
                    {
                        lbThreads.DataSource = null;
                        lbThreads.DataSource = ListThreads;
                    }));
                    break;
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tPos != -1)
            {
                string spath = ListThreads[tPos].getPath();
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tPos != -1)
            {
                string spath = lbThreads.Items[tPos].ToString();
                Process.Start(spath);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox tAbout = new AboutBox();
            tAbout.ShowDialog();
        }

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VInf tvInf = new VInf();
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
            if (Properties.Settings.Default.warnOnClose && ListThreads.Count > 0)
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
                    General.WriteURLs(ListBoards, ListThreads);
            }
        }

        private void openBoardFolderToolTip_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                string spath = ListBoards[bPos].getPath();
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
                    updateDataSource(typeURL.board);
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

        private void scan(object sender, EventArgs e)
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
            lock (threadLock)
            {
                //Removes 404'd threads
                foreach (Imageboard imB in ListThreads.ToArray())
                {
                    if (imB.isGone())
                    {
                        ListThreads.Remove(imB);
                        updateDataSource(typeURL.thread);

                        if (Properties.Settings.Default.saveOnClose)
                            General.WriteURLs(ListBoards, ListThreads);
                    }
                }
            }

            lock (boardLock)
            {
                //Searches for new threads on the watched boards
                foreach (Imageboard imB in ListBoards)
                {
                    string[] Threads = { };
                    try
                    {
                        Threads = imB.getThreads();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        foreach (string thread in Threads)
                        {
                            Imageboard newImageboard = General.CreateNewImageboard(thread);
                            if (newImageboard != null && isUnique(newImageboard.getURL(), ListThreads))
                            {
                                AddURLToList(newImageboard);
                            }
                        }
                    }
                }
            }

            lock (threadLock)
            {
                //Download threads
                foreach (Imageboard imB in ListThreads)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(imB.download));
                }
            }
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.minimizeToTray && this.WindowState == FormWindowState.Minimized)
            {
                // when minimized hide from taskbar if trayicon enabled
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
            if (e.Data.GetDataPresent(DataFormats.Text) &&
                (e.AllowedEffect & DragDropEffects.Copy) != 0)
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
            string url = (string)e.Data.GetData(DataFormats.Text);               // get url from drag and drop
            AddUrl(url);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (tcApp.SelectedIndex > 1) return;
            bool board = (tcApp.SelectedIndex == 1);                             // Board Tab is open -> board=true; Thread tab -> board=false

            string type = "threads";
            if (board) type = "boards";

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all " + type + "?", "Clear all " + type,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);    // confirmation prompt
            if (dialogResult == DialogResult.Yes)
            {
                if (board)
                {
                    ListBoards.Clear();
                    updateDataSource(typeURL.board);
                }
                else
                {
                    ListThreads.Clear();
                    updateDataSource(typeURL.thread);
                }

                if (Properties.Settings.Default.saveOnClose)
                    General.WriteURLs(ListBoards, ListThreads);
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

        private void lbThreads_MouseDoubleClick(object sender, MouseEventArgs e)
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
        }

        private void lbBoards_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int pos;
            if (e.Button == MouseButtons.Left)
            {
                if ((pos = lbBoards.IndexFromPoint(e.Location)) != -1)
                {
                    string spath = ListBoards[pos].getPath();
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
                        updateDataSource(typeURL.board);
                    }
                }
            }
        }

        private void lbThreads_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int pos = lbThreads.SelectedIndex;

                if (pos > -1)
                {
                    lock (threadLock)
                    {
                        ListThreads.RemoveAt(pos);
                        updateDataSource(typeURL.thread);
                    }
                }
            }
        }
    }
}