using NLog;
using System;
using System.IO;
using System.Linq;

namespace GChan
{
    public class ImageLink
    {
        public long Tim;

        /// <summary>
        /// URL to the access the image.
        /// </summary>
        public string URL;

        /// <summary>
        /// The filename the image was uploaded with. 
        /// e.g. "LittleSaintJames.jpg", NOT the stored filename e.g. "1265123123.jpg".
        /// </summary>
        public string UploadedFilename;

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public ImageLink(string url, string uploadedFilename)
        {
            URL = url;
            UploadedFilename = uploadedFilename;
        }

        public ImageLink(long tim, string url, string uploadedFilename)
        {
            Tim = tim;
            URL = url;
            UploadedFilename = uploadedFilename;
        }

        public string GenerateNewFilename(ImageFileNameFormat format)
        {
            //const int FILENAME_MAX_LENGTH = 254;
            string[] parts = URL.Split('/');
            string lastPart = (parts.Length > 0) ? parts.Last() : URL;

            string extension = Path.GetExtension(URL); // Contains period (.).

            string result = format switch
            {
                ImageFileNameFormat.ID => lastPart,
                ImageFileNameFormat.OriginalFilename => UploadedFilename + extension,
                ImageFileNameFormat.IDAndOriginalFilename => $"{Tim}-{UploadedFilename}{extension}",
                ImageFileNameFormat.OriginalFilenameAndID => $"{UploadedFilename}-{Tim}{extension}",
                _ => throw new ArgumentException("Given value for 'format' is unknown.")
            };

            logger.Info($"Filename generated: {result}. Length: {result.Length}");

            return result;
        }

        public override string ToString()
        {
            return $"ImageLink {{ Tim: '{Tim}', URL: '{URL}', UploadedFilename: '{UploadedFilename}' }}";
        }
    }
}
