using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GChan.Trackers
{
    public class Thread_8Kun : Thread
    {
        public const string threadRegex = @"8kun.top/[a-zA-Z0-9]*?/res/[0-9]*.[^0-9]*";
        public const string boardCodeRegex = @"(?<=(8kun.top/)).*(?=(/res/))";
        public const string idCodeRegex = @"(?<=(res/)).*(?=(.html))";

        public Thread_8Kun(string url) : base(url)
        {
            SiteName = "8kun";

            //Match simplifiedThreadUrlMatch = Regex.Match(url, threadRegex);
            //URL = "https://" + simplifiedThreadUrlMatch.Groups[0].Value;      // simplify thread url

            Match boardCodeMatch = Regex.Match(url, boardCodeRegex);
            BoardCode = boardCodeMatch.Groups[0].Value;

            Match idCodeMatch = Regex.Match(url, idCodeRegex);
            ID = idCodeMatch.Groups[0].Value;

            SaveTo = Path.Combine(Properties.Settings.Default.SavePath, SiteName, BoardCode, ID);

            if (subject == null)
                subject = GetThreadSubject();
        }

        public static bool UrlIsThread(string url)
        {
            return Regex.IsMatch(url, threadRegex);
        }

        protected override void Download()
        {
            try
            {
                if (!Directory.Exists(SaveTo))
                    Directory.CreateDirectory(SaveTo);

                ImageLink[] imageLinks = GetImageLinks();

                Parallel.ForEach(imageLinks, (link) =>
                {
                    if (link.Tim > GreatestSavedFileTim)
                    {
#if DEBUG
                        Program.Log(true, $"Downloading file {link} because it's Tim was greater than {GreatestSavedFileTim}");
#endif
                        Utils.DownloadToDir(link, SaveTo);
                    }
                    else
                    {
#if DEBUG
                        Program.Log(true, $"Skipping downloading file {link} because it's Tim was less than than {GreatestSavedFileTim}");
#endif
                    }
                });

#if DEBUG
                long max = imageLinks.Max(t => t.Tim);
                Program.Log(true, $"Setting thread {this} {nameof(GreatestSavedFileTim)} to {max}.");
                GreatestSavedFileTim = max;
#else
                GreatestSavedFileTim = imageLinks.Max(t => t.Tim);
#endif
            }
            catch (WebException webEx)
            {
                if (((int)webEx.Status) == 7)
                    Gone = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "No Permission to access folder");
                throw;
            }
        }

        protected override ImageLink[] GetImageLinks()
        {
            List<ImageLink> links = new List<ImageLink>();
            string JSONUrl = ("http://8kun.top/" + BoardCode + "/res/" + ID + ".json"); // Thread JSON url
            string str = "";
            XmlNodeList xmlNos, xmlTims, xmlFilenames, xmlExts, xmlFpaths;

            string Fpath0Url(string boardcode, string tim, string ext) => $"https://8kun.top/{boardcode}/src/{tim}{ext}";
            string Fpath1Url(string tim, string ext) => $"https://8kun.top/file_store/{tim}{ext}";

            try
            {
                string Content = new WebClient().DownloadString(JSONUrl);

                byte[] bytes = Encoding.ASCII.GetBytes(Content);
                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();                                                               // convert JSON to XML (funny, I know)
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);

                // Get the numbers of replies that have 1 or more images.
                xmlNos = doc.DocumentElement.SelectNodes("/root/posts/item[tim]/no");

                // Loop through each reply.
                for (int i = 0; i < xmlNos.Count; i++)
                {
                    // Get the tims, filenames and extensions of each image in this reply.
                    xmlTims = doc.DocumentElement.SelectNodes($"/root/posts/item[no={xmlNos[i].InnerText}]//tim");
                    xmlFilenames = doc.DocumentElement.SelectNodes($"/root/posts/item[no={xmlNos[i].InnerText}]//filename");
                    xmlExts = doc.DocumentElement.SelectNodes($"/root/posts/item[no={xmlNos[i].InnerText}]//ext");
                    xmlFpaths = doc.DocumentElement.SelectNodes($"/root/posts/item[no={xmlNos[i].InnerText}]//fpath");

                    for (int j = 0; j < xmlTims.Count; j++)
                    {
                        string url;

                        if (xmlExts[j].InnerText != "deleted")
                        {
                            if (xmlFpaths[j].InnerText == "0")
                                url = Fpath0Url(BoardCode, xmlTims[j].InnerText, xmlExts[j].InnerText);
                            else // "1"
                                url = Fpath1Url(xmlTims[j].InnerText, xmlExts[j].InnerText);

                            // Save image link using reply no (number) as tim because 8kun tims have letters and numbers in them. The reply number will work just fine.
                            links.Add(new ImageLink(long.Parse(xmlNos[i].InnerText), url, xmlFilenames[j].InnerText));
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError)   // 404
                    Gone = true;
                throw;
            }

            fileCount = links.Count;
            return links.ToArray();
        }

        protected override void DownloadHTMLPage()
        {
            List<string> thumbs = new List<string>();
            string htmlPage = "";
            string str;

            try
            {
                htmlPage = new WebClient().DownloadString(URL);

                string JURL = URL.Replace(".html", ".json");

                string Content = new WebClient().DownloadString(JURL);

                byte[] bytes = Encoding.ASCII.GetBytes(Content);
                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                // get single images
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                XmlNodeList xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");
                XmlNodeList xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");
                for (int i = 0; i < xmlExt.Count; i++)
                {
                    string ext = xmlExt[i].InnerText;
                    //                        if(ext == ".webm")
                    //                            ext = ".jpg";
                    thumbs.Add("https://8kun.top/file_store/thumb/" + xmlTim[i].InnerText + ext);

                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/thumb/" + xmlTim[i].InnerText + ext, "=\"thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/" + xmlTim[i].InnerText + ext, "=\"" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                }

                // get images of posts with multiple images
                xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/tim");
                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/ext");
                for (int i = 0; i < xmlExt.Count; i++)
                {
                    string ext = xmlExt[i].InnerText;
                    //                        if(ext == ".webm")
                    //                            ext = ".jpg";
                    thumbs.Add("https://8kun.top/file_store/thumb/" + xmlTim[i].InnerText + ext);

                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/thumb/" + xmlTim[i].InnerText + ext, "=\"thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/" + xmlTim[i].InnerText + ext, "=\"" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                }

                htmlPage = htmlPage.Replace("=\"/", "=\"https://8kun.top/");

                for (int i = 0; i < thumbs.Count; i++)
                {
                    Utils.DownloadToDir(thumbs[i], SaveTo + "\\thumb");
                }

                if (!String.IsNullOrWhiteSpace(htmlPage))
                    File.WriteAllText(SaveTo + "\\Thread.html", htmlPage); // save thread
            }
            catch
            {
                throw;
            }
        }

        protected override string GetThreadSubject()
        {
            string subject = NO_SUBJECT;

            try
            {
                string JSONUrl = "http://8kun.top/" + BoardCode + "/res/" + ID + ".json";

                const string SUB_HEADER = "\"sub\":\"";
                const string SUB_ENDER = "\",";
                const string ITEM_ENDER = "},";

                using (var web = new WebClient())
                {
                    string rawjson = web.DownloadString(JSONUrl);
                    int subStartIndex = rawjson.IndexOf(SUB_HEADER);
                    int firstItemEnderIndex = rawjson.IndexOf(ITEM_ENDER);

                    // If "sub":" was found in json then there is a subject.
                    if (subStartIndex >= 0 && subStartIndex < firstItemEnderIndex)
                    {
                        //Increment along the rawjson until the ending ", sequence is found, then substring it to extract the subject.
                        for (int i = subStartIndex; i < rawjson.Length; i++)
                        {
                            if (rawjson.Substring(i, SUB_ENDER.Length) == SUB_ENDER)
                            {
                                subject = rawjson.Substring(subStartIndex + SUB_HEADER.Length, i - (subStartIndex + SUB_HEADER.Length));
                                subject = Utils.CleanSubjectString(WebUtility.HtmlDecode(subject));
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                subject = NO_SUBJECT;
            }

            return subject;
        }
    }
}
