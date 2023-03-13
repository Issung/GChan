using NLog;

namespace GChan.Trackers
{
    public enum Type { Board, Thread };

    public abstract class Tracker
    {
        public string Url { get; protected set; }

        public string SaveTo { get; protected set; }

        public Type Type { get; protected set; }

        public string SiteName { get; protected set; }

        /// <summary>
        /// Code for the board this is tracking, excluding slashes.
        /// e.g. gif, r9k, b
        /// </summary>
        public string BoardCode { get; protected set; }

        /// <summary>
        /// Whether or not to keep scraping this tracker.
        /// </summary>
        public bool Scraping { get; set; } = true;

        protected readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected Tracker(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            if (this is Thread thread)
            {
                return $"Thread {{ {SiteName}, /{BoardCode}/, {thread.ID}, Gone: {thread.Gone} }}";
            }
            else if (this is Board)
            {
                return $"Board {{ {SiteName}, /{BoardCode}/ }}";
            }
            else
            {
                return $"{this.GetType().Name} {{ {SiteName}, /{BoardCode}/ }}";
            }
        }
    }
}
