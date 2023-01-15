using GChan.Data;
using GChan.Forms;
using GChan.Models;
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
using Type = GChan.Trackers.Type;

namespace GChan.Controllers
{
    class MainController
    {
        internal MainForm Form;

        internal MainFormModel Model;

        private SysThread scanThread = null;

        private readonly System.Windows.Forms.Timer scanTimer = new System.Windows.Forms.Timer();

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public int ScanTimerInterval { get { return scanTimer.Interval; } set { scanTimer.Interval = value; } }

#if DEBUG
        /// Define special lock objects when in DEBUG mode that print out when/where they are acquired.
        /// Credit: https://stackoverflow.com/a/36487032/8306962

        private object _threadLock = new object();

        object threadLock { get {
                StackFrame frame = new StackFrame(1, true);
                logger.Trace($"threadLock acquired by: {frame.GetMethod().Name} at line {frame.GetFileLineNumber()} on thread {SysThread.CurrentThread.ManagedThreadId}.");
                return _threadLock;
            } 
        }

        object _boardLock = new object();

        object boardLock
        {
            get
            {
                StackFrame frame = new StackFrame(1, true);
                logger.Trace($"boardLock acquired by: {frame.GetMethod().Name} at line {frame.GetFileLineNumber()} on thread {SysThread.CurrentThread.ManagedThreadId}.");
                return _boardLock;
            }
        }
#else
        readonly object threadLock = new object();
        readonly object boardLock = new object();
#endif

        public MainController(MainForm mainForm)
        {
            Form = mainForm;

            Model = new MainFormModel(Form);
            Form.mainFormModelBindingSource.DataSource = Model;

            UpdateController.Instance.UpdateCheckFinished += Instance_UpdateCheckFinished;

            if (!Properties.Settings.Default.MinimizeToTray)
                Form.systemTrayNotifyIcon.Visible = false;

            scanTimer.Enabled = false;
            scanTimer.Interval = Properties.Settings.Default.ScanTimer;
            scanTimer.Tick += new EventHandler(Scan);
        }

        public void LoadTrackers()
        {
            ///Require the save on close setting to be true to load threads on application open.
            const bool requireSaveOnCloseToBeTrueToLoadThreadsAndBoards = true;

            if (!requireSaveOnCloseToBeTrueToLoadThreadsAndBoards || Properties.Settings.Default.SaveListsOnClose)                                // If enabled load URLs from file
            {
                var boards = DataController.LoadBoards();
                var threads = DataController.LoadThreads();

                lock (boardLock)
                {
                    for (int i = 0; i < boards.Count; i++)
                    {
                        Board newBoard = (Board)Utils.CreateNewTracker(boards[i]);
                        AddURLToList(newBoard);
                    }
                }

                new SysThread(() =>
                {
                    // END TODO
                    Parallel.ForEach(threads, (thread) =>
                    {
                        Thread newThread = (Thread)Utils.CreateNewTracker(thread);
                        Form.BeginInvoke(new Action(() => { AddURLToList(newThread); }));
                    });

                    Form.Invoke((MethodInvoker)delegate {
                        Done();
                    });
                }).Start();
            }

            /// Executed once everything has finished being loaded.
            void Done()
            {
                scanTimer.Enabled = true;
                Scan(this, new EventArgs());

                // Check for updates.
                if (Properties.Settings.Default.CheckForUpdatesOnStart)
                {
                    UpdateController.Instance.CheckForUpdates(false);
                }
            }
        }

        public void AddUrl(string url)
        {
            Tracker newTracker = Utils.CreateNewTracker(url);

            if (newTracker != null)
            {
                List<Tracker> trackerList = ((newTracker.Type == Type.Board) ? Model.Boards.Cast<Tracker>() : Model.Threads.Cast<Tracker>()).ToList();

                if (IsUnique(newTracker, trackerList))
                {
                    AddURLToList(newTracker);

                    if (!scanTimer.Enabled)
                        scanTimer.Enabled = true;

                    Scan(this, new EventArgs());
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        "URL is already being tracked!\nOpen corresponding folder?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                    if (result == DialogResult.Yes)
                    {
                        string spath = newTracker.SaveTo;
                        if (!Directory.Exists(spath))
                            Directory.CreateDirectory(spath);
                        Process.Start(spath);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Entered text '{url}' is not a supported site or board/thread!");
            }
        }

        private bool AddURLToList(Tracker tracker)
        {
            if (tracker == null)
                return false;

            if (tracker.Type == Type.Board)
            {
                lock (boardLock)
                {
                    Form.Invoke((MethodInvoker)delegate () {
                        Model.Boards.Add((Board)tracker);
                    });
                }
            }
            else //Thread
            {
                lock (threadLock)
                {
                    Form.Invoke((MethodInvoker)delegate () {
                        Model.Threads.Add((Thread)tracker);
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
                    lock (threadLock)
                    {
                        for (int i = Model.Threads.Count - 1; i >= 0; i--)
                        {
                            RemoveThread(Model.Threads[i], true);
                        }
                    }
                }
                else // Boards
                {
                    lock (boardLock)
                    {
                        for (int i = Model.Boards.Count - 1; i >= 0; i--)
                        {
                            RemoveBoard(Model.Boards[i]);
                        }
                    }
                }
            }
        }

        private void Scan(object sender, EventArgs e)
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

        private void ScanRoutine()
        {
            lock (threadLock)
            {
                // Remove 404'd threads
                for (int i = 0; i < Model.Threads.Count; i++)
                {
                    if (Model.Threads[i].Gone)
                    {
                        RemoveThread(Model.Threads[i]);
                        i--;
                    }
                }
            }

            // Make a copy of the current boards and scrape them for new threads.
            var boards = Model.Boards.ToList();

            for (int i = 0; i < boards.Count; i++)
            {
                if (boards[i].Scraping)
                {
                    // OrderBy because we need the threads to be in ascending order by ID for LargestAddedThreadNo to be useful.
                    string[] boardThreadUrls = boards[i].GetThreadLinks().OrderBy(t => t).ToArray();
                    int largestNo = 0;

                    Parallel.ForEach(boardThreadUrls, (url) =>
                    {
                        if (boards[i].Scraping)
                        {
                            int? id = GetThreadID(boards[i], url);

                            if (id.HasValue)
                            {
                                if (id.Value > boards[i].LargestAddedThreadNo)
                                {
                                    Thread newThread = (Thread)Utils.CreateNewTracker(url);

                                    if (newThread != null && IsUnique(newThread, Model.Threads))
                                    {
                                        bool urlWasAdded = AddURLToList(newThread);

                                        if (urlWasAdded)
                                        {
                                            if (id.Value > largestNo)   //Not exactly safe in multithreaded but should work fine.
                                                largestNo = id.Value;
                                        }
                                    }
                                }
                            }
                        }
                    });

                    boards[i].LargestAddedThreadNo = largestNo;
                }
            }

            // Make a copy of the current threads and download them.
            var threads = Model.Threads.ToList();

            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].Scraping)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(threads[i].Download));
            }
        }

