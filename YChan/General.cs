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

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GChan
{
    internal class General
    {
        /// <summary>
        /// Returns all saved threads or boards
        /// </summary>
        /// <param name="board">Set true to load boards, false to load threads</param>
        /// <returns></returns>
        public static string LoadURLs(bool board)
        {                                         // read saved URLS
            if (board && File.Exists(Application.CommonAppDataPath + "\\boards.dat"))
                return File.ReadAllText(Application.CommonAppDataPath + "\\boards.dat");
            else if (!board && File.Exists(Application.CommonAppDataPath + "\\threads.dat"))
                return File.ReadAllText(Application.CommonAppDataPath + "\\threads.dat");
            else
                return "";
        }

        /// <summary>
        /// Saves the thread and board list to disk
        /// </summary>
        /// <param name="Boards">List of boards</param>
        /// <param name="Threads">List of threads</param>
        public static void WriteURLs(List<Imageboard> Boards, List<Imageboard> Threads)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Boards.Count; i++)
                sb.AppendLine(Boards[i].getURL());
            File.WriteAllText(Application.CommonAppDataPath + "\\boards.dat", sb.ToString());

            sb = new StringBuilder();

            for (int i = 0; i < Threads.Count; i++)
                sb.AppendLine(Threads[i].getURL());
            File.WriteAllText(Application.CommonAppDataPath + "\\threads.dat", sb.ToString());
        }

        /// <summary>
        /// Validates if the string only contains digits
        /// </summary>
        /// <param name="str">String to validate</param>
        /// <returns></returns>
        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Save settings to disk
        /// </summary>
        /// <param name="path">Path to where the application downloads</param>
        /// <param name="time">Thrad refresh timer in seconds</param>
        /// <param name="loadHTML">Save HTML or not</param>
        /// <param name="saveOnclose">Save URLs or not</param>
        /// <param name="tray">Minimize to tray or not</param>
        /// <param name="closeWarn">Warn before closing or not</param>
        public static void SaveSettings(string path, int time, bool loadHTML, bool saveOnclose, bool tray, bool closeWarn)
        {
            Properties.Settings.Default.path = path;
            Properties.Settings.Default.timer = time;
            Properties.Settings.Default.loadHTML = loadHTML;
            Properties.Settings.Default.saveOnClose = saveOnclose;
            Properties.Settings.Default.minimizeToTray = tray;
            Properties.Settings.Default.warnOnClose = closeWarn;

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Create a new Imageboard
        /// </summary>
        public static Imageboard CreateNewImageboard(string url)
        {
            // if FChan, create FChan
            // if 8chan, create 8chan

            if (FourChan.urlIsThread(url))
                return new FourChan(url, false);
            else if (Infinitechan.urlIsThread(url))
                return new Infinitechan(url, false);

            if (FourChan.urlIsBoard(url))
                return new FourChan(url, true);
            else if (Infinitechan.urlIsBoard(url))
                return new Infinitechan(url, true);

            return null;
        }

        public static string PrepareURL(string url)
        {
            url = url.Trim();

            int indexOfHash = url.IndexOf('#');
            if (indexOfHash > 0)
                url = url.Substring(0, indexOfHash);

            return url;
        }

        private static string GetFileName(string hrefLink)
        {
            string[] parts = hrefLink.Split('/');
            string fileName = "";

            if (parts.Length > 0)
                fileName = parts[parts.Length - 1];
            else
                fileName = hrefLink;

            return fileName;
        }

        public static bool DownloadToDir(string url, string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fileName = GetFileName(url);
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
    }
}