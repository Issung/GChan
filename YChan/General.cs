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
using System.Windows.Forms;
using System.Xml;

namespace YChan
{
    internal class General
    {
        public static string path = "";                                                   // dl Path
        public static int timer = 0;                                                      // Timer for frmMain.scnTimer
        public static bool loadHTML = false;                                              // HTML download activated?
        public static bool firstStart = false;                                            // First start?
        public static bool saveOnClose = false;                                           // saveonclose activated?
        public static bool minimizeToTray = true;                                         // Minimize to tray activated?
        public static bool warnOnClose = true;                                            // Display warning before closing main window

        /// <summary>
        /// Returns all saved threads or boards
        /// </summary>
        /// <param name="board">Set true to load boards, false to load threads</param>
        /// <returns></returns>
        public static string loadURLs(bool board)
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
        public static void writeURLs(List<Imageboard> Boards, List<Imageboard> Threads)
        {
            string Buffer = "";
            for (int i = 0; i < Boards.Count; i++)
                Buffer = Buffer + Boards[i].getURL() + "\n";
            File.WriteAllText(Application.CommonAppDataPath + "\\boards.dat", Buffer);

            Buffer = "";

            for (int i = 0; i < Threads.Count; i++)
                Buffer = Buffer + Threads[i].getURL() + "\n";
            File.WriteAllText(Application.CommonAppDataPath + "\\threads.dat", Buffer);
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
        /// <param name="HTML">Save HTML or not</param>
        /// <param name="Save">Save URLs or not</param>
        /// <param name="tray">Minimize to tray or not</param>
        /// <param name="closewarn">Warn before closing or not</param>
        public static void setSettings(string path, int time, bool HTML, bool Save, bool tray, bool closewarn)
        { // set settings
            General.path = path;
            General.timer = time;
            General.loadHTML = HTML;
            General.saveOnClose = Save;
            General.minimizeToTray = tray;
            General.warnOnClose = closewarn;
            General.writeSettings();
        }

        public static void writeSettings()
        {                                                    // save settings in ugly XML
            string lines = "<?xml version=\"1.0\"?>\r\n<data>\r\n<timer>" + General.timer + "</timer>\r\n<path>" + General.path + "</path>\r\n<loadhtml>";
            if (loadHTML)
                lines = lines + "1";
            else
                lines = lines + "0";
            lines = lines + "</loadhtml>\r\n<saveonclose>";
            if (saveOnClose)
                lines = lines + "1";
            else
                lines = lines + "0";
            lines = lines + "</saveonclose>\r\n<minimizetotray>";
            if (minimizeToTray)
                lines = lines + "1";
            else
                lines = lines + "0";
            lines = lines + "</minimizetotray>\r\n<warnonclose>";
            if (warnOnClose)
                lines = lines + "1";
            else
                lines = lines + "0";
            lines = lines + "</warnonclose>\r\n</data>\r\n";

            StreamWriter file = new StreamWriter(Application.CommonAppDataPath + "\\YCHAN2.xml");
            file.WriteLine(lines);
            file.Close();
        }

        public static void loadSettings()
        {                                                 // read settings from XML
            if (File.Exists(Application.CommonAppDataPath + "\\YCHAN2.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Application.CommonAppDataPath + "\\YCHAN2.xml");

                XmlNode NodTimer = doc.DocumentElement.SelectSingleNode("/data/timer");
                string Time = NodTimer.InnerText;

                XmlNode NodPath = doc.DocumentElement.SelectSingleNode("/data/path");
                string Path = NodPath.InnerText;

                XmlNode NodLoadhtml = doc.DocumentElement.SelectSingleNode("/data/loadhtml");
                bool html = (NodLoadhtml.InnerText == "1");

                XmlNode NodSaveOnClose = doc.DocumentElement.SelectSingleNode("/data/saveonclose");
                bool sav;
                if (NodSaveOnClose == null)
                    sav = false;
                else
                    sav = (NodSaveOnClose.InnerText == "1");

                XmlNode NodWarnOnClose = doc.DocumentElement.SelectSingleNode("/data/warnonclose");
                bool warn;
                if (NodWarnOnClose == null)
                    warn = true;
                else
                    warn = (NodWarnOnClose.InnerText == "1");

                XmlNode NodMinimizeToTray = doc.DocumentElement.SelectSingleNode("/data/minimizetotray");
                bool tray;
                if (NodMinimizeToTray == null)
                    tray = true;
                else
                    tray = (NodMinimizeToTray.InnerText == "1");

                int iTime = 10000;
                if (IsDigitsOnly(Time))
                    iTime = int.Parse(Time);

                General.setSettings(Path, iTime, html, sav, tray, warn);
            }
            else
            {
                firstStart = true;                                                                      // No settings file,
                General.setSettings("C:\\", 10000, false, false, true, true);                           // set default values
            }

            if (!File.Exists(Application.CommonAppDataPath + "\\2.4"))
            {                                    // Old settings file, new
                firstStart = true;                                                                      // version, first start,
                File.Create(Application.CommonAppDataPath + "\\2.4").Dispose();                           // create file to save version
            }
        }

        public static Imageboard createNewIMB(string url, bool board)
        {                                 // Create a new Imageboard
            if (!board)
            {
                if (Fchan.isThread(url))                                                                 // if FChan, create FChan
                    return new Fchan(url, board);
                else if (Infinitechan.isThread(url))
                    return new Infinitechan(url, board);
            }
            else
            {
                if (Fchan.isBoard(url))
                    return new Fchan(url, board);                                                       // if 8chan, create 8chan
                else if (Infinitechan.isBoard(url))
                    return new Infinitechan(url, board);
            }
            return null;
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

        public static bool dlTo(string url, string dir)
        {                                               // download to dir
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string FN = GetFileName(url);
            dir = dir + "\\" + FN;
            try
            {
                if (!File.Exists(dir))
                {
                    WebClient webClient = new WebClient();
                    //                   MessageBox.Show(url);
                    //                    MessageBox.Show(dir);
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