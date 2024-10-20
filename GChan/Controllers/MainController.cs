using GChan.Data;
using GChan.Forms;
using GChan.Models.Trackers;
using GChan.Properties;
using GChan.Services;
using GChan.ViewModels;
using NLog;
using Onova.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysThread = System.Threading.Thread;
using Thread = GChan.Models.Trackers.Thread;
using Type = GChan.Models.Trackers.Type;

namespace GChan.Controllers
{
    class MainController
    {
        internal readonly MainForm Form;
        internal readonly MainFormModel Model;

        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly ProcessQueue processQueue;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

#if DEBUG
        /// Define special lock objects when in DEBUG mode that print when/where they are acquired.
        /// Credit: https://stackoverflow.com/a/36487032/8306962

        readonly object threadLock = new();

        object ThreadLock 
        { 
            get 
            {
                var frame = new StackFrame(1, true);
                logger.Trace($"threadLock acquired by: {frame.GetMethod().Name} at line {frame.GetFileLineNumber()} on thread {SysThread.CurrentThread.ManagedThreadId}.");
                return threadLock;
            } 
        }

        readonly object boardLock = new();

        object BoardLock
        {
            get
            {
                var frame = new StackFrame(1, true);
                logger.Trace($"boardLock acquired by: {frame.GetMethod().Name} at line {frame.GetFileLineNumber()} on thread {SysThread.CurrentThread.ManagedThreadId}.");
                return boardLock;
            }
        }
#else
        readonly object ThreadLock = new();
        readonly object BoardLock = new();
#endif

        public MainController(MainForm mainForm)
        {
            // Controller Setup
            processQueue = new(AddTrackerIfNew, cancellationTokenSource.Token);

            // Form Setup
            Form = mainForm;

            Model = new MainFormModel(Form);
            Form.mainFormModelBindingSource.DataSource = Model;

            UpdateController.Instance.UpdateCheckFinished += Instance_UpdateCheckFinished;

            if (!Settings.Default.MinimizeToTray)
            { 
                Form.systemTrayNotifyIcon.Visible = false;
            }
        }

        public async Task LoadTrackers()
        {
            var (threads, boards) = await DataController.Load();

            lock (BoardLock)
            {
                for (int i = 0; i < boards.Count; i++)
                {
                    var board = Utils.CreateBoardFromData(boards[i]);
                    AddNewTracker(board);
                }
            }

            foreach (var threadData in threads)
            {
                var thread = Utils.CreateThreadFromData(threadData);
                AddNewTracker(thread);
            }

            // Check for updates.
            if (Settings.Default.CheckForUpdatesOnStart)
            {
                UpdateController.Instance.CheckForUpdates(false);
            }
        }

