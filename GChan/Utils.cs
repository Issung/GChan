using GChan.Data;
using GChan.Properties;
using GChan.Trackers;
using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Thread = GChan.Trackers.Thread;

namespace GChan
{
    /// <summary>
    /// Utility methods
    /// </summary>
    internal class Utils
    {
        public const string PROGRAM_NAME = "GChan";

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

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
            bool addUrlFromClipboardWhenTextboxEmpty,
            bool checkForUpdatesOnStart)
        {
            Settings.Default.SavePath = path;
            Settings.Default.ScanTimer = time;
            Settings.Default.ImageFilenameFormat = (byte)imageFileNameFormat;
            Settings.Default.ThreadFolderNameFormat = (byte)threadFolderNameFormat;
            Settings.Default.SaveHTML = loadHTML;
            Settings.Default.SaveListsOnClose = saveOnclose;
            Settings.Default.MinimizeToTray = minimizeToTray;
            Settings.Default.WarnOnClose = closeWarn;
            Settings.Default.StartWithWindowsMinimized = startWithWindowsMinimized;
            Settings.Default.AddThreadSubjectToFolder = addThreadSubjectToFolder;
            Settings.Default.AddUrlFromClipboardWhenTextboxEmpty = addUrlFromClipboardWhenTextboxEmpty;
            Settings.Default.CheckForUpdatesOnStart = checkForUpdatesOnStart;

            Settings.Default.Save();

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
        /// Create a new Tracker (Thread or Board).
        /// </summary>
        public static Tracker CreateNewTracker(LoadedInfo info)
        {
            long greatestSaved = long.Parse(info.GreatestSaved);

            Tracker tracker = CreateNewTracker(info.URL);

            // TODO: Use 'is' check on type and cast at same time.
            if (tracker.Type == Trackers.Type.Thread)
            {
                ((Thread)tracker).GreatestSavedFileTim = greatestSaved;
                ((Thread)tracker).Subject = ((LoadedThreadInfo)info).Subject;
            }
            else if (tracker.Type == Trackers.Type.Board)
            {
                ((Board)tracker).LargestAddedThreadNo = greatestSaved;
            }

            return tracker;
        }

        /// <summary>
        /// Create a new Tracker (Thread or Board).
        /// </summary>
        public static Tracker CreateNewTracker(string url)
        {
            if (Thread_4Chan.UrlIsThread(url))
            {
                return new Thread_4Chan(url);
            }
            else if (Thread_8Kun.UrlIsThread(url))
            {
                return new Thread_8Kun(url);
            }

            if (Board_4Chan.UrlIsBoard(url))
            {
                return new Board_4Chan(url);
            }
            else if (Board_8Kun.UrlIsBoard(url))
            {
                return new Board_8Kun(url);
            }

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

        private static string GetFilenameFromUrl(string hrefLink)
        {
            var parts = hrefLink.Split('/');
            var fileName = parts.LastOrDefault() ?? hrefLink;
            return fileName;
        }

        /// <returns>True if file was downloaded or already existed, false for error occured.</returns>
        public static bool DownloadFile(string url, string directory)
        {
            if (!Directory.Exists(directory))
            { 
                Directory.CreateDirectory(directory);
            }

            var fileName = GetFilenameFromUrl(url);
            var fullpath = Path.Combine(directory, fileName);

            try
            {
                if (!File.Exists(fullpath))
                {
                    using var client = new WebClient();
                    client.DownloadFile(url, fullpath);
                }

                return true;
            }
            catch (Exception e)
            {
                // Should we throw?
                logger.Log(LogLevel.Warn, e, "Exception occured downloading file.");
                return false;
            }
        }

        /// <summary>
        /// New DownloadToDir overload that takes a new ImageLink class, this is used to retain the image's uploaded filename.
        /// </summary>
        public static bool DownloadToDir(ImageLink link, string dir)
        {
            if (!Directory.Exists(dir))
            { 
                Directory.CreateDirectory(dir);
            }

            string destFilepath = CombinePathAndFilename(dir, link.GenerateFilename((ImageFileNameFormat)Settings.Default.ImageFilenameFormat));

            try
            {
                using var webClient = new WebClient();
                webClient.DownloadFile(link.Url, destFilepath);

                // TODO: Figure out how to make the async downloading work properly.
                // This line is currently not working and downloads 0 byte files 100% of the time.
                //webClient.DownloadFileAsync(new Uri(link.URL), destFilepath);
                /*Task.Run(() => { 
                    webClient.DownloadFileAsync(new Uri(link.URL), destFilepath);
                    while (webClient.IsBusy)
                    { 
                        //Do nothing?
                    }
                });*/

                return true;
            }
            catch (WebException ex)
            {
                logger.Error(ex, $"Error occured while downloading link {link.Url}.");
                return false;
            }
        }

        /// <summary>
        /// Move thread directory for removal, based on settings.
        /// </summary>
        /// <exception cref="IOException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="PathTooLongException"/>
        public static void MoveThread(Thread thread)
        {
            string currentDirectory = thread.SaveTo.Replace("\r", "");
            string subject = SanitiseSubjectString(thread.Subject);

            // There are \r characters appearing from the custom subjects, TODO: need to get to the bottom of the cause of this.
            var folderNameFormat = (ThreadFolderNameFormat)Settings.Default.ThreadFolderNameFormat;
            var destinationDirectory = folderNameFormat switch
            {
                ThreadFolderNameFormat.IdSubject => $"{currentDirectory} - {subject}",
                ThreadFolderNameFormat.SubjectId => Path.Combine(Path.GetDirectoryName(currentDirectory), $"{subject} - {thread.ID}"),
                _ => throw new Exception($"Unknown value {folderNameFormat} for thread folder name format."),
            };

            destinationDirectory = destinationDirectory.Replace("\r", "").Trim('\\', '/');

            if (!Directory.Exists(currentDirectory))
            {
                logger.Info($"While attempting to move thread {thread} the current directory could not be found, abandoning.");
                return;
            }

            // Attmept normal directory move, if already exists add a number in bracket starting with 1 going upwards.
            int number = 0;
            string indexBracket() => (number == 0) ? "" : $" ({number})";
            string newDirectory() => destinationDirectory + indexBracket();

            while (Directory.Exists(newDirectory()))
            {
                number++;
            }

            logger.Info($"Moving thread directory '{currentDirectory}' to '{newDirectory()}'.");

            Directory.Move(currentDirectory, newDirectory());
        }

        /// <summary>
        /// Combine a path and filename with two backslashes \\ and clamp the final return out at a constant 255 length.
        /// </summary>
        public static string CombinePathAndFilename(string directory, string filename)
        {
            /// https://stackoverflow.com/a/265803/8306962
            /// This could be off by a few 1s. Information that floats around is conflicted, can't be bothered to research more.
            const int FILEPATH_MAX_LENGTH = 255;

            string fullpath = Path.Combine(directory, filename);

            if (fullpath.Length >= FILEPATH_MAX_LENGTH)
            {
                fullpath = fullpath.Substring(0, FILEPATH_MAX_LENGTH - Path.GetExtension(fullpath).Length) + Path.GetExtension(fullpath);
                //Program.Log(true, $"destFilepath max length exceeded new destfilepath: {fullpath}. Old Length: {oldLength}. New Length: {fullpath.Length}");
            }

            return fullpath;
        }

        public static string RemoveCharactersFromString(string haystack, params char[] charactersToRemove)
        {
            string ret = haystack;

            for (int i = 0; i < charactersToRemove.Length; i++)
            { 
                ret = ret.Replace(charactersToRemove[i].ToString(), "");
            }

            return ret;
        }

        readonly static char[] SubjectIllegalCharacters = Path.GetInvalidFileNameChars();

        /// <summary>
        /// Remove a string of characters illegal for a folder name. Used for thread subjects if 
        /// addThreadSubjectToFolder setting is enabled.
        /// </summary>
        public static string SanitiseSubjectString(string subject)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < subject.Length; i++)
            {
                if (!SubjectIllegalCharacters.Contains(subject[i]))
                {
                    sb.Append(subject[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Method to bypass 63 character limit of NotifyIcon.Text
        /// Credit: https://stackoverflow.com/a/580264/8306962
        /// </summary>
        public static void SetNotifyIconText(NotifyIcon ni, string text)
        {
            if (text.Length >= 128)
                throw new ArgumentOutOfRangeException("Text limited to 127 characters");
            System.Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
        }
    }
}