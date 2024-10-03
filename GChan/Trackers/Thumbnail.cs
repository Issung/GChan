using GChan.Helpers;
using GChan.Properties;
using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Trackers
{
    /// <summary>
    /// Thumbnail of an upload.
    /// </summary>
    public class Thumbnail : IAsset<Thumbnail>, IEquatable<Thumbnail>
    {
        public AssetId Id { get; private set; }

        /// <summary>
        /// URL to the access the image.
        /// </summary>
        public string Url;

        /// <summary>
        /// The thread this image is from.
        /// </summary>
        public Thread Thread;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public CancellationToken CancellationToken => Thread.CancellationToken;

        public bool ShouldDownload => Thread.Scraping && !Thread.Gone && Settings.Default.SaveThumbnails;

        public Thumbnail(
            Thread thread,
            string url
        )
        {
            Id = new AssetId(AssetType.Thumbnail, url);
            Thread = thread;
        }

        public async Task<DownloadResult> DownloadAsync(CancellationToken cancellationToken)
        {
            if (!ShouldDownload)
            {
                return new(false);
            }

            if (!Directory.Exists(Thread.SaveTo))
            {
                Directory.CreateDirectory(Thread.SaveTo);
            }

            var destinationPath = Path.Combine(Thread.SaveTo, "thumb", Utils.GetFilenameFromUrl(Url));

            try
            {
                var client = Utils.GetHttpClient();
                var fileBytes = await client.GetByteArrayAsync(Url, cancellationToken);
                await Utils.WriteFileBytesAsync(destinationPath, fileBytes, cancellationToken);

                Thread.SavedIds.Add(Id);
            }
            catch (OperationCanceledException)
            {
                logger.Debug("Cancelling download for {thumbnail}.", this);
                return new(false);
            }
            catch (StatusCodeException e) when (e.IsGone())
            {
                logger.Debug("Downloading {thumbnail} resulted in {status_code}", this, e.StatusCode);
                return new(false);  // Thread is gone, don't retry.
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occured downloading an image.");
                return new(true);   // Unknown error, retry.
            }

            return new(false);
        }

        public bool Equals(Thumbnail other) => Id.Equals(other.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            return $"Thumbnail {{ Url: '{Url}' }}";
        }
    }
}
