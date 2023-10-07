using GChan.Data;
using GChan.Forms;
using GChan.Helpers;
using GChan.Models;
using GChan.Properties;
using GChan.Trackers;
using NLog;
using Onova.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysThread = System.Threading.Thread;
using Thread = GChan.Trackers.Thread;
using Timer = System.Windows.Forms.Timer;
using Type = GChan.Trackers.Type;

namespace GChan.Controllers
{
    class MainController
    {
        internal MainForm Form;

        internal MainFormModel Model;

        private SysThread scanThread = null;

        private readonly DownloadManager<ImageLink> imageDownloadManager = new();

        private readonly Timer scanTimer = new();

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public int ScanTimerInterval { get { return scanTimer.Interval; } set { scanTimer.Interval = value; } }

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
            Form = mainForm;

            Model = new MainFormModel(Form);
            Form.mainFormModelBindingSource.DataSource = Model;

            UpdateController.Instance.UpdateCheckFinished += Instance_UpdateCheckFinished;

            if (!Settings.Default.MinimizeToTray)
            { 
                Form.systemTrayNotifyIcon.Visible = false;
            }

            scanTimer.Enabled = false;
            scanTimer.Interval = Settings.Default.ScanTimer;
            scanTimer.Tick += new EventHandler(StartScanThread);
        }

        public void LoadTrackers()
        {
            ///Require the save on close setting to be true to load threads on application open.
            const bool requireSaveOnCloseToBeTrueToLoadThreadsAndBoards = true;

            if (!requireSaveOnCloseToBeTrueToLoadThreadsAndBoards || Settings.Default.SaveListsOnClose)              // If enabled load URLs from file
            {
                var boards = DataController.LoadBoards();
                var threads = DataController.LoadThreads();

                lock (BoardLock)
                {
                    for (int i = 0; i < boards.Count; i++)
                    {
                        Board newBoard = (Board)Utils.CreateNewTracker(boards[i]);
                        AddNewTracker(newBoard);
                    }
                }

                new SysThread(() =>
                {
                    Parallel.ForEach(threads, (thread) =>
                    {
                        Thread newThread = (Thread)Utils.CreateNewTracker(thread);
                        Form.BeginInvoke(new Action(() => { AddNewTracker(newThread); }));
                    });

                    Form.Invoke((MethodInvoker)delegate {
                        FinishLoadingTrackers();
                    });
                }).Start();
            }

            /// Executed once everything has finished being loaded.
            void FinishLoadingTrackers()
            {
                scanTimer.Enabled = true;
                StartScanThread(this, new EventArgs());

                // Check for updates.
                if (Settings.Default.CheckForUpdatesOnStart)
                {
                    UpdateController.Instance.CheckForUpdates(false);
                }
            }
        }

