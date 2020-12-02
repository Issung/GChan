using System.Linq;
using System.Text.RegularExpressions;

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
                Download();

                if (!Gone && Properties.Settings.Default.loadHTML)
                    DownloadHTMLPage();
            }
        }

        protected abstract void Download();

        protected abstract ImageLink[] GetImageLinks();

        protected abstract void DownloadHTMLPage();

        public string GetID()
        {
            //Split up the url by slashes and return the only string that is an integer.
            return URL.Split('\\', '/').Where(t => int.TryParse(t, out int whocares)).FirstOrDefault();
            //return URL.Substring(URL.LastIndexOf('/') + 1);
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
