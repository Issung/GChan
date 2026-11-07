using GChan.Data.Models;
using NLog;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Models.Trackers
{
    public enum Type { Board, Thread };

    public abstract class Tracker : IAsyncDisposable
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

        public Site Site { get; protected set; }

        public string SiteDisplayName => Site.ToString().TrimStart('_');

        /// <summary>
        /// Code for the board this is tracking, excluding slashes.
        /// e.g. gif, r9k, b
        /// </summary>
        public string BoardCode { get; protected set; }

        /// <summary>
        /// The server's last-modified time of the content from the last successful scrape, null if not yet scraped.<br/>
        /// Sent as <c>If-Modified-Since</c>, so it must be the server's time, not ours: a local time taken after the request
        /// would hide anything posted while the request was in flight, or within any clock skew.
        /// </summary>
        public DateTimeOffset? LastModified { get; protected set; }

        /// <summary>
        /// Local time the last scrape was started (successful or not), null if not yet attempted this session.<br/>
        /// Used to space scrapes out by <see cref="Settings.MinSecondsBetweenScrapes"/>.
        /// </summary>
        public DateTimeOffset? LastAttempt { get; protected set; }

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
                return $"Thread {{ {Site}, /{BoardCode}/, {thread.Id} }}";
            }
            else if (this is Board)
            {
                return $"Board {{ {Site}, /{BoardCode}/ }}";
            }
            else
            {
                return $"{GetType().Name} {{ {Site}, /{BoardCode}/ }}";
            }
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        public ValueTask DisposeAsync()
        {
            cancellationTokenSource.Dispose();
            return default;
        }
    }
}
