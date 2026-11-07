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

        public bool ShouldProcess => Settings.Default.SaveThumbnails && thread.ShouldProcess;

        public DateTimeOffset? ReadyToProcessAt => null;

        public ProcessPriority Priority => ProcessPriority.Low;

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

            Id = new AssetId(AssetType.Thumbnail, $"{thread.Site}.{thread.Id}.{replyId}");
        }

        public async Task<ProcessResult> ProcessAsync(ProcessableParams parameters, CancellationToken cancellationToken)
        {
            if (!ShouldProcess)
            {
                return new(removeFromQueue: true);
            }

            var destinationDirectory = Path.Combine(thread.SaveTo, "thumb");
            Directory.CreateDirectory(destinationDirectory);
            var destinationPath = Path.Combine(destinationDirectory, Utils.GetFilenameFromUrl(url));

            var client = Utils.GetHttpClient();
            var fileBytes = await client.GetByteArrayAsync(url, cancellationToken);
            await Utils.WriteFileBytesAsync(destinationPath, fileBytes, cancellationToken);

            thread.SavedAssetIds.Add(Id);

            return new(removeFromQueue: true);
        }

        public bool Equals(Thumbnail other) => other != null && Id.Equals(other.Id);

        public override bool Equals(object obj) => Equals(obj as Thumbnail);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            return $"Thumbnail {{ {Id.Identifier} }}";
        }

        public ValueTask DisposeAsync() => default;
    }
}
