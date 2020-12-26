using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using SysThread = System.Threading.Thread;
using GChan.Trackers;
using Thread = GChan.Trackers.Thread;
using Type = GChan.Trackers.Type;
using GChan.Models;
using GChan.Controllers;
using URLType = GChan.Trackers.Type;
using GChan.Forms;

namespace GChan.Controllers
{
    class MainController
    {
        internal MainForm Form;

        internal MainFormModel Model;

        private SysThread Scanner = null;   // TODO: Rename to lowercase "scanner".

        private System.Windows.Forms.Timer scanTimer = new System.Windows.Forms.Timer();     // Timer for scanning

        public int ScanTimerInterval { get { return scanTimer.Interval; } set { scanTimer.Interval = value; } }

#if DEBUG
        private object _threadLock = new object();

        /// <summary>
        /// Credit: https://stackoverflow.com/a/36487032/8306962
        /// </summary>
        object threadLock { get {
                StackFrame frame = new StackFrame(1);
                Trace.WriteLine(String.Format("Lock acquired by: {0} on thread {1}", frame.GetMethod().Name, SysThread.CurrentThread.ManagedThreadId));
                return _threadLock;
            } 
        }
#else
        object threadLock = new object();
#endif

        private object boardLock = new object();

        /// <summary>
        /// Flag for whether or not the last update check was initiated by the user.
        /// False for automatic program start-up update check.
        /// </summary>
        private bool updateCheckWasManual = false;

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
                string boards = Utils.LoadURLs(true);
                string threads = Utils.LoadURLs(false);

                if (!String.IsNullOrWhiteSpace(boards))
                {
                    lock (boardLock)
                    {
                        string[] URLs = boards.Split('\n');
                        for (int i = 0; i < URLs.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(URLs[i]))
                            {
                                Board newBoard = (Board)Utils.CreateNewTracker(URLs[i].Trim());
                                AddURLToList(newBoard);
                            }
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(threads))
                {
                    string[] URLs = threads.Split('\n');

                    new SysThread(() =>
                    {
                        // Without setting ScrollBars to None and then setting to Vertical the program will crash.
                        // It doesnt like items being added in parallel in this method...
                        // User cannot also resize columns while adding, or else program crashes.
                        // TODO: Just move these to outside of the thread call so they don't require an Invoke?

                        MainForm.CheckForIllegalCrossThreadCalls = false;
                        Form.Invoke((MethodInvoker)delegate { Form.threadGridView.ScrollBars = ScrollBars.None; });
                        Form.threadGridView.AllowUserToResizeColumns = false;

                        // END TODO
                        lock (threadLock)
                        { 
                            Parallel.ForEach(URLs, (url) =>
                            {
                                if (!string.IsNullOrWhiteSpace(url))
                                {
                                    Thread newThread = (Thread)Utils.CreateNewTracker(url.Trim());
                                    AddURLToList(newThread);
                                }
                            });
                        }

                        Form.Invoke((MethodInvoker)delegate {
                            Done();
                        });
                    }).Start();
                }
                else
                {
                    Done();
                }
            }

            /// Executed once everything has finished being loaded.
            void Done()
            {
                Form.threadGridView.ScrollBars = ScrollBars.Vertical;
                Form.threadGridView.AllowUserToResizeColumns = true;

                scanTimer.Enabled = true;
                Scan(this, new EventArgs());

                // Check for updates.
                if (Properties.Settings.Default.CheckForUpdatesOnStart)
                {
                    UpdateController.Instance.CheckForUpdates();
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
                    if (Properties.Settings.Default.SaveListsOnClose)
                        Utils.SaveURLs(Model.Boards, Model.Threads);

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
                MessageBox.Show("Corrupt URL, unsupported website or not a board/thread!");
                Form.urlTextBox.Text = "";
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

        internal void RemoveBoard(int index)
        {
            lock (boardLock)
            {
                Form.Invoke((MethodInvoker)delegate { 
                    Model.Boards.RemoveAt(index);
                });
            }
        }

        private void Scan(object sender, EventArgs e)
        {
            if (Scanner == null || !Scanner.IsAlive)
            {
                Scanner = new SysThread(new ThreadStart(ScanThread))
                {
                    Name = "Scan Thread",
                    IsBackground = true
                };

                Scanner.Start();
            }
        }

        private void ScanThread()
        {
            //List<Imageboard> goneThreads = new List<Imageboard>();

            lock (threadLock)
            {
                // Removes 404'd threads
                /*Thread[] array = Model.Threads.ToArray();   //TODO: Is this necessary?
                for (int i = 0; i < array.Length; i++)
                {
                    Thread thread = array[i];

                    if (thread.Gone)
                    {
                        RemoveThread(thread);
                    }
                }

                if (Properties.Settings.Default.SaveListsOnClose)
                    Utils.SaveURLs(Model.Boards, Model.Threads.ToList());*/

                for (int i = 0; i < Model.Threads.Count; i++)
                {
                    if (Model.Threads[i].Gone)
                    {
                        RemoveThread(Model.Threads[i]);
                        i--;
                    }
                }

                if (Properties.Settings.Default.SaveListsOnClose)
                    Utils.SaveURLs(Model.Boards, Model.Threads.ToList());
            }

            lock (boardLock)
            {
                // Searches for new threads on the watched boards
                for (int i = 0; i < Model.Boards.Count; i++)
                {
                    string[] threads = { };

                    try
                    {
                        threads = Model.Boards[i].GetThreadLinks();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        for (int j = 0; j < threads.Length; j++)
                        {
                            Thread newThread = (Thread)Utils.CreateNewTracker(threads[j]);

                            if (newThread != null && IsUnique(newThread, Model.Threads))
                            {
                                AddURLToList(newThread);
                            }
                        }
                    }
                }
            }

            lock (threadLock)
            {
                // Download threads
                for (int i = 0; i < Model.Threads.Count; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Model.Threads[i].Download));
                }
            }
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
            GetStringMessageBox dialog = new GetStringMessageBox(thread.Subject);
            dialog.StartPosition = FormStartPosition.CenterParent;

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string newSubject = string.IsNullOrWhiteSpace(dialog.UserEntry) ? Thread.NO_SUBJECT : dialog.UserEntry;

                thread.SetSubject(newSubject);
            }
        }

        public void RemoveThread(Thread thread)
        {
            Program.Log(true, $"Removing thread {thread.URL}! thread.isGone: {thread.Gone}");

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

                    Program.Log(true, $"Directory.Moving {currentPath} to {destinationPath}");

                    Directory.Move(currentPath, calculatedDestination());
                }
                else
                {
                    Program.Log(true, $"While attempting to rename thread {thread.URL} the current folder could not be found, renaming abandoned.");
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

        private void Instance_UpdateCheckFinished(object sender, Onova.Models.CheckForUpdatesResult result)
        {
            if (result.CanUpdate)
            {
                Form.updateAvailableToolStripMenuItem.Visible = true;
            }
            else
            {
                // Only show notification if the update check was initiated by the user.
                if (updateCheckWasManual)
                    Form.toolTip.Show("No Updates Available.", Form.menuStrip, 70, 20, 1750);

                updateCheckWasManual = false;
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
                    Utils.SaveURLs(Model.Boards, Model.Threads.ToList());
            }

            return cancel;
        }
    }
}
