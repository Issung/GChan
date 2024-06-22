using GChan.Data;
using GChan.Properties;
using GChan.Trackers;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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

        public static readonly char[] IllegalSubjectCharacters = Path.GetInvalidFileNameChars();
        public static readonly char[] IllegalFilenameCharacters = Path.GetInvalidFileNameChars();

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

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
        /// Build a WebClient for one-time use, should be disposed of once done.
        /// </summary>
        public static WebClient CreateWebClient()
        {
            var client = new WebClient();
            client.Headers["User-Agent"] = Settings.Default.UserAgent;
            return client;
        }

        /// <summary>
        /// Create a new Tracker (Thread or Board).
        /// </summary>
        public static Tracker CreateNewTracker(LoadedData data)
        {
            // TODO: Should be making trackers based on the LoadedData (pass loadeddata to constructor).
            // Rather than making them and then loading them with more data.
            // This would help app-startup ui responsiveness as it would reduce the notify property changed spam greatly.
            var tracker = CreateNewTracker(data.Url);

            if (data is LoadedThreadData threadData && tracker is Thread thread)
            {
                thread.Subject = threadData.Subject;
                thread.SavedIds = threadData.SavedIds;
                thread.FileCount = threadData.SavedIds.Count;
            }
            else if (data is LoadedBoardData boardData && tracker is Board board)
            {
                board.GreatestThreadId = boardData.GreatestThreadId;
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

        /// <summary>
        /// Creates <paramref name="directory"/> if it doesn't already exist.
        /// </summary>
        /// <param name="directory">Directory path, exlcluding filename.</param>
        /// <returns>True if file was downloaded or already existed, false for error occured.</returns>
        public static bool DownloadFileIfDoesntExist(string url, string directory)
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
                    using var client = CreateWebClient();
                    client.DownloadFile(url, fullpath);
                }

                return true;
            }
            catch (Exception e)
            {
                // Should we throw?
                logger.Warn(e, "Exception occured downloading file.");
                return false;
            }
        }

        /// <summary>
        /// New DownloadToDir overload that takes a new ImageLink class, this is used to retain the image's uploaded filename.
        /// </summary>
        /// <param name="successCallback">Action to call when download completes successfully.</param>
        public static void DownloadToDir(
            ImageLink link, 
            string dir, 
            Action<ImageLink> successCallback
        )
        {
            if (!Directory.Exists(dir))
            { 
                Directory.CreateDirectory(dir);
            }

            string destFilepath = CombinePathAndFilename(dir, link.GenerateFilename((ImageFileNameFormat)Settings.Default.ImageFilenameFormat));

            try
            {
                using var webClient = CreateWebClient();
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

                successCallback(link);
            }
            catch (WebException ex)
            {
                logger.Error(ex, $"Error occured while downloading link {link.Url}.");
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
            string subject = Utils.SanitiseSubject(thread.Subject);

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

        /// <summary>
        /// Remove a string of characters illegal for a folder name.<br/>
        /// Used for thread subjects if addThreadSubjectToFolder setting is enabled.
        /// </summary>
        public static string SanitiseSubject(string subject)
        {
            return RemoveCharactersFromString(subject, IllegalSubjectCharacters);
        }

        public static string SanitiseFilename(string filename)
        {
            return RemoveCharactersFromString(filename, IllegalFilenameCharacters);
        }


        /// <summary>
        /// Remove <paramref name="chars"/> from <paramref name="input"/>.
        /// </summary>
        /// <remarks>
        /// Found to be the fastest method by: https://stackoverflow.com/a/48590650/8306962.
        /// </remarks>
        public static string RemoveCharactersFromString(string input, params char[] chars) 
        {
            return string.Join(string.Empty, input.Split(chars));
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