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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using GChan.Trackers;

namespace GChan
{
    /// <summary>
    /// Utility methods
    /// </summary>
    internal class Utils
    {
        public const string PROGRAM_NAME = "GChan";
        public static string boardsFilepath => Application.CommonAppDataPath + "\\boards.dat";
        public static string threadsFilepath => Application.CommonAppDataPath + "\\threads.dat";

        /// <summary>
        /// Saves the thread and board list to disk
        /// </summary>
        public static void SaveURLs(List<Board> Boards, List<Thread> Threads)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < Boards.Count; i++)
                    sb.AppendLine(Boards[i].URL.Replace("\r", ""));

                File.WriteAllText(boardsFilepath, sb.ToString());

                sb.Clear();

                for (int i = 0; i < Threads.Count; i++)
                    sb.AppendLine(Threads[i].GetURLWithSubject());

                File.WriteAllText(threadsFilepath, sb.ToString());
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        /// <summary>
        /// Returns all saved threads or boards
        /// </summary>
        /// <param name="board">Set true to load boards, false to load threads</param>
        public static string LoadURLs(bool board)
        {
            if (board && File.Exists(boardsFilepath))
            {
                return File.ReadAllText(boardsFilepath);
            }
            else if (!board && File.Exists(threadsFilepath))
            {
                return File.ReadAllText(threadsFilepath);
            }
            else
            { 
                return "";
            }
        }

