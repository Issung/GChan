using GChan.Helpers;
using GChan.Properties;
using GChan.Services;
using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Thread = GChan.Models.Trackers.Thread;

namespace GChan.Models
{
    /// <summary>
    /// Thumbnail of an upload.
    /// </summary>
    public class Thumbnail : IAsset, IEquatable<Thumbnail>
    {
        public AssetId Id { get; private set; }

        public CancellationToken CancellationToken => thread.CancellationToken;

        public bool ShouldProcess => thread.Scraping && !thread.Gone && Settings.Default.SaveThumbnails;

        /// <summary>
        /// URL to download the thumbnail.
        /// </summary>
        private readonly string url;
        private readonly Thread thread;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public Thumbnail(
            Thread thread,
            long replyId,
            string url
        )
        {
            this.thread = thread;
            this.url = url;

            Id = new AssetId(AssetType.Thumbnail, $"{thread.SiteName}.{thread.Id}.{replyId}");
        }

        public async Task<ProcessResult> ProcessAsync(CancellationToken cancellationToken)
        {
            if (!ShouldProcess)
            {
                return new(this, removeFromQueue: true);
            }

            var destinationDirectory = Path.Combine(thread.SaveTo, "thumb");
            Directory.CreateDirectory(destinationDirectory);
            var destinationPath = Path.Combine(destinationDirectory, Utils.GetFilenameFromUrl(url));

            try
            {
                var client = Utils.GetHttpClient();
                var fileBytes = await client.GetByteArrayAsync(url, cancellationToken);
                await Utils.WriteFileBytesAsync(destinationPath, fileBytes, cancellationToken);

                thread.SavedAssets.Add(Id);
            }
            catch (OperationCanceledException)
            {
                logger.Debug("Cancelling download for {thumbnail}.", this);
                return new(this, removeFromQueue: true);
            }
            catch (StatusCodeException e) when (e.IsGone())
            {
                logger.Debug("Downloading {thumbnail} resulted in {status_code}", this, e.StatusCode);
                return new(this, removeFromQueue: true);  // Thread is gone, don't retry.
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occured downloading an image.");
                return new(this, removeFromQueue: false);   // Unknown error, retry.
            }

            return new(this, removeFromQueue: true);
        }

        public bool Equals(Thumbnail other) => Id.Equals(other.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            return $"Thumbnail {{ {Id.Identifier} }}";
        }

        public ValueTask DisposeAsync() => default;
    }
}
