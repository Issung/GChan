using GChan.Helpers;
using GChan.Properties;
using GChan.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CancellationToken = System.Threading.CancellationToken;

namespace GChan.Models.Trackers
{
    /// <summary>
    /// <see cref="IDownloadable{T}"/> implementation is for downloading the website HTML.<br/>
    /// For downloading images <see cref="ScrapeUploadedAssets"/> is used and results queued into a download manager.
    /// </summary>
    // TODO: Threads should save their last scrape time and use the If-Modified-Since header in requests https://github.com/4chan/4chan-API#:~:text=Use%20If%2DModified%2DSince%20when%20doing%20your%20requests.
    public abstract class Thread : Tracker, IProcessable, INotifyPropertyChanged
    {
        public const string NO_SUBJECT = "No Subject";

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Assets that no longer need to be put in processing. This should load the saved assets from the database.
        /// </summary>
        // TODO: Make readonly
        public AssetIdsCollection SeenAssets { get; set; } = [];

        /// <summary>
        /// Assetss that have successfully completed processing. This should be saved in the database.
        /// </summary>
        // TODO: Make readonly
        public AssetIdsCollection SavedAssets { get; set; } = [];

        public bool ShouldProcess => !Gone;

        public string Subject
        {
            get => subject ?? NO_SUBJECT;
            set
            {
                subject = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The identifier of the thread (AKA No. (number))
        /// </summary>
        public long Id { get; protected set; }

        public int? FileCount
        {
            get => fileCount;
            set
            {
                fileCount = value;

                if (!Program.mainForm.Disposing && !Program.mainForm.IsDisposed)
                {
                    Program.mainForm.Invoke(() => { NotifyPropertyChanged(nameof(FileCount)); });
                }
            }
        }

        public bool Gone { get; protected set; } = false;

        /// <summary>
        /// When this thread was last succesfully scraped, null if not yet scraped.<br/>
        /// <see cref="DateTimeOffset"/> because that's what HttpClient header property accepts.
        /// </summary>
        // TODO: Does this work on boards too? If so should this be on Tracker instead of Thread?
        public DateTimeOffset? LastScrape { get; protected set; } = null;

        private string? subject = null;
        private int? fileCount = null;

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
            logger.Trace($"NotifyPropertyChanged! propertyName: {propertyName}.");
#endif
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<ProcessResult> ProcessAsync(CancellationToken cancellationToken)
        {
            if (!ShouldProcess)
            {
                return new(this, removeFromQueue: true);
            }

            try
            {
                // Should we be able to return more IDownloadables in DownloadResult to be added to the queue?
                var results = await ScrapeThreadImpl(
                    Settings.Default.SaveHtml,
                    Settings.Default.SaveThumbnails,
                    cancellationToken
                );

                if (results == null)
                {
                    return new(this, removeFromQueue: false);
                }

                if (!string.IsNullOrWhiteSpace(results.ThreadHtml))
                {
                    Directory.CreateDirectory(SaveTo);
                    var path = Path.Combine(SaveTo, "Thread.html");
                    await Utils.WriteAllTextAsync(path, results.ThreadHtml, cancellationToken);
                }

                var assets = results.Uploads.Concat<IAsset>(results.Thumbnails).ToArray();
                var newAssets = assets.Where(a => !SeenAssets.Contains(a.Id)).ToArray();

                FileCount = results.Uploads.Length;
                LastScrape = DateTimeOffset.Now;
                SeenAssets.AddRange(newAssets);

                return new(this, removeFromQueue: false, newProcessables: newAssets);
            }
            catch (OperationCanceledException)
            {
                logger.Debug("Cancelling download for {thread}.", this);
                return new(this, removeFromQueue: true);
            }
            catch (StatusCodeException e) when (e.IsGone())
            {
                Gone = true;
                return new(this, removeFromQueue: true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new(this, removeFromQueue: false);
            }
        }

        /// <summary>
        /// Website specific implementation for scraping a thread, returning html, uploads and thumbnail assets.<br/>
        /// Can return null to indicate the thread has had no change.
        /// </summary>
        protected abstract Task<ThreadScrapeResults> ScrapeThreadImpl(
            bool saveHtml,
            bool saveThumbnails,
            CancellationToken cancellationToken
        );

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3;
                hash = hash * 13 + SiteName.GetHashCode();
                hash = hash * 13 + BoardCode.GetHashCode();
                hash = hash * 13 + Id.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"Thread {{ {SiteName}.{Id} }}";
        }

        public ValueTask DisposeAsync()
        {
            cancellationTokenSource.Dispose();
            return default;
        }
    }
}
