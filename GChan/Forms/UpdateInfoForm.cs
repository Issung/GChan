using GChan.Controllers;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GChan.Forms
{
    public partial class UpdateInfoForm : Form
    {
        public UpdateInfoForm()
        {
            InitializeComponent();

            UpdateController.Instance.Progress.ProgressChanged += Progress_ProgressChanged;

            infoLabel.Text = $"An update is available for GChan.\r\n";
            infoLabel.Text += $"Current version: v{UpdateController.Instance.CurrentVersion}.\r\n";
            infoLabel.Text += $"Latest version: v{UpdateController.Instance.CheckForUpdatesResult.LastVersion}.";
        }

        private void Progress_ProgressChanged(object sender, double e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                progressBar.Value = (int)(e * 100);
            });
        }

        private void viewReleasesButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Issung/GChan/releases");
        }

        private void doNotUpdateButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void updateButton_Click(object sender, EventArgs e)
        {
            buttonsPanel.Hide();
            downloadingPanel.Show();
            UpdateController.Instance.PerformUpdate();
        }
    }
}