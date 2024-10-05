using NLog;
using System.Net;
using System.Threading;

namespace GChan.Trackers
{
    public enum Type { Board, Thread };

    public abstract class Tracker
    {
        /// <summary>
        /// Response status codes that indicate content is no longer available.
        /// </summary>
        public static readonly HttpStatusCode?[] GoneStatusCodes =
        {
            HttpStatusCode.NotFound,
            HttpStatusCode.Gone,
        };

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
        public bool Scraping => !CancellationToken.IsCancellationRequested;

        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        protected readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected readonly CancellationTokenSource cancellationTokenSource = new();

        protected Tracker(string url)
        {
            Url = url;
        }

        public override string ToString()
        {
            if (this is Thread thread)
            {
                return $"Thread {{ {SiteName}, /{BoardCode}/, {thread.Id}, Gone: {thread.Gone} }}";
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

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
