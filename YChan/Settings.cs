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
        private const string PROGRAM_NAME = "GChan";

        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Shown(object sender, EventArgs e)
        {
            // Load settings into controls
            edtPath.Text = Properties.Settings.Default.path;
            edtTimer.Text = (Properties.Settings.Default.timer / 1000).ToString();
            chkHTML.Checked = Properties.Settings.Default.loadHTML;
            chkSave.Checked = Properties.Settings.Default.saveOnClose;
            chkTray.Checked = Properties.Settings.Default.minimizeToTray;
            chkWarn.Checked = Properties.Settings.Default.warnOnClose;

            chkStartWithWindows.Checked = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).GetValueNames().Contains(PROGRAM_NAME);
        }

        private void btnSSave_Click(object sender, EventArgs e)
        {
            if ((edtPath.Text != "") && General.IsDigitsOnly(edtTimer.Text))
            {
                string reason;
                if (!hasWriteAccessToFolder(edtPath.Text, out reason))
                {
                    MessageBox.Show(reason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (int.Parse(edtTimer.Text) < 5)
                {
                    MessageBox.Show("Timer has to be higher than 5 seconds");
                    return;
                }
                else
                {
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                    if (chkStartWithWindows.Checked)
                        registryKey.SetValue(PROGRAM_NAME, '"' + Application.ExecutablePath + '"' + " -tray");
                    else
                        registryKey.DeleteValue(PROGRAM_NAME);

                    General.SaveSettings(
                        edtPath.Text, 
                        int.Parse(edtTimer.Text) * 1000,
                        chkHTML.Checked,
                        chkSave.Checked,
                        chkTray.Checked,
                        chkWarn.Checked
                    );

                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Check value for timer and path");
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
                edtPath.Text = FolD.SelectedPath;
            }
        }
    }
}