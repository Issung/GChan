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
        /// The filename the image was uploaded with. 
        /// e.g. "LittleSaintJames.jpg", NOT the stored filename e.g. "1265123123.jpg".
        /// </summary>
        public string UploadedFilename;

        /// <summary>
        /// The ID of the post this image belongs to.
        /// </summary>
        public long No;

        public ImageLink(long tim, string url, string uploadedFilename, long no)
        {
            Tim = tim;
            Url = url;
            UploadedFilename = uploadedFilename;
            No = no;
        }

        public string GenerateFilename(ImageFileNameFormat format)
        {
            //const int FILENAME_MAX_LENGTH = 254;
            string[] parts = Url.Split('/');
            string lastPart = (parts.Length > 0) ? parts.Last() : Url;

            string extension = Path.GetExtension(Url); // Contains period (.).

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
                    result = Path.GetFileNameWithoutExtension(Url) + " - " + UploadedFilename + extension;
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
                result = Path.GetFileNameWithoutExtension(Url) + " - " + UploadedFilename + extension;
            }
            else //ImageFileFormat == OriginalFilenameAndID
            {
                result = UploadedFilename + " - " + Path.GetFileNameWithoutExtension(Url) + extension;
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
            return $"ImageLink {{ Tim: '{Tim}', Url: '{Url}', UploadedFilename: '{UploadedFilename}', No: '{No}' }}";
        }
    }
}
