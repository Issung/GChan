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

            string result;

            switch (format)
            {
                case ImageFileNameFormat.ID:
                    result = lastPart;
                    break;
                case ImageFileNameFormat.OriginalFilename:
                    result = UploadedFilename + extension;
                    break;
                case ImageFileNameFormat.IDAndOriginalFilename:
                default:
                    result = Path.GetFileNameWithoutExtension(URL) + " - " + UploadedFilename + extension;
                    break;
            }

            if (format == ImageFileNameFormat.ID)
            {
                result = lastPart;
            }
            else if (format == ImageFileNameFormat.OriginalFilename)
            {
                result = UploadedFilename + extension;
            }
            else if (format == ImageFileNameFormat.IDAndOriginalFilename)
            {
                result = Path.GetFileNameWithoutExtension(URL) + " - " + UploadedFilename + extension;
            }
            else //ImageFileFormat == OriginalFilenameAndID
            {
                result = UploadedFilename + " - " + Path.GetFileNameWithoutExtension(URL) + extension;
            }

            //if (result.Length > FILENAME_MAX_LENGTH)
            //{
            //    result = result.Substring(0, FILENAME_MAX_LENGTH - extension.Length) + extension;
            //}

            //Program.Log($"Filename generated: {result}. Length: {result.Length}");

            return result;
        }

        public override string ToString()
        {
            return $"ImageLink {{ Tim: '{Tim}', URL: '{URL}', UploadedFilename: '{UploadedFilename}' }}";
        }
    }
}
