using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GChan.Trackers
{
    public abstract class Thread : Tracker
    {
        public const string NO_SUBJECT = "No Subject";

        protected int fileCount;

        protected string subject = null;

        protected long greatestSavedFileTim = 0;

        public long GreatestSavedFileTim {
            get {
                return greatestSavedFileTim;
            }

            set {
                if (value > greatestSavedFileTim) {
                    greatestSavedFileTim = value;
                }
            }
        }

        public string Subject { 
            get {
                if (subject == null)
                    return NO_SUBJECT;
                else
                    return subject;
            }
        }

        /// <summary>
        /// The ID of the thread (AKA No. (number))
        /// </summary>
        public string ID { get; protected set; }
        public int FileCount { get { return fileCount; } }
        public bool Gone { get; protected set; } = false;

        protected Thread(string url) : base(url)
        {
            Type = Type.Thread;
            ID = GetID();

            if (url.Contains("?"))
            {
                //TODO: Do this with Regex or Uri and HTTPUtility HttpUtility.ParseQueryString (https://stackoverflow.com/a/659929/8306962)
                subject = url.Substring(url.LastIndexOf('=') + 1).Replace('_', ' ');
                URL = url.Substring(0, url.LastIndexOf('/'));
            }
        }

        public void Download(object callback)
        {
            if (Gone)
            {
                Program.Log(true, $"Download(object callback) called on thread {BoardCode}{ID}, but will not download because isGone is true");
            }
            else
            {
                DownloadImages();

                if (!Gone && Properties.Settings.Default.SaveHTML)
                    DownloadHTMLPage();
            }
        }

        private void DownloadImages()
        {
            try
            {
                if (!Directory.Exists(SaveTo))
                    Directory.CreateDirectory(SaveTo);

                ImageLink[] imageLinks = GetImageLinks();

                Parallel.ForEach(imageLinks, (link) =>
                {
                    if (Scraping)
                    {
                        if (link.Tim > GreatestSavedFileTim)
                        {
#if DEBUG
                            Program.Log(true, $"Downloading file {link} because it's Tim was greater than {GreatestSavedFileTim}");
#endif
                            Utils.DownloadToDir(link, SaveTo);
                        }
                        else
                        {
#if DEBUG
                            Program.Log(true, $"Skipping downloading file {link} because it's Tim was less than than {GreatestSavedFileTim}");
#endif
                        }
                    }
                });

                if (imageLinks.Length > 0)
                    GreatestSavedFileTim = imageLinks.Max(t => t.Tim);
            }
            catch (WebException webEx)
            {
                Program.Log(webEx);

                var httpWebResponse = (webEx.Response as HttpWebResponse);

                if (webEx.Status == WebExceptionStatus.ProtocolError || (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.NotFound))
                {
                    Program.Log(true, $"WebException encountered in FourChan.download(). Gone marked as true. {URL}");
                    Gone = true;
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                MessageBox.Show(uaex.Message, $"No Permission to access folder {SaveTo}.");
                Program.Log(uaex);
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        protected abstract ImageLink[] GetImageLinks();

        protected abstract void DownloadHTMLPage();

        //TODO: This probably needs to be overriden in specific website's Thread extensions, with Regex.
        public string GetID()
        {
            //Split up the url by slashes and return the only string that is an integer.
            return URL.Split('\\', '/').Where(t => int.TryParse(t, out int _)).FirstOrDefault();
        }

        protected abstract string GetThreadSubject();

        public void SetSubject(string newSubject)
        {
            subject = newSubject;
        }

        public string GetURLWithSubject()
        {
            return (URL + ("/?subject=" + Utils.CleanSubjectString(Subject).Replace(' ', '_'))).Replace("\r", "");
        }
    }
}
