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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.IO;
using System.Diagnostics;


namespace YChan {
    public partial class frmMain : Form {
        public List<Imageboard> clThreads = new List<Imageboard>();                        // list of monitored threads
        public List<Imageboard> clBoards  = new List<Imageboard>();                        // list of monitored boards
        List<Thread> thrThreads    = new List<Thread>();                            // list of threads that download 
        Thread Scanner = null;                                                      // thread that addes stuff

        int tPos = -1;                                                              // Item position in lbThreads
        int bPos = -1;                                                              // Item position in lbBoards
        System.Windows.Forms.Timer scnTimer = new System.Windows.Forms.Timer();     // Timmer for scanning

        public frmMain() {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e) {
            General.loadSettings();                                                 // load settings on startup
            if(!General.minimizeToTray)                                             // if trayicon deactivated
                nfTray.Visible = false;                                             // hide it
            scnTimer.Enabled  = false;                                              // disable timer                 
            scnTimer.Interval = General.timer;                                      // set interval
            scnTimer.Tick += new EventHandler(this.scan);                           // when Timer ticks call scan()
            if(General.saveOnClose) {                                               // if enabled load URLs from file 
                string[] URLs;

                string boards = General.loadURLs(true);
                if(boards != "") {
                    URLs = boards.Split('\n');
                    for(int i = 0; i < URLs.Length - 1; i++) {
                        Imageboard newImageboard = General.createNewIMB(URLs[i], true); // and add them 
                        lbBoards.Items.Add(URLs[i]);
                        clBoards.Add(newImageboard);
                    }
                    scnTimer.Enabled = true;                                       // activate the timer
                    scan(null, null);                                              // and start scanning
                }


                string threads = General.loadURLs(false);                         // load threads
                if(threads != "") {
                    URLs = threads.Split('\n');
                    for(int i = 0; i < URLs.Length - 1; i++) {
                        Imageboard newImageboard = General.createNewIMB(URLs[i], false);
                        if(newImageboard == null) {
                            MessageBox.Show(URLs[i]);
                        } else {
                            lbThreads.Items.Add(URLs[i]);
                            clThreads.Add(newImageboard);
                            Thread nIMB = new Thread(delegate() {
                                newImageboard.download();
                            });
                            thrThreads.Add(nIMB);
                            thrThreads.Last().Start();
                        }
                    }
                }
            }

            if(General.firstStart) {
                FirstStart tFirstStart = new FirstStart();                        // if first start, show first start message
                tFirstStart.ShowDialog();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            bool board = (tcApp.SelectedIndex == 1);                             // Board Tab is open -> board=true; Thread tab -> board=false 
            Imageboard newImageboard = General.createNewIMB(edtURL.Text.Trim(), board);

            if(newImageboard != null) {
                if(isUnique(newImageboard.getURL(), clThreads)) {
                    if(board) {
                        lbBoards.Items.Add(edtURL.Text);
                        clBoards.Add(newImageboard);
                    } else {
                        lbThreads.Items.Add(edtURL.Text);
                        clThreads.Add(newImageboard);
                    }
                } else {
                    MessageBox.Show("URL is already in queu!");
                }
            } else {
                MessageBox.Show("Corrupt URL, unsupported website or not a board/thread!");
                edtURL.Text = "";
                return;
            }
            if(!board) {
                Thread nIMB = new Thread(delegate() {
                    newImageboard.download();
                });
                nIMB.Name = newImageboard.getURL();
                thrThreads.Add(nIMB);
                thrThreads[thrThreads.Count - 1].Start();
            }

            edtURL.Text = "";

            if(!scnTimer.Enabled)
                scnTimer.Enabled = true;
            scan(null, null);
        }        
        
        private bool isUnique(string url, List<Imageboard> List) {
            bool flag = true;
            for(int i = 0; i < List.Count; i++) {
                if(List[i].getURL() == url)
                    flag = false;
            }
            return flag;
        }

        private int getPlace(string url) {
            int plc = -1;
            for(int i = 0; i < clThreads.Count; i++) {
                if(clThreads[i].getURL() == url)
                    plc = i;
            }
            return plc;
        }

        private void lbThreads_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == System.Windows.Forms.MouseButtons.Right) {
                tPos = -1;
                if(lbThreads.IndexFromPoint(e.Location) != -1) {
                    tPos = lbThreads.IndexFromPoint(e.Location);
                    cmThreads.Show(lbThreads, new Point(e.X, e.Y));
                }
            }
        }

