using GChan.Data;
using GChan.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GChan
{
    internal static class Program
    {
        public static MainForm mainForm;
        public static string APPLICATION_INSTALL_DIRECTORY { get; } = AppDomain.CurrentDomain.BaseDirectory;

#if DEBUG
        public static string PROGRAM_DATA_PATH => Path.Combine(Path.GetDirectoryName(Application.CommonAppDataPath), "DEBUG");
#else
        public static string PROGRAM_DATA_PATH => Path.GetDirectoryName(Application.CommonAppDataPath);
#endif
        /// <summary>
        /// Store boards in root ProgramData folder, not in version folder, so it can be accessed across upgrades.
        /// </summary>
        public static string BOARDS_PATH => Path.Combine(PROGRAM_DATA_PATH, "boards.dat");
        /// <summary>
        /// Store threads in root ProgramData folder, not in version folder, so it can be accessed across upgrades.
        /// </summary>
        public static string THREADS_PATH => Path.Combine(PROGRAM_DATA_PATH, "threads.dat");
        /// <summary>
        /// Store crash logs in version folder to help with finding the correct file with less useless info in it.
        /// </summary>
        public static string LOGS_PATH => Path.Combine(Application.CommonAppDataPath, "crash.logs");

        public const string NAME = "GChan";

        public const string GITHUB_REPOSITORY_OWNER = "Issung";

#if DEBUG
        public const string GITHUB_REPOSITORY_NAME = "GChanUpdateTesting";
#else
        public const string GITHUB_REPOSITORY_NAME = "GChan";
#endif

        public static StreamWriter streamWriter;

        public const string TRAY_CMDLINE_ARG = "-tray";

        public static string[] arguments;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            arguments = args;

#if DEBUG
            Directory.CreateDirectory(PROGRAM_DATA_PATH);
#endif

            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Reload();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }

            // See if an instance is already running...
            // Code from https://stackoverflow.com/a/1777704 by Matt Davis (https://stackoverflow.com/users/51170/matt-davis)
            if (_single.WaitOne(TimeSpan.Zero, true))
            {
                // Unhandled exceptions for our Application Domain
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);

                // Unhandled exceptions for the executing UI thread
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    mainForm = new MainForm();
                    Application.Run(mainForm);
                }
                catch
                {
                    // handle exception accordingly
                }
                finally
                {
                    _single.ReleaseMutex();
                }
            }
            else
            {
                // Bring already open instance to top and activate it.
                NativeMethods.PostMessage(
                    (IntPtr)HWND_BROADCAST,
                    WM_MY_MSG,
                    new IntPtr(0xCDCD),
                    new IntPtr(0xEFEF)
                );
            }
        }

        /// <summary>
        /// Main thread exception handler
        /// </summary>
        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Properties.Settings.Default.SaveListsOnClose = true;
            Properties.Settings.Default.Save();

            try
            {
                DataController.SaveAll(mainForm.Model.Threads, mainForm.Model.Boards);

                Log(true, $"Application_ThreadException - {e.Exception.Message}", e.Exception.StackTrace);
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message);
            }
        }

        /// <summary>
        /// Application domain exception handler
        /// </summary>
        public static void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            Properties.Settings.Default.SaveListsOnClose = true;
            Properties.Settings.Default.Save();

            try
            {
                DataController.SaveAll(mainForm.Model.Threads, mainForm.Model.Boards);

                Exception ex = (Exception)e.ExceptionObject;
                Log(true, $"AppDomain_UnhandledException - {ex.GetType().Name} - {ex.Message}", ex.StackTrace);
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message);
            }
        }

        private static void InitialiseStreamWriter()
        {
            streamWriter = new StreamWriter(LOGS_PATH, true)
            {
                AutoFlush = false
            };
        }

        public static void Log(Exception ex)
        {
            StackFrame frame = new StackFrame(1, true);
            Log(true, $"Logged Exception - {ex.GetType().Name} - {ex.Message} in method {frame.GetMethod().Name} at line {frame.GetFileLineNumber()}:{frame.GetFileColumnNumber()}", ex.StackTrace);
        }

        public static void Log(string message, Exception ex)
        {
            StackFrame frame = new StackFrame(1, true);
            Log(true, message, $"Logged Exception - {ex.GetType().Name} - {ex.Message} in method {frame.GetMethod().Name} at line {frame.GetFileLineNumber()}:{frame.GetFileColumnNumber()}", ex.StackTrace);
        }

        public static void Log(bool timestampFirstLine, params string[] lines)
        {
            try 
            {
                if (streamWriter == null)
                    InitialiseStreamWriter();

                if (timestampFirstLine && lines.Length > 0)
                    lines[0] = $"[{DateTime.Now}] - " + lines[0];

                for (int i = 0; i < lines.Length; i++)
                { 
                    streamWriter.WriteLine(lines[i]);
#if DEBUG
                    Console.WriteLine((i > 0 ? "\t" : "") + lines[i]);
#endif
                }

                streamWriter.Flush();
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.ToString());
#endif
            }
        }

#region Dll Imports

        private const int HWND_BROADCAST = 0xFFFF;
        public static readonly int WM_MY_MSG = NativeMethods.RegisterWindowMessage("WM_MY_MSG");
        private static readonly Mutex _single = new Mutex(true, "GChanRunning");

        private class NativeMethods
        {
            [DllImport("user32")]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

            [DllImport("user32", CharSet = CharSet.Unicode)]
            public static extern int RegisterWindowMessage(string message);
        }

#endregion Dll Imports
    }
}