using System.IO;
using System.Linq;

namespace GChan
{
    public class ImageLink
    {
        public string URL;
        public string Filename;

        public ImageLink(string url, string filename)
        {
            URL = url;
            Filename = filename;
        }

        public string GenerateNewFilename(ImageFileNameFormat format)
        {
            //const int FILENAME_MAX_LENGTH = 254;
            string[] parts = URL.Split('/');
            string lastPart = (parts.Length > 0) ? parts.Last() : URL;

            string extension = Path.GetExtension(URL); /// Contains fullstop dot (.).

            string result;

            switch (format)
            {
                case ImageFileNameFormat.ID:
                    result = lastPart;
                    break;
                case ImageFileNameFormat.OriginalFilename:
                    result = Filename + extension;
                    break;
                case ImageFileNameFormat.IDAndOriginalFilename:
                default:
                    result = Path.GetFileNameWithoutExtension(URL) + " - " + Filename + extension;
                    break;
            }

            //if (result.Length > FILENAME_MAX_LENGTH)
            //{
            //    result = result.Substring(0, FILENAME_MAX_LENGTH - extension.Length) + extension;
            //}

            //Program.Log($"Filename generated: {result}. Length: {result.Length}");
            return result;
        }
    }
}
