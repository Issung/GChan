using NLog;
using System;
using System.IO;
using System.Linq;

namespace GChan
{
    public class ImageLink
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

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The ID of the post this image belongs to.
        /// </summary>
        public long No;

        public ImageLink(long tim, string url, string uploadedFilename, long no)
        {
            Tim = tim;
            Url = url;
            UploadedFilename = Utils.SanitiseFilename(uploadedFilename);
            No = no;
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

            logger.Info($"Filename generated: {result}. Length: {result.Length}");

            return result;
        }

        public override string ToString()
        {
            return $"ImageLink {{ Tim: '{Tim}', Url: '{Url}', UploadedFilename: '{UploadedFilename}', No: '{No}' }}";
        }
    }
}