        public void AddUrls(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            { 
                var newTracker = Utils.CreateNewTracker(url);

                if (newTracker != null)
                {
                    var trackerList = ((newTracker.Type == Type.Board) ? Model.Boards.Cast<Tracker>() : Model.Threads.Cast<Tracker>()).ToArray();

                    if (IsUnique(newTracker, trackerList))
                    {
                        AddNewTracker(newTracker);
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
        }

        // TODO: Bit of code duplication here with the above method, remedy eventually.
        private void AddTrackerIfNew(Tracker tracker)
        {
            var trackerList = ((tracker.Type == Type.Board) ? Model.Boards.Cast<Tracker>() : Model.Threads.Cast<Tracker>()).ToArray();
            
            if (IsUnique(tracker, trackerList))
            {
                AddNewTracker(tracker);
            }
        }

        /// <summary>Will also add the the process queue.</summary>
        /// <returns>Was <paramref name="tracker"/> added to a tracker list.</returns>
        private bool AddNewTracker(Tracker tracker)
        {
            if (tracker == null)
            { 
                return false;
            }

            if (tracker is Board board)
            {
                lock (BoardLock)
                {
                    Form.Invoke(() => Model.Boards.Add(board) );
                    processQueue.Enqueue(board);
                }
            }
            else if (tracker is Thread thread)
            {
                lock (ThreadLock)
                {
                    Form.Invoke(() => Model.Threads.Add(thread));
                    processQueue.Enqueue(thread);
                }
            }

            return true;
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
                    while (Model.Threads.Count > 0)
                    {
                        await RemoveThread(Model.Threads.Last());
                    }
                }
                else // Boards
                {
                    while (Model.Boards.Count > 0)
                    {
                        await RemoveBoard(Model.Boards.Last());
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if a url is not contained within a list of trackers. 
        /// </summary>
        private bool IsUnique(Tracker tracker, IEnumerable<Tracker> list)
        {
            if (tracker is Thread thread)
            {
                return !list.OfType<Thread>().Any(t => 
                    t.Site == thread.Site &&
                    t.BoardCode == thread.BoardCode &&
                    t.Id == thread.Id
                );
            }
            else // Board
            {
                return !list.OfType<Board>().Any(t => t.Url == tracker.Url);
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
            }
        }

        public async Task RemoveBoard(Board board)
        {
            board.Cancel();

            await DataController.RemoveBoard(board);

            lock (BoardLock)
            {
                Form.Invoke(() =>
                {
                    Model.Boards.Remove(board);
                });
            }
        }

        public void SettingsUpdated()
        {

        }

        /// <summary>
        /// Remove a thread from tracking.
        /// </summary>
        /// <param name="thread">Thread to remove.</param>
        /// <param name="manualRemove">Was this remove initiated by the user or by the scanning routine.</param>
        /// <remarks>
        /// TODO: RemoveThread likely to throw. Info below:
        /// If removing a newly added thread this is likely to fail beacuse files are still being downloaded into the current directory.
        /// It only fails once because after setting thread.Scraping to no more downloads happen (mostly..).
        /// We kind of need a download manager so we can wait for all downloads from this thread to finish before moving dir.
        /// Comment update in bugfix/rate-limiting branch: This is remedied now with a CancellationToken on the thread and files. But we need >= .NET 5 to use cancellation tokens on the ReadAsStringAsync methods.
        /// </remarks>
        public async Task RemoveThread(Thread thread, bool manualRemove = false)
        {
            logger.Trace($"Removing thread {thread}.");
            thread.Cancel();
            await DataController.RemoveThread(thread);

            try
            {
                if (Settings.Default.AddThreadSubjectToFolder)
                {
                    Utils.MoveThread(thread);
                }

                lock (ThreadLock)
                {
                    // Remove on UI thread because this method can be called from non-ui thread.
                    Form.Invoke(() =>
                    {
                        Model.Threads.Remove(thread);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Exception occured attempting to remove thread {thread}.");

                if (manualRemove)
                {
                    MessageBox.Show(
                        $"An error occured when trying to remove the thread {thread.Subject} ({thread.Id}). Please check the logs file in the ProgramData folder for more information.", 
                        "Thread Removal Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error, 
                        MessageBoxDefaultButton.Button1);
                }
            }
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
        /// Returns true to cancel closing, false to go ahead with closing.
        /// </summary>
        // TODO: This method has 2 functions, warning if needed and cleanup, seperate into 2 responsibilities.
        public async Task<bool> Closing()
        {
            cancellationTokenSource.Cancel();

            if (Settings.Default.WarnOnClose && Model.Threads.Count > 0)
            {
                using var closeDialog = new CloseWarn();
                var dialogResult = closeDialog.ShowDialog();

                if (dialogResult == DialogResult.Cancel)
                {
                    return true;
                }
            }

            Form.systemTrayNotifyIcon.Visible = false;
            Form.systemTrayNotifyIcon.Dispose();

            if (Settings.Default.SaveListsOnClose)
            {
                await DataController.Save(Model.Threads.ToArray(), Model.Boards.ToArray());
            }

            return false;
        }
    }
}
