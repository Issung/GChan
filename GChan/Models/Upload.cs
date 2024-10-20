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
    /// Original content uploaded by a user. Mostly images/webms, but can be flash files from /f/, or I've seen pdfs on /po/.
    /// </summary>
    public class Upload : IAsset, IEquatable<Upload>
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public AssetId Id { get; }

        /// <summary>
        /// For 4chan: Unix timestamp with microseconds at which the image was uploaded.
        /// </summary>
        public long Tim { get; }

        /// <summary>
        /// URL to the access the upload.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The <strong>sanitised</strong> filename the image was uploaded as <strong>without an extension</strong>.<br/>
        /// e.g. "LittleSaintJames", <strong>not</strong> the stored filename e.g. "1265123123.jpg".
        /// </summary>
        public string UploadedFilename { get; }

        /// <summary>
        /// The thread this upload is from.
        /// </summary>
        public Thread Thread { get; }

        /// <summary>
        /// The ID of the post (thread or reploy) this upload belongs to.
        /// </summary>
        public long No { get; }

        /// <summary>
        /// Generate save path filename with the current image filename format setting.
        /// </summary>
        public string Filename => GenerateFilename((ImageFileNameFormat)Settings.Default.ImageFilenameFormat);

        /// <summary>
        /// Contains leading period (e.g. ".png").
        /// </summary>
        public string Extension { get; }

        public CancellationToken CancellationToken => Thread.CancellationToken;

        public bool ShouldProcess => Thread.Scraping && !Thread.Gone;

        public Upload(
            long tim,
            string url,
            string uploadedFilename,
            long no,
            Thread thread
        )
        {
            Id = new AssetId(AssetType.Upload, $"{thread.Id}.{tim}");
            Tim = tim;
            Url = url;
            UploadedFilename = Utils.SanitiseFilename(uploadedFilename);
            No = no;
            Thread = thread;
            Extension = Path.GetExtension(Url);
        }

        public async Task<ProcessResult> ProcessAsync(CancellationToken cancellationToken)
        {
            if (!ShouldProcess)
            {
                return new(this, removeFromQueue: true);
            }

            Directory.CreateDirectory(Thread.SaveTo);
            var path = Utils.CombinePathAndFilename(Thread.SaveTo, Filename);

            try
            {
                var client = Utils.GetHttpClient();
                var fileBytes = await client.GetByteArrayAsync(Url, cancellationToken);
                await Utils.WriteFileBytesAsync(path, fileBytes, cancellationToken);

                Thread.SavedAssets.Add(Id);
            }
            catch (OperationCanceledException)
            {
                logger.Debug("Cancelling download for {image_link}.", this);
                return new(this, removeFromQueue: true);
            }
            catch (StatusCodeException e) when (e.IsGone())
            {
                logger.Debug("Downloading {image_link} resulted in {status_code}", this, e.StatusCode);
                return new(this, removeFromQueue: true);  // Thread is gone, don't retry.
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occured downloading an image.");
                return new(this, removeFromQueue: false);   // Unknown error, retry.
            }

            return new(this, removeFromQueue: true);
        }

        private string GenerateFilename(ImageFileNameFormat format)
        {
            var result = format switch
            {
                ImageFileNameFormat.ID => $"{No}{Extension}",
                ImageFileNameFormat.OriginalFilename => $"{UploadedFilename}{Extension}",
                ImageFileNameFormat.IDAndOriginalFilename => $"{No} - {UploadedFilename}{Extension}",
                ImageFileNameFormat.OriginalFilenameAndID => $"{UploadedFilename} - {No}{Extension}",
                _ => throw new ArgumentException("Given value for 'format' is unknown.")
            };

            return result;
        }

        public bool Equals(Upload other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            return $"Upload {{ Tim: '{Tim}', Url: '{Url}', UploadedFilename: '{UploadedFilename}', No: '{No}' }}";
        }

        public ValueTask DisposeAsync() => default;
    }
}
