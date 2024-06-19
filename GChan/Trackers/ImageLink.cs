using GChan.Controllers;
using GChan.Helpers;
using GChan.Properties;
using GChan.Trackers;
using NLog;
using System;
using System.IO;
using System.Net;
using CancellationToken = System.Threading.CancellationToken;

namespace GChan
{
    public class ImageLink : IDownloadable<ImageLink>, IEquatable<ImageLink>
    {
        /// <summary>
        /// For 4chan: Unix timestamp with microseconds at which the image was uploaded.
        /// For 8kun: The ID of the post this image belongs to (same as No).
        /// </summary>
        public long Tim;

        /// <summary>
        /// URL to the access the image.
        /// </summary>
        public string Url;

        /// <summary>
        /// The <strong>sanitised</strong> filename the image was uploaded as <strong>without an extension</strong>.<br/>
        /// e.g. "LittleSaintJames", <strong>not</strong> the stored filename e.g. "1265123123.jpg".
        /// </summary>
        public string UploadedFilename;

        /// <summary>
        /// The ID of the post this image belongs to.
        /// </summary>
        public long No;

        /// <summary>
        /// The thread this image is from.
        /// </summary>
        public Thread Thread;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public bool ShouldDownload => !Thread.Gone;

        public ImageLink(
            long tim, 
            string url, 
            string uploadedFilename, 
            long no,
            Thread thread
        )
        {
            Tim = tim;
            Url = url;
            UploadedFilename = Utils.SanitiseFilename(uploadedFilename);
            No = no;
            Thread = thread;
        }

        public void Download(
            DownloadManager<ImageLink>.SuccessCallback successCallback,
            DownloadManager<ImageLink>.FailureCallback failureCallback,
            CancellationToken cancellationToken
        )
        {
            if (!ShouldDownload)
            {
                successCallback(this);
                return;
            }

            if (!Directory.Exists(Thread.SaveTo))
            {
                Directory.CreateDirectory(Thread.SaveTo);
            }

            var destFilepath = Utils.CombinePathAndFilename(Thread.SaveTo, GenerateFilename((ImageFileNameFormat)Settings.Default.ImageFilenameFormat));

            try
            {
                // TODO: Asyncify/Taskify.
                // TODO: Use cancellation token.
                using var webClient = Utils.CreateWebClient();
                webClient.DownloadFile(Url, destFilepath);
                Thread.SavedIds.Add(Tim);
                successCallback(this);
            }
            catch (WebException webException) when (webException.IsGone(out var httpWebResponse))
            {
                logger.Debug("Downloading {image_link} resulted in {status_code}", this, httpWebResponse.StatusCode);
                failureCallback(this, false);   // Don't retry.
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occured downloading an image.");
                failureCallback(this, true);
            }
        }

        public string GenerateFilename(ImageFileNameFormat format)
        {
            var extension = Path.GetExtension(Url); // Contains period (.).

            var result = format switch
            {
                ImageFileNameFormat.ID => $"{No}{extension}",
                ImageFileNameFormat.OriginalFilename => $"{UploadedFilename}{extension}",
                ImageFileNameFormat.IDAndOriginalFilename => $"{No} - {UploadedFilename}{extension}",
                ImageFileNameFormat.OriginalFilenameAndID => $"{UploadedFilename} - {No}{extension}",
                _ => throw new ArgumentException("Given value for 'format' is unknown.")
            };

            return result;
        }

        public bool Equals(ImageLink other)
        {
            if (other == null)
            {
                return false;
            }

            return Tim == other.Tim &&
                   Url == other.Url &&
                   UploadedFilename == other.UploadedFilename;
        }

        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            return this.Equals((ImageLink)other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Tim.GetHashCode();
                hash = hash * 23 + (Url?.GetHashCode() ?? 0);
                hash = hash * 23 + (UploadedFilename?.GetHashCode() ?? 0);
                hash = hash * 23 + No.GetHashCode();
                hash = hash * 23 + Thread.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"ImageLink {{ Tim: '{Tim}', Url: '{Url}', UploadedFilename: '{UploadedFilename}', No: '{No}' }}";
        }
    }
}
