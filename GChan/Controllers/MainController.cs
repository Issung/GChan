using GChan.Data;
using GChan.Forms;
using GChan.Helpers.Extensions;
using GChan.Models.Trackers;
using GChan.Properties;
using GChan.Services;
using GChan.ViewModels;
using NLog;
using Onova.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thread = GChan.Models.Trackers.Thread;
using Timer = System.Windows.Forms.Timer;
using Type = GChan.Models.Trackers.Type;

namespace GChan.Controllers
{
    /// <remarks>
    /// <see cref="Model"/>'s lists are only ever modified on the UI thread, which is what keeps them consistent.
    /// Code running elsewhere (processing threads) must marshal changes with <see cref="OnUiThread"/>, and must not hold
    /// a lock while doing so, as the UI thread may be waiting for that lock.
    /// </remarks>
    public class MainController : IProcessableHooks
    {
        /// <summary>
        /// How often tracker state (download progress, subjects, board positions) is saved while running,
        /// so little is lost if the process is killed.
        /// </summary>
        private static readonly TimeSpan PERIODIC_SAVE_INTERVAL = TimeSpan.FromMinutes(1);

        internal readonly MainForm Form;
        internal readonly MainFormModel Model;

        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly ProcessQueue processQueue;
        private readonly Timer periodicSaveTimer = new();
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private bool periodicSaveRunning;

        public MainController(MainForm mainForm)
        {
            // Controller Setup
            processQueue = new(this, AddTrackerIfNew, cancellationTokenSource.Token);

            // Form Setup
            Form = mainForm;

            Model = new MainFormModel(Form, processQueue);
            Form.mainFormModelBindingSource.DataSource = Model;

            UpdateController.Instance.UpdateCheckFinished += Instance_UpdateCheckFinished;

            if (!Settings.Default.MinimizeToTray)
            {
                Form.systemTrayNotifyIcon.Visible = false;
            }

            periodicSaveTimer.Interval = (int)PERIODIC_SAVE_INTERVAL.TotalMilliseconds;
            periodicSaveTimer.Tick += PeriodicSaveTimer_Tick;
        }

        public async Task LoadTrackers()
        {
            var (threads, boards) = await DataController.Load();

            foreach (var boardData in boards)
            {
                var board = Utils.CreateBoardFromData(boardData);
                AddNewTracker(board);
            }

            foreach (var threadData in threads)
            {
                var thread = Utils.CreateThreadFromData(threadData);
                AddNewTracker(thread);
            }

            periodicSaveTimer.Start();

            // Check for updates.
            if (Settings.Default.CheckForUpdatesOnStart)
            {
                UpdateController.Instance.CheckForUpdates(false);
            }
        }