        /// <summary>
        /// Save settings to disk.
        /// </summary>
        /// <param name="path">Path to where the application downloads</param>
        /// <param name="time">Thrad refresh timer in seconds</param>
        /// <param name="loadHTML">Save HTML or not</param>
        /// <param name="saveOnclose">Save URLs or not</param>
        /// <param name="minimizeToTray">Minimize to tray or not</param>
        /// <param name="closeWarn">Warn before closing or not</param>
        public static void SaveSettings(
            string path,
            int time,
            ImageFileNameFormat imageFileNameFormat,
            ThreadFolderNameFormat threadFolderNameFormat,
            bool loadHTML,
            bool saveOnclose,
            bool minimizeToTray,
            bool closeWarn,
            bool startWithWindows,
            bool startWithWindowsMinimized,
            bool addThreadSubjectToFolder,
            bool addUrlFromClipboardWhenTextboxEmpty)
        {
            Properties.Settings.Default.path = path;
            Properties.Settings.Default.timer = time;
            Properties.Settings.Default.imageFilenameFormat = (byte)imageFileNameFormat;
            Properties.Settings.Default.threadFolderNameFormat = (byte)threadFolderNameFormat;
            Properties.Settings.Default.loadHTML = loadHTML;
            Properties.Settings.Default.saveOnClose = saveOnclose;
            Properties.Settings.Default.minimizeToTray = minimizeToTray;
            Properties.Settings.Default.warnOnClose = closeWarn;
            Properties.Settings.Default.startWithWindowsMinimized = startWithWindowsMinimized;
            Properties.Settings.Default.addThreadSubjectToFolder = addThreadSubjectToFolder;
            Properties.Settings.Default.addUrlFromClipboardWhenTextboxEmpty = addUrlFromClipboardWhenTextboxEmpty;

            Properties.Settings.Default.Save();

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startWithWindows)
                registryKey.SetValue(PROGRAM_NAME, '"' + Application.ExecutablePath + '"' + (minimizeToTray && startWithWindowsMinimized ? $" {Program.TRAY_CMDLINE_ARG}" : ""));
            else if (registryKey.GetValue(PROGRAM_NAME) != null)
                registryKey.DeleteValue(PROGRAM_NAME);
        }

        /// <summary>
        /// Validates if the string only contains digits
        /// </summary>
        /// <param name="str">String to validate</param>
        public static bool IsDigitsOnly(string str)
        {
            foreach(char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Create a new Imageboard
        /// </summary>
        public static Tracker CreateNewTracker(string url)
        {
            if (Thread_4Chan.UrlIsThread(url))
                return new Thread_4Chan(url);
            else if (Thread_8Kun.UrlIsThread(url))
                return new Thread_8Kun(url);

            if (Board_4Chan.UrlIsBoard(url))
                return new Board_4Chan(url);
            else if (Board_8Kun.UrlIsBoard(url))
                return new Board_8Kun(url);

            return null;
        }

        /// <summary>
        /// Remove the reply specifier from a thread url.
        /// eg "4chan.org/gif/thread/16245377#p16245377" becomes "4chan.org/gif/thread/16245377".
        /// (hashtag and any following characters are trimmed).
        /// </summary>
        public static string PrepareURL(string url)
        {
            url = url.Trim();

            int indexOfHash = url.IndexOf('#');
            if (indexOfHash > 0)
                url = url.Substring(0, indexOfHash);

            return url;
        }

        private static string GetFileNameFromURL(string hrefLink)
        {
            string[] parts = hrefLink.Split('/');
            string fileName = "";

            if (parts.Length > 0)
                fileName = parts.Last();
            else
                fileName = hrefLink;

            return fileName;
        }

        public static bool DownloadToDir(string url, string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fileName = GetFileNameFromURL(url);
            dir = dir + "\\" + fileName;

            try
            {
                if (!File.Exists(dir))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(url, dir);
                }
                return true;
            }
            catch (WebException WebE)
            {
                return false;
                throw WebE;
            }
        }

        /// <summary>
        /// New DownloadToDir overload that takes a new ImageLink class, this is used to retain the image's uploaded filename.
        /// </summary>
        public static bool DownloadToDir(ImageLink link, string dir)
        {
            //Program.Log(true, $"DownloadToDir called for link {link.URL} downloading to {dir}");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string destFilepath = CombinePathAndFilename(dir, link.GenerateNewFilename((ImageFileNameFormat)Properties.Settings.Default.imageFilenameFormat));

            int formatCount = Enum.GetValues(typeof(ImageFileNameFormat)).Length;
            List<ImageFileNameFormat> formatsNotInUse = new List<ImageFileNameFormat>();

            for (int i = 0; i < formatCount; i++)
            {
                if (i != Properties.Settings.Default.imageFilenameFormat)
                    formatsNotInUse.Add((ImageFileNameFormat)i);
            }

            try
            {
                // Incase imageFileNameFormat has changed and the file is already saved, instead of downloading another copy with a
                // new name, check if it already exists and if it does then rename it to the new imageFileNameFormat.
                for (int i = 0; i < formatsNotInUse.Count; i++)
                {
                    string potentialFilepath = CombinePathAndFilename(dir, link.GenerateNewFilename(formatsNotInUse[i]));
                    if (File.Exists(potentialFilepath))
                    {
                        File.Move(potentialFilepath, destFilepath);
                        return true;
                    }
                }

                //if (!File.Exists(destFilepath) || new System.IO.FileInfo(destFilepath).Length < 1)
                if( (File.Exists(destFilepath) && new System.IO.FileInfo(destFilepath).Length < 1) || !File.Exists(destFilepath))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(link.URL, destFilepath);

                    //TODO: Figure out how to make the async downloading work properly.
                    // This line is currently not working and downloads 0 byte files 100% of the time.
                    //webClient.DownloadFileAsync(new Uri(link.URL), destFilepath);
                    /*Task.Run(() => { 
                        webClient.DownloadFileAsync(new Uri(link.URL), destFilepath);
                        while (webClient.IsBusy)
                        { 
                            //Do nothing?
                        }
                    });*/

                    //new Thread(() => { webClient.DownloadFile(link.URL, destFilepath); }).Start();

                    return true;
                }

                return false;
            }
            catch (WebException WebE)
            {
                return false;
                throw WebE;
            }
        }

        /// <summary>
        /// Combine a path and filename with two backslashes \\ and clamp the final return out at a constant 255 length.
        /// </summary>
        public static string CombinePathAndFilename(string directory, string filename)
        {
            /// https://stackoverflow.com/a/265803/8306962
            /// This could be off by a few 1s. Information that floats around is conflicted, can't be bothered to research more.
            const int FILEPATH_MAX_LENGTH = 255;

            string fullpath = directory + "\\" + filename;
            //Program.Log($"destFilepath Generated: {destFilepath}. Length: {destFilepath.Length}");

            if (fullpath.Length >= FILEPATH_MAX_LENGTH)
            {
                //int oldLength = fullpath.Length;
                fullpath = fullpath.Substring(0, FILEPATH_MAX_LENGTH - Path.GetExtension(fullpath).Length) + Path.GetExtension(fullpath);
                //Program.Log(true, $"destFilepath max length exceeded new destfilepath: {fullpath}. Old Length: {oldLength}. New Length: {fullpath.Length}");
            }

            return fullpath;
        }

        public static string MessageBoxGetString(string currentSubject, int x = 0, int y = 0)
        {
            GetStringMessageBox dialog = new GetStringMessageBox(currentSubject);
            dialog.TopMost = true;

            if (x != 0 && y != 0)
            {
                dialog.Left = x;
                dialog.Top = y;
            }

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                return dialog.UserEntry.Replace("\n", "").Replace("\r", "");
            }
            else
            {
                return null;
            }
        }

        public static string RemoveCharactersFromString(string haystack, params char[] removeThese)
        {
            string ret = haystack;
            for (int i = 0; i < removeThese.Length; i++)
                ret = ret.Replace(removeThese[i].ToString(), "");
            return ret;
        }

        readonly static char[] subjectIllegalCharacters = { '"', '<', '>', '|', '\0', ':', '*', '?', '/', '\\',
            '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t', '\n', '\v', '\f', 
            '\r', '\u000e', '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', 
            '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d', '\u001e', '\u001f' };

        /// <summary>
        /// Remove a string of characters illegal for a folder name. Used for thread subjects if 
        /// addThreadSubjectToFolder setting is enabled.
        /// </summary>
        public static string CleanSubjectString(string subject)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < subject.Length; i++)
            {
                if (!subjectIllegalCharacters.Contains(subject[i]))
                {
                    sb.Append(subject[i]);
                }
            }

            return sb.ToString();
        }
    }
}