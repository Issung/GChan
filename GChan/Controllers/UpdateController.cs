using GChan.Data;
using GChan.Forms;
using Onova;
using Onova.Models;
using Onova.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GChan.Controllers
{
    /// <summary>
    /// Controller that manages the check for updates/updating version flow.
    /// </summary>
    class UpdateController
    {
        private static UpdateController instance = null;

        public static UpdateController Instance 
        { 
            get 
            {
                instance ??= new UpdateController();
                return instance;
            } 
        }

        readonly IUpdateManager updateManager = new UpdateManager(
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

        public void ShowUpdateDialog()
        {
            infoForm = new UpdateInfoForm
            {
                TopMost = true
            };
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

                await DataController.Save(Program.mainForm.Model.Threads, Program.mainForm.Model.Boards);

                Application.Exit();
            }
        }
    }
}