        public void AddUrls(IEnumerable<string> urls)
        {
            bool trackerWasAdded = false;
            foreach (var url in urls)
            { 
                var newTracker = Utils.CreateNewTracker(url);

                if (newTracker != null)
                {
                    var trackerList = ((newTracker.Type == Type.Board) ? Model.Boards.Cast<Tracker>() : Model.Threads.Cast<Tracker>()).ToArray();

                    if (IsUnique(newTracker, trackerList))
                    {
                        var addSuccess = AddNewTracker(newTracker);
                        if (addSuccess)
                        { 
                            trackerWasAdded = true;
                        }
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
                            Process.Start(path);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Entered text '{url}' is not a supported site or board/thread!");
                }
            }

            if (trackerWasAdded)
            {
                scanTimer.Enabled = true;
                StartScanThread(this, new EventArgs());
            }
        }

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
                    Form.Invoke((MethodInvoker)delegate () 
                    {
                        Model.Boards.Add(board);
                    });
                }
            }
            else if (tracker is Thread thread)
            {
                lock (ThreadLock)
                {
                    Form.Invoke((MethodInvoker)delegate () 
                    {
                        Model.Threads.Add(thread);
                    });
                }
            }

            return true;
        }

        internal void ClearTrackers(Type type)
        {
            string typeName = type.ToString().ToLower() + "s";

            DialogResult dialogResult = MessageBox.Show(
                $"Are you sure you want to clear all {typeName}?",
                $"Clear all {typeName}?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);    // Confirmation prompt

            if (dialogResult == DialogResult.Yes)
            {
                if (type == Type.Thread)
                {
                    lock (ThreadLock)
                    {
                        for (int i = Model.Threads.Count - 1; i >= 0; i--)
                        {
                            RemoveThread(Model.Threads[i], true);
                        }
                    }
                }
                else // Boards
                {
                    lock (BoardLock)
                    {
                        for (int i = Model.Boards.Count - 1; i >= 0; i--)
                        {
                            RemoveBoard(Model.Boards[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Run the scan thread if it isn't already running.
        /// </summary>
        private void StartScanThread(object sender, EventArgs e)
        {
            if (scanThread == null || !scanThread.IsAlive)
            {
                scanThread = new SysThread(new ThreadStart(ScanRoutine))
                {
                    Name = "Scan Thread",
                    IsBackground = true
                };

                scanThread.Start();
            }
        }

        /// <summary>
        /// Thread entry-point.
        /// </summary>
        private void ScanRoutine()
        {
            lock (ThreadLock)
            {
                // Remove 404'd threads
                var removedThreads = Model.Threads.RemoveAll(t => t.Gone);
                removedThreads.ForEach(t => RemoveThread(t));
            }

            // Make a copy of the current boards and scrape them for new threads.
            var boards = Model.Boards.ToArray();

            foreach (var board in boards)
            {
                if (board.Scraping)
                {
                    var threadUrls = board.GetThreadLinks();
                    var greatestThreadIdLock = new object();
                    var greatestThreadId = 0;

                    Parallel.ForEach(threadUrls, (threadUrl) =>
                    {
                        if (board.Scraping)
                        {
                            var id = GetThreadId(board, threadUrl);

                            if (id != null && id > board.GreatestThreadId)
                            {
                                var newThread = (Thread)Utils.CreateNewTracker(threadUrl);

                                if (newThread != null && IsUnique(newThread, Model.Threads))
                                {
                                    var urlWasAdded = AddNewTracker(newThread);

                                    if (urlWasAdded)
                                    {
                                        lock (greatestThreadIdLock)
                                        { 
                                            if (id > greatestThreadId)
                                            {
                                                greatestThreadId = id.Value;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                    board.GreatestThreadId = greatestThreadId;
                }
            }

            // Make a copy of the current threads and download them.
            var threads = Model.Threads.ToArray();

            for (int i = 0; i < threads.Length; i++)
            {
                if (threads[i].Scraping)
                {
                    var links = threads[i].GetImageLinks();
                    imageDownloadManager.Queue(links);
                }
            }
        }

        public int? GetThreadId(Board board, string url)
        {
            try
            {
                var idCodeMatch = board.SiteName switch
                {
                    Board_4Chan.SITE_NAME => Regex.Match(url, Thread_4Chan.ID_CODE_REGEX),
                    Board_8Kun.SITE_NAME => Regex.Match(url, Thread_8Kun.ID_CODE_REGEX),
                    _ => null,
                };

                if (idCodeMatch?.Groups.Count > 0)
                {
                    return int.Parse(idCodeMatch.Groups[0].Value);
                }
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// Returns true if a url is not contained within a list of trackers. 
        /// </summary>
        private bool IsUnique(Tracker tracker, IEnumerable<Tracker> list)
        {
            if (tracker is Thread thread)
            {
                return !list.OfType<Thread>().Any(t => 
                    t.SiteName == thread.SiteName &&
                    t.BoardCode == thread.BoardCode &&
                    t.ID == thread.ID
                );
            }
            else // Board
            {
                return !list.OfType<Board>().Any(t => t.Url == tracker.Url);
            }
        }

        public void RenameThreadSubjectPrompt(int threadBindingSourceIndex)
        {
            Thread thread = Model.Threads[threadBindingSourceIndex];
            GetStringMessageBox dialog = new GetStringMessageBox(thread.Subject)
            {
                StartPosition = FormStartPosition.CenterParent
            };

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string newSubject = string.IsNullOrWhiteSpace(dialog.UserEntry) ? Thread.NO_SUBJECT : dialog.UserEntry;

                thread.Subject = newSubject;
            }
        }

        public void RemoveBoard(Board board)
        {
            board.Scraping = false;

            lock (BoardLock)
            {
                Form.Invoke((MethodInvoker)delegate 
                {
                    Model.Boards.Remove(board);
                });
            }
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
        /// </remarks>
        public void RemoveThread(Thread thread, bool manualRemove = false)
        {
            logger.Info($"Removing thread {thread}.");

            thread.Scraping = false;

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
                        $"An error occured when trying to remove the thread {thread.Subject} ({thread.ID}). Please check the logs file in the ProgramData folder for more information.", 
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
        public bool Closing()
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

            // TODO: Call cancel scraping method.
            Form.systemTrayNotifyIcon.Visible = false;
            Form.systemTrayNotifyIcon.Dispose();
            scanTimer.Enabled = false;
            scanTimer.Dispose();

            if (Settings.Default.SaveListsOnClose)
            {
                DataController.SaveAll(Model.Threads.ToArray(), Model.Boards.ToArray());
            }

            return false;
        }
    }
}
