﻿using Onova;
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
        public delegate void UpdateCheckStartedEvent(object sender, EventArgs args);
        public event UpdateCheckStartedEvent UpdateCheckStarted;
        
        // Update Check Finished
        public delegate void UpdateCheckFinishedEvent(object sender, CheckForUpdatesResult result);
        public event UpdateCheckFinishedEvent UpdateCheckFinished;

        // Update Started
        public delegate void UpdateStartedEvent(object sender, EventArgs args);
        public event UpdateStartedEvent UpdateStarted;

        public string CurrentVersion { get; private set; }

        private UpdateController()
        {
            CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString().Trim();

            // Save current version and cut off trailing ".0"'s.
            while (string.Join("", CurrentVersion.Skip(Math.Max(0, CurrentVersion.Length - 2))) == ".0")
            {
                CurrentVersion = CurrentVersion.Substring(0, CurrentVersion.Length - 2);
            }
        }

        public async void CheckForUpdates()
        {
            UpdateCheckStarted?.Invoke(this, EventArgs.Empty);

            CheckForUpdatesResult = await updateManager.CheckForUpdatesAsync();

            UpdateCheckFinished?.Invoke(this, CheckForUpdatesResult);
        }

        public async void PresentInformation()
        {
            // Show a dialog box with info and links to GitHub, etc.

            /*DialogResult result = MessageBox.Show(
                    $"There is an update available. Your current version is {CurrentVersion}, the latest version is {CheckForUpdatesResult.LastVersion}. Would you like to update to the latest version?",
                    "Update Available",
                    MessageBoxButtons.YesNo
                );

            if (result == DialogResult.Yes)
            {
                 await PerformUpdate();
            }*/

            UpdateInfoForm infoForm = new UpdateInfoForm();
            infoForm.ShowDialog();
        }

        public async Task PerformUpdate()
        {
            if (CheckForUpdatesResult.CanUpdate)
            {
                UpdateStarted?.Invoke(this, EventArgs.Empty);

                // Prepare the latest update
                await updateManager.PrepareUpdateAsync(CheckForUpdatesResult.LastVersion, Progress);

                Properties.Settings.Default.UpdateSettings = true;
                Properties.Settings.Default.Save();

                // Launch updater and exit
                updateManager.LaunchUpdater(CheckForUpdatesResult.LastVersion);

                Application.Exit();
            }
        }
    }
}