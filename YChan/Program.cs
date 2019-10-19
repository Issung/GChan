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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GChan
{
    internal static class Program
    {
        public static frmMain MainFrame;

        /// <summary>
        /// Main thread exception handler
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Properties.Settings.Default.saveOnClose = true;
            Properties.Settings.Default.Save();
            try
            {
                General.WriteURLs(MainFrame.ListBoards, MainFrame.ListThreads);
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message);
            }
        }

        /// <summary>
        /// Application domain exception handler
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        public static void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            Properties.Settings.Default.saveOnClose = true;
            Properties.Settings.Default.Save();
            try
            {
                General.WriteURLs(MainFrame.ListBoards, MainFrame.ListThreads);
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
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
                    MainFrame = new frmMain();
                    Application.Run(MainFrame);
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
                // Yes...Bring existing instance to top and activate it.
                NativeMethods.PostMessage(
                    (IntPtr)HWND_BROADCAST,
                    WM_MY_MSG,
                    new IntPtr(0xCDCD),
                    new IntPtr(0xEFEF));
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