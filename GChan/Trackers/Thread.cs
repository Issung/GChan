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

        protected long greatestSavedFileTim = 0;

        public event PropertyChangedEventHandler PropertyChanged;

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

            set {
                subject = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The ID of the thread (AKA No. (number))
        /// </summary>
        public string ID { get; protected set; }

        public int? FileCount { 
            get { return fileCount; }
            set { 
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
                URL = url.Substring(0, url.LastIndexOf('/'));
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if DEBUG
            Console.WriteLine($"NotifyPropertyChanged! propertyName: {propertyName}");
#endif

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        protected abstract string GetThreadSubject();

        public string GetURLWithSubject()
        {
            return (URL + ("/?subject=" + Utils.CleanSubjectString(Subject).Replace(' ', '_'))).Replace("\r", "");
        }
    }
}
