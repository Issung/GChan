using GChan.Models;
using GChan.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GChan.Trackers
{
    public abstract class Thread : Tracker, INotifyPropertyChanged
    {
        public const string NO_SUBJECT = "No Subject";

        private int? fileCount = null;

        protected string subject { get; private set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public SavedIdsCollection SavedIds { get; set; } = new();

        public string Subject
        {
            get
            {
                if (subject == null)
                    return NO_SUBJECT;
                else
                    return subject;
            }

            set
            {
                subject = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The identifier of the thread (AKA No. (number))
        /// </summary>
        public string ID { get; protected set; }

        private readonly HttpStatusCode[] goneStatusCodes =
        {
            HttpStatusCode.NotFound,
            HttpStatusCode.Gone,
        };

        public int? FileCount
        {
            get => fileCount;
            set
            {
                fileCount = value;
                Program.mainForm.Invoke(new Action(() => { NotifyPropertyChanged(nameof(FileCount)); }));
            }
        }

        public bool Gone { get; protected set; } = false;

        protected Thread(string url) : base(url)
        {
            Type = Type.Thread;

            if (url.Contains("?"))
            {
                //TODO: Do this with Regex or Uri and HTTPUtility HttpUtility.ParseQueryString (https://stackoverflow.com/a/659929/8306962)
                subject = url.Substring(url.LastIndexOf('=') + 1).Replace('_', ' ');
                Url = url.Substring(0, url.LastIndexOf('/'));
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if DEBUG
            logger.Debug($"NotifyPropertyChanged! propertyName: {propertyName}.");
#endif
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Download(object callback)
        {
            if (Gone)
            {
                logger.Info($"Download(object callback) called on {this}, but will not download because {nameof(Gone)} is true.");
            }
            else
            {
                DownloadImages();

                if (!Gone && Settings.Default.SaveHTML)
                {
                    DownloadHTMLPage();
                }
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
                        if (!SavedIds.Contains(link.Tim))
                        {
                            logger.Debug($"Downloading file {link} because its Tim is not in the saved set.");
                            Utils.DownloadToDir(link, SaveTo);
                            SavedIds.Add(link.Tim);
                        }
                        else
                        {
                            logger.Debug($"Skipping downloading file {link} because its Tim is in the saved set.");
                        }
                    }
                });
            }
            catch (WebException webEx)
            {
                var httpWebResponse = (HttpWebResponse)webEx.Response;
                var statusCode = httpWebResponse.StatusCode;

                if (webEx.Status == WebExceptionStatus.ProtocolError && goneStatusCodes.Contains(statusCode))
                {
                    logger.Info(webEx, $"404 occured in {this} {nameof(DownloadImages)}. 'Gone' set to true.");
                    Gone = true;
                }
                else
                {
                    logger.Error(webEx);
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                MessageBox.Show(uaex.Message, $"No Permission to access folder {SaveTo}.");
                logger.Error(uaex);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected abstract ImageLink[] GetImageLinks(bool includeAlreadySaved = false);

        protected abstract void DownloadHTMLPage();

        protected abstract string GetThreadSubject();

        public string GetURLWithSubject()
        {
            return (Url + ("/?subject=" + Utils.SanitiseSubject(Subject).Replace(' ', '_'))).Replace("\r", "");
        }
    }
}