        private void lbBoards_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == System.Windows.Forms.MouseButtons.Right) {
                bPos = -1;
                if(lbBoards.IndexFromPoint(e.Location) != -1) {
                    bPos = lbBoards.IndexFromPoint(e.Location);
                    cmBoards.Show(lbBoards, new Point(e.X, e.Y));
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            if(tPos != -1){
                clThreads.RemoveAt(tPos);
                thrThreads[tPos].Abort();
                thrThreads.RemoveAt(tPos);
                lbThreads.Items.RemoveAt(tPos);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            if(tPos != -1) {
                string spath = clThreads[tPos].getPath();
                if(!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e) {
            if(tPos != -1) {
                string spath = lbThreads.Items[tPos].ToString();
                Process.Start(spath);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            About tAbout = new About();
            tAbout.ShowDialog();
        }

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e) {
            VInf tvInf = new VInf();
            tvInf.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
            Settings tSettings = new Settings();
            tSettings.ShowDialog();
            if(General.minimizeToTray)
                nfTray.Visible = true;
            else
                nfTray.Visible = false;

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            bool close = true;
            if(General.warnOnClose && clThreads.Count > 0) {
                CloseWarn clw = new CloseWarn();
                var result = clw.ShowDialog();
                close = clw.closeit;
                if(!close)
                    e.Cancel = true;
            }

            if(close == true || !General.warnOnClose) {
                this.Hide();
                nfTray.Visible = false;
                if(General.saveOnClose)
                    General.writeURLs(clBoards, clThreads);

                if(Scanner != null && Scanner.IsAlive)
                    Scanner.Abort();
                for(int i = 0; i < thrThreads.Count; i++) {
                    if(thrThreads[i].IsAlive)
                        thrThreads[i].Abort();
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) {
            if(bPos != -1) {
                string spath = clBoards[bPos].getPath();
                if(!Directory.Exists(spath))
                    Directory.CreateDirectory(spath);
                Process.Start(spath);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) {
            if(bPos != -1) {
                string spath = lbBoards.Items[bPos].ToString();
                Process.Start(spath);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) {
            if(bPos != -1) {
                clBoards.RemoveAt(bPos);
                lbBoards.Items.RemoveAt(bPos);
            }
        }

        private void nfTray_MouseDoubleClick(object sender, MouseEventArgs e) {
            if(this.Visible)
                this.Hide();
            else {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void cmTrayExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void cmTrayOpen_Click(object sender, EventArgs e) {
            string spath = General.path;
            if(!Directory.Exists(spath))
                Directory.CreateDirectory(spath);
            Process.Start(spath);
        }
        private void scan(object sender, EventArgs e) {
            /*#if DEBUG
                        MessageBox.Show("Threads: (" + thrThreads.Count + ") (" + clThreads.Count +")");
                        MessageBox.Show("Boards: (" + thrBoards.Count + ") (" + clBoards.Count +")");
            #endif*/

            if(Scanner == null || !Scanner.IsAlive) {
                Scanner = new Thread(delegate() {
                    for(int k = 0; k < clThreads.Count; k++) {
                        if(clThreads[k].isGone()) {
                            clThreads.RemoveAt(k);
                            thrThreads.RemoveAt(k);
                            lbThreads.Invoke((MethodInvoker) delegate {
                                lbThreads.Items.RemoveAt(k);
                            });
                        }
                    }

                    for(int k = 0; k < clBoards.Count; k++) {
                        string[] Threads = { };
                        try {
                            Threads = clBoards[k].getThreads().Split('\n');
                        } catch(Exception exep) {

                        }
                        for(int l = 0; l < Threads.Length; l++) {
                            Imageboard newImageboard = General.createNewIMB(Threads[l], false);
                            if(newImageboard != null && isUnique(newImageboard.getURL(), clThreads)) {
                                lbThreads.Invoke((MethodInvoker) (() => {
                                    lbThreads.Items.Add(Threads[l]);
                                }));
                                clThreads.Add(newImageboard);
                                Thread nIMB = new Thread(delegate() {
                                    newImageboard.download();
                                });
                                nIMB.Name = newImageboard.getURL();
                                thrThreads.Add(nIMB);
                                thrThreads[thrThreads.Count - 1].Start();
                            }
                        }
                    }

                    for(int k = 0; k < clThreads.Count; k++) {
                        if(!thrThreads[k].IsAlive) {
                            /*                        MessageBox.Show("Down: " + k);
                            */
                            thrThreads[k] = new Thread(delegate() {
                                int x = k;
                                try {
                                    clThreads[k-1].download();   // why
                                } catch(Exception exp) {
                                    //                                    MessageBox.Show(exp.Message + " k: " + x);
                                }
                            });
                            thrThreads[k].Name = clThreads[k].getURL();
                            thrThreads[k].Start();
                        }

                    }
                });
                Scanner.Start();
            }
        }

        private void frmMain_SizeChanged(object sender, EventArgs e) {                  
            if(General.minimizeToTray && this.WindowState == FormWindowState.Minimized) {
                this.Hide();                                                                // when minimized hide from taskbar if trayicon enabled
            }
        }
    }
}
