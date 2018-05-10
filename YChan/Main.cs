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
        public List<Imageboard> listThreads = new List<Imageboard>();                       // list of monitored threads
        public List<Imageboard> listBoards = new List<Imageboard>();                        // list of monitored boards
        private Thread Scanner = null;                                                      // thread that addes stuff

        private int tPos = -1;                                                              // Item position in lbThreads
        private int bPos = -1;                                                              // Item position in lbBoards
        private System.Windows.Forms.Timer scnTimer = new System.Windows.Forms.Timer();     // Timmer for scanning

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            General.loadSettings();                                                     // load settings on startup
            if (!General.minimizeToTray)                                                // if trayicon deactivated
                nfTray.Visible = false;                                                 // hide it
            scnTimer.Enabled = false;                                                   // disable timer
            scnTimer.Interval = General.timer;                                          // set interval
            scnTimer.Tick += new EventHandler(this.scan);                               // when Timer ticks call scan()

            if (General.firstStart)
            {
                FirstStart tFirstStart = new FirstStart();                              // if first start, show first start message
                tFirstStart.ShowDialog();
            }

            if (General.saveOnClose)                                                    // if enabled load URLs from file
            {                                               

                string boards = General.loadURLs(true);
                string threads = General.loadURLs(false);                               // load threads

                if (!String.IsNullOrWhiteSpace(boards))
                {
                    string[] URLs = boards.Split('\n');
                    for (int i = 0; i < URLs.Length - 1; i++)
                    {
                        Imageboard newImageboard = General.createNewIMB(URLs[i], true); // and add them
                        listBoards.Add(newImageboard);
                    }
                }

                if (!String.IsNullOrWhiteSpace(threads))
                {
                    string[] URLs = threads.Split('\n');
                    for (int i = 0; i < URLs.Length - 1; i++)
                    {
                        Imageboard newImageboard = General.createNewIMB(URLs[i], false);
                        listThreads.Add(newImageboard);
                    }
                }

                lbBoards.DataSource = listBoards;
                lbThreads.DataSource = listThreads;

                scnTimer.Enabled = true;                                       // activate the timer
                scan(null, null);                                              // and start scanning
            }

            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool board = (tcApp.SelectedIndex == 1);                             // Board Tab is open -> board=true; Thread tab -> board=false
            Imageboard newImageboard = General.createNewIMB(edtURL.Text.Trim(), board);

            if (newImageboard != null)
            {
                if (isUnique(newImageboard.getURL(), listThreads))
                {
                    if (board)
                    {
                        listBoards.Add(newImageboard);

                        lbBoards.Invoke((MethodInvoker)(() =>
                        {
                            lbBoards.DataSource = null;
                            lbBoards.DataSource = listBoards;
                        }));
                    }
                    else
                    {
                        listThreads.Add(newImageboard);

                        lbThreads.Invoke((MethodInvoker)(() =>
                        {
                            lbThreads.DataSource = null;
                            lbThreads.DataSource = listThreads;
                        }));
                    }
                }
                else
                {
                    MessageBox.Show("URL is already in queue!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Corrupt URL, unsupported website or not a board/thread!");
                edtURL.Text = "";
                return;
            }

            edtURL.Text = "";

            if (!scnTimer.Enabled)
                scnTimer.Enabled = true;
            scan(null, null);
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
            for (int i = 0; i < listThreads.Count; i++)
            {
                if (listThreads[i].getURL() == url)
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
                listThreads[tPos].getThread().Abort();
                listThreads.RemoveAt(tPos);
                lbThreads.Invoke((MethodInvoker)(() =>
                {
                    lbThreads.DataSource = null;
                    lbThreads.DataSource = listThreads;
                }));
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tPos != -1)
            {
                string spath = listThreads[tPos].getPath();
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
            About tAbout = new About();
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
            if (General.minimizeToTray)
                nfTray.Visible = true;
            else
                nfTray.Visible = false;

            scnTimer.Interval = General.timer;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = DialogResult.OK;
            if (General.warnOnClose && listThreads.Count > 0)
            {
                CloseWarn clw = new CloseWarn();
                result = clw.ShowDialog();
                e.Cancel = (result == DialogResult.Cancel);
            }

            if (result == DialogResult.OK)
            {
                this.Hide();
                nfTray.Visible = false;
                scnTimer.Enabled = false;

                if (General.saveOnClose)
                    General.writeURLs(listBoards, listThreads);

                if (Scanner != null && Scanner.IsAlive)
                    Scanner.Abort();

                for (int i = 0; i < listThreads.Count; i++)
                {
                    if(listThreads[i].getThread().IsAlive)
                        listThreads[i].getThread().Abort();
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                string spath = listBoards[bPos].getPath();
                if (!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                string spath = lbBoards.Items[bPos].ToString();
                Process.Start(spath);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (bPos != -1)
            {
                listBoards.RemoveAt(bPos);
                lbBoards.Invoke((MethodInvoker)(() =>
                {
                    lbBoards.DataSource = null;
                    lbBoards.DataSource = listBoards;
                }));
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
            string spath = General.path;
            if (!Directory.Exists(spath))
                Directory.CreateDirectory(spath);
            Process.Start(spath);
        }

        private void scan(object sender, EventArgs e)
        {
            if (Scanner == null || !Scanner.IsAlive)
            {
                Scanner = new Thread(new ThreadStart(ScanThread));
                Scanner.Start();
            }
        }

        private void ScanThread()
        {
            for (int i = 0; i < listThreads.Count; i++)
            {
                if (listThreads[i].isGone())
                {
                    listThreads.RemoveAt(i);

                    lbThreads.Invoke((MethodInvoker)delegate
                    {
                        lbThreads.DataSource = null;
                        lbThreads.DataSource = listThreads;
                    });

                    i--;
                }
            }

            for (int i = 0; i < listBoards.Count; i++)
            {
                string[] Threads = { };
                try
                {
                    Threads = listBoards[i].getThreads().Split('\n');
                }
                catch (Exception exep)
                {
                }
                for (int j = 0; j < Threads.Length; j++)
                {
                    Imageboard newImageboard = General.createNewIMB(Threads[j], false);
                    if (newImageboard != null && isUnique(newImageboard.getURL(), listThreads))
                    {
                        listThreads.Add(newImageboard);
                        lbThreads.Invoke((MethodInvoker)(() =>
                        {
                            lbThreads.DataSource = null;
                            lbThreads.DataSource = listThreads;
                        }));
                    }
                }
            }

            for (int i = 0; i < listThreads.Count; i++)
            {
                if (!listThreads[i].getThread().IsAlive)
                {
                    //TODO: Make it so only a few threads are running so as to not overload the computer and server resources
                    listThreads[i].resetThread();
                    listThreads[i].getThread().Start();
                }
            }
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (General.minimizeToTray && this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();                                                                // when minimized hide from taskbar if trayicon enabled
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, false));
        }

        private void openFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(General.path);
        }
    }
}