        public void AddUrls(IEnumerable<string> urls)
        {
            var addedTrackers = new List<Tracker>();

            foreach (var url in urls)
            {
                var newTracker = Utils.CreateNewTracker(url);

                if (newTracker != null)
                {
                    if (IsUnique(newTracker))
                    {
                        AddNewTracker(newTracker);
                        addedTrackers.Add(newTracker);
                    }
                    else
                    {
                        var result = MessageBox.Show(
                            "URL is already being tracked!\nOpen corresponding folder?",
                            "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                        if (result == DialogResult.Yes)
                        {
                            var path = newTracker.SaveTo;

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            Utils.OpenDirectory(path);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Entered text '{url}' is not a supported site or board/thread!");
                }
            }

            if (addedTrackers.Count > 0)
            {
                // Save trackers the user adds straight away, so they survive the process being killed.
                // Threads found by board scraping are left to the periodic save, which saves them together with the board's GreatestThreadId.
                _ = Persist(() => DataController.Save(addedTrackers.OfType<Thread>(), addedTrackers.OfType<Board>()));
            }
        }

        /// <summary>
        /// Called by the process queue (from a processing thread) for threads found by board scraping.
        /// </summary>
        private void AddTrackerIfNew(Tracker tracker)
        {
            // Check and add in one step on the UI thread, so two boards finding the same thread can't both add it.
            OnUiThread(() =>
            {
                if (IsUnique(tracker))
                {
                    AddNewTracker(tracker);
                }
            });
        }

        /// <summary>Adds to the tracker list and the process queue. Must be called on the UI thread.</summary>
        private void AddNewTracker(Tracker tracker)
        {
            if (tracker is Board board)
            {
                Model.Boards.Add(board);
                processQueue.Enqueue(board);
            }
            else if (tracker is Thread thread)
            {
                Model.Threads.Add(thread);
                processQueue.Enqueue(thread);
            }
        }

        internal async Task ClearTrackers(Type type)
        {
            var typeName = type.ToString().ToLower() + "s";

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to clear all {typeName}?",
                $"Clear all {typeName}?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (confirmResult == DialogResult.Yes)
            {
                if (type == Type.Thread)
                {
                    // Threads can fail to removal (e.g. I/O), so try to remove all, not saving to database on each, then do 1 db operation.
                    var threads = Model.Threads.ToArray();
                    var removedThreads = await threads.ParallelWhereAsync(async t => await RemoveThread(t, manualRemoval: true, saveToDatabase: false));
                    await TryDatabase(() => DataController.RemoveThreads(removedThreads));
                }
                else // Boards
                {
                    // Boards are less likely to fail removal (no I/O upon removal), so no special error handling here.
                    while (Model.Boards.Count > 0)
                    {
                        await RemoveBoard(Model.Boards.Last(), saveToDatabase: false);
                    }

                    await TryDatabase(DataController.RemoveAllBoards);
                }
            }
        }

        /// <summary>
        /// Returns true if <paramref name="tracker"/> is not already in its tracker list. Must be called on the UI thread.
        /// </summary>
        private bool IsUnique(Tracker tracker)
        {
            if (tracker is Thread thread)
            {
                return !Model.Threads.Any(t =>
                    t.Site == thread.Site &&
                    t.BoardCode == thread.BoardCode &&
                    t.Id == thread.Id
                );
            }
            else // Board
            {
                return !Model.Boards.Any(t => t.Url == tracker.Url);
            }
        }

        public void RenameThreadSubjectPrompt(int threadBindingSourceIndex)
        {
            var thread = Model.Threads[threadBindingSourceIndex];
            var dialog = new GetStringMessageBox(thread.Subject)
            {
                StartPosition = FormStartPosition.CenterParent
            };

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string newSubject = string.IsNullOrWhiteSpace(dialog.UserEntry) ? Thread.NO_SUBJECT : dialog.UserEntry;

                thread.Subject = newSubject;
                _ = Persist(() => DataController.Save([thread], []));
            }
        }

        public async Task RemoveBoard(Board board, bool saveToDatabase = true)
        {
            board.Cancel();

            if (saveToDatabase)
            {
                await TryDatabase(() => DataController.RemoveBoard([board]));
            }

            OnUiThread(() => Model.Boards.Remove(board));
        }

        public void SettingsUpdated()
        {
            // Save everything now, so trackers added before saving was turned on are kept too.
            _ = SaveAll();
        }

        public async Task ThreadRemoveSelf(Thread thread)
        {
            await RemoveThread(thread, manualRemoval: false, saveToDatabase: true);
        }

        /// <summary>
        /// Remove a thread from tracking.
        /// </summary>
        /// <param name="thread">Thread to remove.</param>
        /// <param name="manualRemoval">Was this remove initiated by the user or by the scanning routine.</param>
        /// <remarks>
        /// TODO: RemoveThread likely to throw. Info below:
        /// If removing a newly added thread this is likely to fail beacuse files are still being downloaded into the current directory.
        /// It only fails once because after setting thread.Scraping to no more downloads happen (mostly..).
        /// We kind of need a download manager so we can wait for all downloads from this thread to finish before moving dir.
        /// Comment update in bugfix/rate-limiting branch: This is remedied now with a CancellationToken on the thread and files. But we need >= .NET 5 to use cancellation tokens on the ReadAsStringAsync methods.
        /// </remarks>
        /// <returns>Returns true if removal successful.</returns>
        public async Task<bool> RemoveThread(Thread thread, bool manualRemoval = false, bool saveToDatabase = true)
        {
            logger.Trace($"Removing thread {thread}.");
            thread.Cancel();

            if (saveToDatabase)
            {
                await TryDatabase(() => DataController.RemoveThreads([thread]));
            }

            try
            {
                if (Settings.Default.AddThreadSubjectToFolder)
                {
                    Utils.MoveThread(thread);
                }

                // Remove on UI thread because this method can be called from non-ui thread.
                OnUiThread(() => Model.Threads.Remove(thread));
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Exception occured attempting to remove thread {thread}.");

                if (manualRemoval)
                {
                    MessageBox.Show(
                        $"An error occured when trying to remove the thread {thread.Subject} ({thread.Id}). Please check the logs file in the ProgramData folder for more information.",
                        "Thread Removal Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1
                    );
                }

                return false;
            }

            return true;
        }

        private void Instance_UpdateCheckFinished(object sender, CheckForUpdatesResult result, bool initiatedByUser)
        {
            if (result.CanUpdate)
            {
                Form.updateAvailableToolStripMenuItem.Visible = true;
            }
            else
            {
                // Only show notification if the update check was initiated by the user.
                if (initiatedByUser)
                {
                    Form.toolTip.Show("No Updates Available.", Form.menuStrip, 70, 20, 1750);
                }
            }
        }

        /// <summary>
        /// Asks the user to confirm closing if needed, then stops processing and saves.
        /// </summary>
        /// <returns>True to cancel closing, false to go ahead with closing.</returns>
        // TODO: This method has 2 functions, warning if needed and cleanup, seperate into 2 responsibilities.
        public async Task<bool> Closing()
        {
            if (Settings.Default.WarnOnClose && Model.Threads.Count > 0)
            {
                using var closeDialog = new CloseWarn();
                var dialogResult = closeDialog.ShowDialog();

                if (dialogResult == DialogResult.Cancel)
                {
                    return true;
                }
            }

            // Only shut down processing once closing is certain, as it can't be restarted.
            periodicSaveTimer.Stop();
            cancellationTokenSource.Cancel();

            Form.systemTrayNotifyIcon.Visible = false;
            Form.systemTrayNotifyIcon.Dispose();

            await SaveAll();

            return false;
        }

        internal void PauseQueue()
        {
            logger.Info("Pausing Queue.");
            processQueue.Pause = true;
        }

        internal void ResumeQueue()
        {
            logger.Info("Resuming Queue.");
            processQueue.Pause = false;
        }

        private async void PeriodicSaveTimer_Tick(object sender, EventArgs e)
        {
            if (periodicSaveRunning)
            {
                return;
            }

            periodicSaveRunning = true;

            try
            {
                await SaveAll();
            }
            finally
            {
                periodicSaveRunning = false;
            }
        }

        /// <summary>
        /// Saves all trackers, if lists are being saved. Must be called on the UI thread, where the lists are snapshotted.
        /// </summary>
        private Task SaveAll()
        {
            var threads = Model.Threads.ToArray();
            var boards = Model.Boards.ToArray();

            return Persist(() => DataController.Save(threads, boards));
        }

        /// <summary>
        /// Runs a database write if lists are being saved (<see cref="Settings.SaveListsOnClose"/>).
        /// Failures are logged rather than thrown, so a database problem never interrupts tracking.
        /// </summary>
        private Task Persist(Func<Task> write)
        {
            return Settings.Default.SaveListsOnClose ? TryDatabase(write) : Task.CompletedTask;
        }

        /// <summary>
        /// Runs a database operation, logging rather than throwing any failure.
        /// </summary>
        private async Task TryDatabase(Func<Task> operation)
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception occured accessing the database.");
            }
        }

        /// <summary>
        /// Runs <paramref name="action"/> on the UI thread and waits for it. Does nothing once the form is closed.
        /// </summary>
        private void OnUiThread(Action action)
        {
            if (Form.IsDisposed || Form.Disposing)
            {
                return;
            }

            if (!Form.InvokeRequired)
            {
                action();
                return;
            }

            try
            {
                Form.Invoke(action);
            }
            catch (Exception ex) when (ex is ObjectDisposedException || (ex is InvalidOperationException && (Form.IsDisposed || !Form.IsHandleCreated)))
            {
                // The form closed between the check above and the invoke.
            }
        }
    }
}
