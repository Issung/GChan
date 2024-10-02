using GChan.Helpers;
using GChan.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CancellationToken = System.Threading.CancellationToken;

namespace GChan.Trackers
{
    /// <summary>
    /// <see cref="IDownloadable{T}"/> implementation is for downloading the website HTML.<br/>
    /// For downloading images <see cref="GetImageLinks"/> is used and results queued into a download manager.
    /// </summary>
    public abstract class Thread : Tracker, IDownloadable, INotifyPropertyChanged
    {
        public const string NO_SUBJECT = "No Subject";

        public event PropertyChangedEventHandler PropertyChanged;

        public SavedIdsCollection SavedIds { get; set; } = new();

        public bool ShouldDownload => !Gone;

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

        public async Task<DownloadResult> DownloadAsync(CancellationToken cancellationToken)
        {
            if (!ShouldDownload)
            {
                return new(false);
            }

            try
            {
                await DownloadHtmlImpl(cancellationToken);
                await GetImageLinks(cancellationToken);
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

        public abstract Task DownloadHtmlImpl(CancellationToken cancellationToken);

        /// <summary>
        /// Get imagelinks for this thread.
        /// </summary>
        private ImageLink[] GetImageLinks(CancellationToken cancellationToken)
        {
            if (Gone)
            {
                logger.Info($"Download(object callback) called on {this}, but will not download because {nameof(Gone)} is true.");
                return Array.Empty<ImageLink>();
            }

            try
            {
                if (!Directory.Exists(SaveTo))
                { 
                    Directory.CreateDirectory(SaveTo);
                }

                var imageLinks = GetImageLinksImpl();
                return imageLinks;
            }
            catch (WebException webEx) when (webEx.IsGone(out var httpWebResponse))
            {
                Gone = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return Array.Empty<ImageLink>();
        }

        /// <summary>
        /// Implementation point for website specific image link retreival.
        /// </summary>
        protected abstract Task<ImageLink[]> GetImageLinksImpl(bool includeAlreadySaved, CancellationToken cancellationToken);

        protected abstract string GetThreadSubject();

        public string GetURLWithSubject()
        {
            return (Url + ("/?subject=" + Utils.SanitiseSubject(Subject).Replace(' ', '_'))).Replace("\r", "");
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
    }
}
