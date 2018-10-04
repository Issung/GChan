using System;/************************************************************************

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

using System.IO;
using System.Windows.Forms;

namespace YChan
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void btnSSave_Click(object sender, EventArgs e)
        {
            if ((edtPath.Text != "") && (General.IsDigitsOnly(edtTimer.Text)))
            {
                if (!hasWriteAccessToFolder(edtPath.Text))
                {
                    MessageBox.Show("No Permission to write to the selected folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (int.Parse(edtTimer.Text) < 5)
                {
                    MessageBox.Show("Timer has to be higher than 5 seconds");
                }
                else
                {
                    General.SaveSettings(edtPath.Text, int.Parse(edtTimer.Text) * 1000, chkHTML.Checked, chkSave.Checked, chkTray.Checked, chkWarn.Checked); // save settings

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

        private void edtPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolD = new FolderBrowserDialog();
            FolD.Description = "Select Folder";
            FolD.SelectedPath = @"C:\";       // Vorgabe Pfad (und danach der gewählte Pfad)
            DialogResult dRes = FolD.ShowDialog(this);
            if (dRes == DialogResult.OK)
            {
                edtPath.Text = FolD.SelectedPath;
            }
            else
                MessageBox.Show("Aborted");
        }

        private void Settings_Shown(object sender, EventArgs e)
        { // load settings into controls
            edtPath.Text = Properties.Settings.Default.path;
            edtTimer.Text = (Properties.Settings.Default.timer / 1000).ToString();
            chkHTML.Checked = Properties.Settings.Default.loadHTML;
            chkSave.Checked = Properties.Settings.Default.saveOnClose;
            chkTray.Checked = Properties.Settings.Default.minimizeToTray;
            chkWarn.Checked = Properties.Settings.Default.warnOnClose;
        }

        //Source: https://stackoverflow.com/questions/1410127/c-sharp-test-if-user-has-write-access-to-a-folder?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        private bool hasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder.
                // This will raise an exception if the path is read only or do not have access to view the permissions.
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}