/************************************************************************
 * Copyright (C) 2015 by themirage <mirage@secure-mail.biz>             *
 *                                                                      *
 * This program is free software: you can redistribute it and/or modify *
 * it under the terms of the GNU General Public License as published by *
 * the Free Software Foundation, either version 3 of the License, or    *
 * (at your option) any later version.                                  *
 *                                                                      *
 * This program is distributed in the hope that it will be useful,      *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 * GNU General Public License for more details.                         *
 *                                                                      *
 * You should have received a copy of the GNU General Public License    *
 * along with this program.  If not, see <http://www.gnu.org/licenses/> *
 ************************************************************************/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace GChan
{
    internal static class Program
    {
        public static MainForm mainForm;
        public static string APPLICATION_INSTALL_DIRECTORY { get; } = AppDomain.CurrentDomain.BaseDirectory;

#if DEBUG
        public static string BOARDS_PATH => Path.Combine(Application.CommonAppDataPath, "DEBUG", "boards.dat");
        public static string THREADS_PATH => Path.Combine(Application.CommonAppDataPath, "DEBUG", "threads.dat");
        public static string LOGS_PATH => Path.Combine(Application.CommonAppDataPath, "DEBUG", "crash.logs");
#else
        public static string BOARDS_PATH => Path.Combine(Application.CommonAppDataPath, "boards.dat");
        public static string THREADS_PATH => Path.Combine(Application.CommonAppDataPath, "threads.dat");
        public static string LOGS_PATH { get; } = Path.Combine(Application.CommonAppDataPath, "crash.logs");
#endif

        public static readonly string NAME = "GChan";

        public static readonly string GITHUB_REPOSITORY_OWNER = "Issung";

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
            Directory.CreateDirectory(Path.Combine(Application.CommonAppDataPath, "DEBUG"));
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
                Utils.SaveURLs(mainForm.Model.Boards, mainForm.Model.Threads);

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
                Utils.SaveURLs(mainForm.Model.Boards, mainForm.Model.Threads);

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
            streamWriter = new StreamWriter(LOGS_PATH, true);
            streamWriter.AutoFlush = false;
        }

        public static void Log(Exception ex)
        {
            Log(true, $"Logged Exception - {ex.GetType().Name} - {ex.Message}", ex.StackTrace);
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
            catch (Exception)
            { 
                
            }
        }

        #region Dll Imports

        private const int HWND_BROADCAST = 0xFFFF;
        public static readonly int WM_MY_MSG = NativeMethods.RegisterWindowMessage("WM_MY_MSG");
        private static Mutex _single = new Mutex(true, "GChanRunning");

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