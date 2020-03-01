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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GChan
{
    public enum ImageFileNameFormat {
        [Description("ID (eg. '1570301.jpg')")]
        ID = 0,
        [Description("OriginalFilename (eg. 'LittleSaintJames.jpg')")]
        OriginalFilename = 1,
        [Description("ID - OriginalFilename (eg. '1570301 - LittleSaintJames.jpg')")]
        IDAndOriginalFilename = 2
    };

    public partial class Settings : Form
    {
        string directory;

        public Settings()
        {
            InitializeComponent();
            imageFilenameFormatComboBox.DataSource = EnumHelper.GetEnumDescriptions(typeof(ImageFileNameFormat));
        }

        private void Settings_Shown(object sender, EventArgs e)
        {
            // Load settings into controls
            directory = Properties.Settings.Default.path;
            directoryTextBox.Text = directory;

            timerNumeric.Value = (Properties.Settings.Default.timer / 1000);

            imageFilenameFormatComboBox.SelectedIndex = Properties.Settings.Default.imageFilenameFormat;

            chkHTML.Checked = Properties.Settings.Default.loadHTML;
            chkSave.Checked = Properties.Settings.Default.saveOnClose;
            chkTray.Checked = Properties.Settings.Default.minimizeToTray;
            chkWarn.Checked = Properties.Settings.Default.warnOnClose;

            chkStartWithWindows.Checked = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).GetValueNames().Contains(Utils.PROGRAM_NAME);

            addThreadSubjectToFolderCheckBox.Checked = Properties.Settings.Default.addThreadSubjectToFolder;

            addUrlFromClipboardWhenTextboxEmpty.Checked = Properties.Settings.Default.addUrlFromClipboardWhenTextboxEmpty;
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
                Utils.SaveSettings(
                    directory, 
                    (int)timerNumeric.Value * 1000,
                    (ImageFileNameFormat)imageFilenameFormatComboBox.SelectedIndex,
                    chkHTML.Checked,
                    chkSave.Checked,
                    chkTray.Checked,
                    chkWarn.Checked,
                    chkStartWithWindows.Checked,
                    addThreadSubjectToFolderCheckBox.Checked,
                    addUrlFromClipboardWhenTextboxEmpty.Checked
                );

                Close();
            }
        }

        private void btnSCan_Click(object sender, EventArgs e)
        {
            Close();
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

        /// <summary>
        /// Extends the ComboBox's dropdown horizontally to accomodate for long length items.
        /// https://stackoverflow.com/a/28271652/8306962
        /// </summary>
        private void imageFilenameFormatComboBox_DropDown(object sender, EventArgs e)
        {
            Graphics g = imageFilenameFormatComboBox.CreateGraphics();
            float largestSize = 0;

            for (int i = 0; i < imageFilenameFormatComboBox.Items.Count; i++)
            {
                SizeF textSize = g.MeasureString(imageFilenameFormatComboBox.Items[i].ToString(), imageFilenameFormatComboBox.Font);
                if (textSize.Width > largestSize)
                    largestSize = textSize.Width;
            }

            if (largestSize > 0)
                imageFilenameFormatComboBox.DropDownWidth = (int)largestSize;
        }
    }
}