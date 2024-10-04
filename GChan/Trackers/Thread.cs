using GChan.Helpers;
using GChan.Models;
using GChan.Properties;
using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CancellationToken = System.Threading.CancellationToken;

namespace GChan.Trackers
{
    /// <summary>
    /// <see cref="IDownloadable{T}"/> implementation is for downloading the website HTML.<br/>
    /// For downloading images <see cref="ScrapeUploadedAssets"/> is used and results queued into a download manager.
    /// </summary>
    public abstract class Thread : Tracker, IProcessable, INotifyPropertyChanged
    {
        public const string NO_SUBJECT = "No Subject";

        public event PropertyChangedEventHandler PropertyChanged;

        public SavedAssetIdsCollection SavedIds { get; set; } = new();

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
                return new(false);
            }

            try
            {
                // Should we be able to return more IDownloadables in DownloadResult to be added to the queue?
                await ScrapeThread(cancellationToken);
                await ScrapeUploadedAssets(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger.Debug("Cancelling download for {thread}.", this);
                return new(false);
            }
            catch (StatusCodeException e) when (e.IsGone())
            {
                Gone = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new(true);
            }

            return new(false);
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

        private async Task ScrapeThread(CancellationToken cancellationToken)
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
                    var thumbnails = results.Thumbnails;

                    // How can we filter out thumbnails that are already in the queue? Keep 2 collections? SeenIds & SavedIds?
                    // What do we do with the new ones?
                }
            }
        }

        private async Task ScrapeUploadedAssets(CancellationToken cancellationToken)
        {
            try
            {
                var uploads = await ScrapeUploadsImpl(cancellationToken);

                // How can we filter out uploads that are already in the queue? Keep 2 collections? SeenIds & SavedIds?
                // What do we do with them now?
            }
            catch (WebException webEx) when (webEx.IsGone(out var httpWebResponse))
            {
                Gone = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected abstract string GetThreadSubject();

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
    }
}
