using Onova;
using Onova.Models;
using Onova.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GChan.Models;
using System.Windows.Forms;
using GChan.Forms;

namespace GChan.Controllers
{
    class UpdateController
    {
        private static UpdateController instance = null;

        public static UpdateController Instance { get {
                if (instance == null)
                    instance = new UpdateController();

                return instance;
            } 
        }

        IUpdateManager updateManager = new UpdateManager(
                new GithubPackageResolver(Program.GITHUB_REPOSITORY_OWNER, Program.GITHUB_REPOSITORY_NAME, $"{Program.NAME}-*.zip"),
                new ZipPackageExtractor()
            );

        /// <summary>
        /// Progress reporter for update downloading process.
        /// </summary>
        public Progress<double> Progress = new Progress<double>();

        public CheckForUpdatesResult CheckForUpdatesResult { get; private set; }

        // Update Check Started
        public delegate void UpdateCheckStartedEvent(object sender, bool initiatedByUser);
        public event UpdateCheckStartedEvent UpdateCheckStarted;
        
        // Update Check Finished
        public delegate void UpdateCheckFinishedEvent(object sender, CheckForUpdatesResult result, bool initiatedByUser);
        public event UpdateCheckFinishedEvent UpdateCheckFinished;

        // Update Started
        public delegate void UpdateStartedEvent(object sender);
        public event UpdateStartedEvent UpdateStarted;

        public string CurrentVersion { get; private set; }

        UpdateInfoForm infoForm;

        private UpdateController()
        {
            CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString().Trim();

            // Save current version and cut off trailing ".0"'s.
            while (string.Join("", CurrentVersion.Skip(Math.Max(0, CurrentVersion.Length - 2))) == ".0")
            {
                CurrentVersion = CurrentVersion.Substring(0, CurrentVersion.Length - 2);
            }
        }

        public async void CheckForUpdates(bool initiatedByUser)
        {
            UpdateCheckStarted?.Invoke(this, initiatedByUser);

            CheckForUpdatesResult = await updateManager.CheckForUpdatesAsync();

            UpdateCheckFinished?.Invoke(this, CheckForUpdatesResult, initiatedByUser);
        }

        public async void PresentInformation()
        {
            infoForm = new UpdateInfoForm();
            infoForm.TopMost = true;
            infoForm.ShowDialog();
        }

        public async Task PerformUpdate()
        {
            if (CheckForUpdatesResult.CanUpdate)
            {
                UpdateStarted?.Invoke(this);

                // Prepare the latest update
                await updateManager.PrepareUpdateAsync(CheckForUpdatesResult.LastVersion, Progress);

                // Launch updater and exit
                updateManager.LaunchUpdater(CheckForUpdatesResult.LastVersion);

                infoForm?.Close();

                Program.mainForm.FormClosing -= Program.mainForm.MainForm_FormClosing;

                Utils.SaveURLs(Program.mainForm.Model.Boards, Program.mainForm.Model.Threads);

                Application.Exit();
            }
        }
    }
}