using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GChan
{
    public enum ImageFileNameFormat {
        [Description("ID (eg. '1570301.jpg')")]
        ID = 0,
        [Description("OriginalFilename (eg. 'LittleSaintJames.jpg')")]
        OriginalFilename = 1,
        [Description("ID - OriginalFilename (eg. '1570301 - LittleSaintJames.jpg')")]
        IDAndOriginalFilename = 2,
        [Description("OriginalFilename - ID (eg. 'LittleSaintJames.jpg - 1570301.jpg')")]
        OriginalFilenameAndID = 3
    };

    public enum ThreadFolderNameFormat {
        [Description("ThreadID - ThreadName")]
        IdName = 0,
        [Description("ThreadName - ThreadID")]
        NameId = 1
    }

    public partial class Settings : Form
    {
        string directory;

        public Settings()
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

        private void Settings_Shown(object sender, EventArgs e)
        {
            // Load settings into controls
            directory = Properties.Settings.Default.SavePath;
            directoryTextBox.Text = directory;

            timerNumeric.Value = (Properties.Settings.Default.ScanTimer / 1000);

            imageFilenameFormatComboBox.SelectedIndex = Properties.Settings.Default.ImageFilenameFormat;
            threadFolderNameFormatComboBox.SelectedIndex = Properties.Settings.Default.ThreadFolderNameFormat;

            chkHTML.Checked = Properties.Settings.Default.SaveHTML;
            chkSave.Checked = Properties.Settings.Default.SaveListsOnClose;
            chkTray.Checked = Properties.Settings.Default.MinimizeToTray;
            chkWarn.Checked = Properties.Settings.Default.WarnOnClose;

            chkStartWithWindows.Checked = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).GetValueNames().Contains(Utils.PROGRAM_NAME);
            chkStartWithWindowsMinimized.Checked = Properties.Settings.Default.StartWithWindowsMinimized;

            renameThreadFolderCheckBox.Checked = Properties.Settings.Default.AddThreadSubjectToFolder;

            addUrlFromClipboardWhenTextboxEmpty.Checked = Properties.Settings.Default.AddUrlFromClipboardWhenTextboxEmpty;

            checkForUpdatesOnStartCheckBox.Checked = Properties.Settings.Default.CheckForUpdatesOnStart;
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
                    (ThreadFolderNameFormat)threadFolderNameFormatComboBox.SelectedIndex,
                    chkHTML.Checked,
                    chkSave.Checked,
                    chkTray.Checked,
                    chkWarn.Checked,
                    chkStartWithWindows.Checked,
                    chkStartWithWindowsMinimized.Checked,
                    renameThreadFolderCheckBox.Checked,
                    addUrlFromClipboardWhenTextboxEmpty.Checked,
                    checkForUpdatesOnStartCheckBox.Checked
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
            /*Graphics g = imageFilenameFormatComboBox.CreateGraphics();
            float largestSize = 0;

            for (int i = 0; i < imageFilenameFormatComboBox.Items.Count; i++)
            {
                SizeF textSize = g.MeasureString(imageFilenameFormatComboBox.Items[i].ToString(), imageFilenameFormatComboBox.Font);
                if (textSize.Width > largestSize)
                    largestSize = textSize.Width;
            }

            if (largestSize > 0)
                imageFilenameFormatComboBox.DropDownWidth = (int)largestSize;*/
        }

        private void chkStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            EnableChkStartWithWindowsMinimizedCheckBox();
        }

        private void chkTray_CheckedChanged(object sender, EventArgs e)
        {
            EnableChkStartWithWindowsMinimizedCheckBox();
        }

        private void EnableChkStartWithWindowsMinimizedCheckBox()
        {
            if (chkTray.Checked && chkStartWithWindows.Checked)
                chkStartWithWindowsMinimized.Enabled = true;
            else
                chkStartWithWindowsMinimized.Enabled = false;
        }

        private void renameThreadFolderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            threadFolderNameFormatLabel.Enabled = renameThreadFolderCheckBox.Checked;
            threadFolderNameFormatComboBox.Enabled = renameThreadFolderCheckBox.Checked;
        }
    }
}