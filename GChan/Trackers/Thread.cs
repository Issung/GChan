using GChan.Helpers;
using GChan.Models;
using GChan.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CancellationToken = System.Threading.CancellationToken;

namespace GChan.Trackers
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
        public AssetIdsCollection SeenAssets { get; set; } = new();

        /// <summary>
        /// Assetss that have successfully completed processing. This should be saved in the database.
        /// </summary>
        // TODO: Make readonly
        public AssetIdsCollection SavedAssets { get; set; } = new();

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
        public string ID { get; protected set; }

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

        protected string subject { get; private set; } = null;
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
            logger.Debug($"NotifyPropertyChanged! propertyName: {propertyName}.");
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
                var thumbs = await ScrapeThread(cancellationToken);
                var uploads = await ScrapeUploadedAssets(cancellationToken);

                var assets = Enumerable.Concat<IAsset>(thumbs, uploads);
                var newAssets = assets.Where(a => !SeenAssets.Contains(a.Id));

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
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new(this, removeFromQueue: false);
            }

            return new(this, removeFromQueue: true);
        }

        /// <summary>
        /// Website specific implementation for scraping a thread, returning html and thumbnail assets.<br/>
        /// <see cref="Thread"/> base class will only call this if needed, and will only save what is needed.
        /// </summary>
        protected abstract Task<ThreadScrapeResults> ScrapeThreadImpl(CancellationToken cancellationToken);

        /// <summary>
        /// Website specific implementation to obtain a collection of assets specific image link retreival.
        /// </summary>
        protected abstract Task<Upload[]> ScrapeUploadsImpl(CancellationToken cancellationToken);

        private async Task<IEnumerable<Thumbnail>> ScrapeThread(CancellationToken cancellationToken)
        {
            if (Settings.Default.SaveHtml)
            {
                var results = await ScrapeThreadImpl(cancellationToken);

                if (!string.IsNullOrWhiteSpace(results.ThreadHtml))
                {
                    await Utils.WriteAllTextAsync($"{SaveTo}\\Thread.html", results.ThreadHtml, cancellationToken);
                }

                if (Settings.Default.SaveThumbnails)
                {
                    return results.Thumbnails;
                }
            }

            return Enumerable.Empty<Thumbnail>();
        }

        private async Task<IEnumerable<Upload>> ScrapeUploadedAssets(CancellationToken cancellationToken)
        {
            var uploads = await ScrapeUploadsImpl(cancellationToken);
            return uploads;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3;
                hash = hash * 13 + SiteName.GetHashCode();
                hash = hash * 13 + BoardCode.GetHashCode();
                hash = hash * 13 + ID.GetHashCode();
                return hash;
            }
        }

        public ValueTask DisposeAsync()
        {
            cancellationTokenSource.Dispose();
            return default;
        }
    }
}
