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

using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GChan
{
    public partial class Settings : Form
    {
        string directory;

        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Shown(object sender, EventArgs e)
        {
            // Load settings into controls
            directory = Properties.Settings.Default.path;
            directoryTextBox.Text = directory;

            timerNumeric.Value = (Properties.Settings.Default.timer / 1000);
            chkHTML.Checked = Properties.Settings.Default.loadHTML;
            chkSave.Checked = Properties.Settings.Default.saveOnClose;
            chkTray.Checked = Properties.Settings.Default.minimizeToTray;
            chkWarn.Checked = Properties.Settings.Default.warnOnClose;

            chkStartWithWindows.Checked = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).GetValueNames().Contains(General.PROGRAM_NAME);
        }

        private void btnSSave_Click(object sender, EventArgs e)
        {
            string reason;

            if (!hasWriteAccessToFolder(directory, out reason))
            {
                MessageBox.Show(reason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (timerNumeric.Value < 5)
            {
                MessageBox.Show("Timer has to be higher than 5 seconds");
                return;
            }
            else
            {
                General.SaveSettings(
                    directory, 
                    (int)timerNumeric.Value * 1000,
                    chkHTML.Checked,
                    chkSave.Checked,
                    chkTray.Checked,
                    chkWarn.Checked,
                    chkStartWithWindows.Checked
                );

                this.Close();
            }
        }

        private void btnSCan_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Source: https://stackoverflow.com/q/1410127/8306962
        private bool hasWriteAccessToFolder(string folderPath, out string reason)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder.
                // This will raise an exception if the path is read only or do not have access to view the permissions.
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                reason = "No problem";
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                reason = "No permission to write on that location.";
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                reason = "Directory not found.";
                return false;
            }
        }

        private void SetPathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolD = new FolderBrowserDialog();
            FolD.Description = "Select Folder";
            FolD.SelectedPath = @"C:\";
            DialogResult dRes = FolD.ShowDialog(this);

            if (dRes == DialogResult.OK)
            {
                directory = FolD.SelectedPath;
                directoryTextBox.Text = directory;
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", string.Format(directory));
        }
    }
}