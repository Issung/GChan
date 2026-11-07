using GChan.Controllers;
using System;
using System.Windows.Forms;

namespace GChan.Forms
{
    public partial class UpdateInfoForm : Form
    {
        public UpdateInfoForm()
        {
            InitializeComponent();

            UpdateController.Instance.Progress.ProgressChanged += Progress_ProgressChanged;

            infoLabel.Text = infoLabel.Text.Replace("%currentversion%", $"v{UpdateController.Instance.CurrentVersion}");
            infoLabel.Text = infoLabel.Text.Replace("%latestversion%", $"v{UpdateController.Instance.CheckForUpdatesResult.LastVersion}");
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
            Utils.OpenWebpage("https://github.com/" + Program.GITHUB_REPOSITORY_OWNER + "/" + Program.GITHUB_REPOSITORY_NAME + "/releases");
        }

        private void doNotUpdateButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void updateButton_Click(object sender, EventArgs e)
        {
            buttonsPanel.Hide();
            downloadingPanel.Show();
            await UpdateController.Instance.PerformUpdate();
        }
    }
}