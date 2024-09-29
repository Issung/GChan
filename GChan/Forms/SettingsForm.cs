﻿using GChan.Properties;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GChan
{
    public enum ImageFileNameFormat
    {
        [Description("ID (eg. '1570301.jpg')")]
        ID = 0,
        [Description("OriginalFilename (eg. 'LittleSaintJames.jpg')")]
        OriginalFilename = 1,
        [Description("ID - OriginalFilename (eg. '1570301 - LittleSaintJames.jpg')")]
        IDAndOriginalFilename = 2,
        [Description("OriginalFilename - ID (eg. 'LittleSaintJames.jpg - 1570301.jpg')")]
        OriginalFilenameAndID = 3
    };

    public enum ThreadFolderNameFormat
    {
        [Description("ID - Subject")]
        IdSubject = 0,
        [Description("Subject - ID")]
        SubjectId = 1
    }

    public partial class SettingsForm : Form
    {
        string directory;

        public SettingsForm()
        {
            InitializeComponent();
            //imageFilenameFormatComboBox.DataSource = EnumHelper.GetEnumDescriptions(typeof(ImageFileNameFormat));
            imageFilenameFormatComboBox.DataSource = Enum.GetValues(typeof(ImageFileNameFormat))
                .Cast<ImageFileNameFormat>()
                .Select(value => new
                {
                    EnumDescription = EnumHelper.GetEnumDescription(value),
                    EnumValue = value
                })
                .ToList();

            //threadFolderNameFormatComboBox.DataSource = EnumHelper.GetEnumDescriptions(typeof(ThreadFolderNameFormat));
            threadFolderNameFormatComboBox.DataSource = Enum.GetValues(typeof(ThreadFolderNameFormat))
                .Cast<ThreadFolderNameFormat>()
                .Select(value => new
                {
                    EnumDescription = EnumHelper.GetEnumDescription(value),
                    EnumValue = value
                })
                .ToList();
        }

        /// <summary>
        /// Load settings into controls.
        /// </summary>
        private void Settings_Shown(object sender, EventArgs e)
        {
            userAgentTextBox.Text = Settings.Default.UserAgent;

            directory = Settings.Default.SavePath;
            directoryTextBox.Text = directory;

            timerNumeric.Value = (Settings.Default.ScanTimer / 1000);
            max1RequestPerSecondCheckBox.Checked = Settings.Default.Max1RequestPerSecond;
            concurrentDownloadsNumeric.Value = Settings.Default.MaximumConcurrentDownloads;

            imageFilenameFormatComboBox.SelectedIndex = Settings.Default.ImageFilenameFormat;
            threadFolderNameFormatComboBox.SelectedIndex = Settings.Default.ThreadFolderNameFormat;

            chkSaveHtml.Checked = Settings.Default.SaveHtml;
            chkSaveThumbnails.Checked = Settings.Default.SaveThumbnails;
            chkSave.Checked = Settings.Default.SaveListsOnClose;
            chkTray.Checked = Settings.Default.MinimizeToTray;
            chkWarn.Checked = Settings.Default.WarnOnClose;

            chkStartWithWindows.Checked = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).GetValueNames().Contains(Utils.PROGRAM_NAME);
            chkStartWithWindowsMinimized.Checked = Settings.Default.StartWithWindowsMinimized;

            renameThreadFolderCheckBox.Checked = Settings.Default.AddThreadSubjectToFolder;

            addUrlFromClipboardWhenTextboxEmpty.Checked = Settings.Default.AddUrlFromClipboardWhenTextboxEmpty;

            checkForUpdatesOnStartCheckBox.Checked = Settings.Default.CheckForUpdatesOnStart;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!HasWriteAccessToFolder(directory, out var reason))
            {
                MessageBox.Show(reason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (timerNumeric.Value < 5)
            {
                MessageBox.Show("Timer must be greater than 5 seconds.");
                return;
            }

            SaveSettings();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveSettings()
        {
            Settings.Default.UserAgent = userAgentTextBox.Text;
            Settings.Default.SavePath = directory;
            Settings.Default.ScanTimer = (int)timerNumeric.Value * 1000;
            Settings.Default.Max1RequestPerSecond = max1RequestPerSecondCheckBox.Checked;
            Settings.Default.MaximumConcurrentDownloads = (int)concurrentDownloadsNumeric.Value;
            Settings.Default.ImageFilenameFormat = (byte)imageFilenameFormatComboBox.SelectedIndex;
            Settings.Default.ThreadFolderNameFormat = (byte)threadFolderNameFormatComboBox.SelectedIndex;
            Settings.Default.SaveHtml = chkSaveHtml.Checked;
            Settings.Default.SaveThumbnails = chkSaveThumbnails.Checked;
            Settings.Default.SaveListsOnClose = chkSave.Checked;
            Settings.Default.MinimizeToTray = chkTray.Checked;
            Settings.Default.WarnOnClose = chkWarn.Checked;
            Settings.Default.StartWithWindowsMinimized = chkStartWithWindowsMinimized.Checked;
            Settings.Default.AddThreadSubjectToFolder = renameThreadFolderCheckBox.Checked;
            Settings.Default.AddUrlFromClipboardWhenTextboxEmpty = addUrlFromClipboardWhenTextboxEmpty.Checked;
            Settings.Default.CheckForUpdatesOnStart = checkForUpdatesOnStartCheckBox.Checked;

            Settings.Default.Save();

            var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (chkStartWithWindows.Checked)
            {
                var args = (Settings.Default.MinimizeToTray && Settings.Default.StartWithWindowsMinimized) ? $" {Program.TRAY_CMDLINE_ARG}" : "";
                registryKey.SetValue(
                    Utils.PROGRAM_NAME,
                    '"' + Application.ExecutablePath + '"' + args
                );
            }
            else if (registryKey.GetValue(Utils.PROGRAM_NAME) != null)
            {
                registryKey.DeleteValue(Utils.PROGRAM_NAME);
            }
        }

        //Source: https://stackoverflow.com/q/1410127/8306962
        private bool HasWriteAccessToFolder(string folderPath, out string reason)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder.
                // This will raise an exception if the path is read only or do not have access to view the permissions.
                var directorySecurity = Directory.GetAccessControl(folderPath);
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
            var openFileDialog = new CommonOpenFileDialog();
            openFileDialog.IsFolderPicker = true;
            openFileDialog.Title = "Select Folder";
            openFileDialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                directory = openFileDialog.FileName;
                directoryTextBox.Text = directory;
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", string.Format(directory));
        }

        private void chkTray_CheckedChanged(object sender, EventArgs e)
        {
            EnableChkStartWithWindowsMinimizedCheckBox();
        }

        private void chkStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            EnableChkStartWithWindowsMinimizedCheckBox();
        }

        private void EnableChkStartWithWindowsMinimizedCheckBox()
        {
            chkStartWithWindowsMinimized.Enabled = chkTray.Checked && chkStartWithWindows.Checked;
        }

        private void renameThreadFolderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            threadFolderNameFormatLabel.Enabled = renameThreadFolderCheckBox.Checked;
            threadFolderNameFormatComboBox.Enabled = renameThreadFolderCheckBox.Checked;
        }

        private void chkHTML_CheckedChanged(object sender, EventArgs e)
        {
            chkSaveThumbnails.Enabled = chkSaveHtml.Checked;
        }
    }
}