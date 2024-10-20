using GChan.Data;
using GChan.Forms;
using GChan.Properties;
using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows.Forms;

namespace GChan
{
    internal static class Program
    {
        public static MainForm mainForm;
        public static string APPLICATION_INSTALL_DIRECTORY { get; } = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// User data path at %localappdata%/gchan (no version).
        /// </summary>
        public static string USER_DATA_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
#if DEBUG
            $"{NAME}_DEBUG"
#else
            NAME
#endif
        );

        public static string LOGS_PATH = Path.Combine(USER_DATA_PATH, "logs");

        public const string NAME = "GChan";

        public const string GITHUB_REPOSITORY_OWNER = "Issung";

#if DEBUG
        public const string GITHUB_REPOSITORY_NAME = "GChanUpdateTesting";
#else
        public const string GITHUB_REPOSITORY_NAME = "GChan";
#endif

        public static readonly string VERSION = Assembly.GetEntryAssembly().GetName().Version.ToString();

        public const string TRAY_CMDLINE_ARG = "-tray";

        public static string[] arguments;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            arguments = args;

            Directory.CreateDirectory(LOGS_PATH);
            LogManager.Configuration.Variables["logsDirectory"] = LOGS_PATH;

            if (Settings.Default.UpdateSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.UpdateSettings = false;
                Settings.Default.Save();
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
        public static async void Application_ThreadException(object sender, ThreadExceptionEventArgs exceptionArgs)
        {
            Settings.Default.SaveListsOnClose = true;
            Settings.Default.Save();

            try
            {
                await DataController.Save(mainForm.Model.Threads, mainForm.Model.Boards);
                logger.Error(exceptionArgs.Exception, $"Application_ThreadException.");
            }
            catch (Exception ex)
            {
                var aggregate = new AggregateException(ex, exceptionArgs.Exception);
                logger.Fatal(aggregate, "Thread Exception.");
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Application domain exception handler
        /// </summary>
        public static async void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Settings.Default.SaveListsOnClose = true;
            Settings.Default.Save();

            try
            {
                await DataController.Save(mainForm.Model.Threads, mainForm.Model.Boards);
                Exception argsException = args.ExceptionObject as Exception;
                logger.Error(argsException, $"AppDomain_UnhandledException.");
            }
            catch (Exception ex)
            {
                var aggregate = new AggregateException(ex, args.ExceptionObject as Exception);
                logger.Fatal(aggregate, "Unhandled Exception.");
                MessageBox.Show(ex.Message);
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