        public int? GetThreadID(Board board, string url)
        {
            try
            {
                Match idCodeMatch = null;

                if (board.SiteName == Board_4Chan.SITE_NAME_4CHAN)
                {
                    idCodeMatch = Regex.Match(url, Thread_4Chan.ID_CODE_REGEX);
                }
                else if (board.SiteName == Board_8Kun.SITE_NAME_8KUN)
                {
                    idCodeMatch = Regex.Match(url, Thread_8Kun.idCodeRegex);
                }

                if (idCodeMatch != null)
                    if (idCodeMatch.Groups.Count > 0)
                        return int.Parse(idCodeMatch.Groups[0].Value);
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
            if (tracker.Type == Type.Thread)
            {
                return !list.OfType<Thread>().Any(
                    t => t.SiteName == tracker.SiteName &&
                    t.BoardCode == tracker.BoardCode &&
                    t.ID == ((Thread)tracker).ID);
            }
            else // Board
            {
                return !list.OfType<Board>().Any(t => t.URL == tracker.URL);
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

            lock (boardLock)
            {
                Form.Invoke((MethodInvoker)delegate {
                    Model.Boards.Remove(board);
                });
            }
        }

        public void RemoveThread(Thread thread, bool manualRemove = false)
        {
            logger.Info($"Removing thread {thread}.");

            thread.Scraping = false;

            try
            {
                if (Properties.Settings.Default.AddThreadSubjectToFolder)
                {
                    string currentPath = thread.SaveTo.Replace("\r", "");

                    string cleanSubject = Utils.CleanSubjectString(thread.Subject);

                    // There are \r characters appearing from the custom subjects, TODO: need to get to the bottom of the cause of this.
                    string destinationPath;

                    if ((ThreadFolderNameFormat)Properties.Settings.Default.ThreadFolderNameFormat == ThreadFolderNameFormat.IdName)
                    {
                        destinationPath = (thread.SaveTo + " - " + cleanSubject);
                    }
                    else //NameId
                    {
                        destinationPath = Path.Combine(Path.GetDirectoryName(thread.SaveTo), $"{thread.Subject} - {thread.ID}");
                    }

                    destinationPath = destinationPath.Replace("\r", "").Trim('\\', '/');

                    if (Directory.Exists(currentPath))
                    {
                        int number = 0;
                        string numberText() => (number == 0) ? "" : $" ({number})";

                        string calculatedDestination() => destinationPath + numberText();

                        while (Directory.Exists(calculatedDestination()))
                        {
                            number++;
                        }

                        logger.Info($"Directory.Moving '{currentPath}' to '{destinationPath}'.");

                        Directory.Move(currentPath, calculatedDestination());
                    }
                    else
                    {
                        logger.Warn($"While attempting to rename thread {thread} the current folder could not be found, renaming abandoned.");
                    }
                }

                lock (threadLock)
                {
                    // Remove on UI thread because this method can be called from non-ui thread.
                    Form.Invoke((MethodInvoker)delegate () {
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
                        "Remove Thread Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                    Form.toolTip.Show("No Updates Available.", Form.menuStrip, 70, 20, 1750);
            }
        }

        /// <summary>
        /// Returns true to cancel closing.
        /// </summary>
        public bool Closing()
        {
            bool cancel = false;
            DialogResult result = DialogResult.OK;

            if (Properties.Settings.Default.WarnOnClose && Model.Threads.Count > 0)
            {
                CloseWarn clw = new CloseWarn();
                result = clw.ShowDialog();
                clw.Dispose();
            }

            if (result == DialogResult.Cancel)
            { 
                cancel = true;
            }
            else if (result == DialogResult.OK)
            {
                // TODO: Call cancel scraping method.
                Form.systemTrayNotifyIcon.Visible = false;
                Form.systemTrayNotifyIcon.Dispose();
                scanTimer.Enabled = false;

                if (Properties.Settings.Default.SaveListsOnClose)
                    DataController.SaveAll(Model.Threads.ToList(), Model.Boards);
            }

            return cancel;
        }
    }
}
