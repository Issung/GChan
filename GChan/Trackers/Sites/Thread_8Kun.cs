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

        public Thread_8Kun(string url) : base(url)
        {
            SiteName = "8kun";

            Match match = Regex.Match(url, threadRegex);
            URL = "https://" + match.Groups[0].Value;      // simplify thread url
            SaveTo = (Properties.Settings.Default.path + "\\" + SiteName + "\\" + url.Split('/')[3] + "\\" + url.Split('/')[5]).Replace(".html", ""); // set saveto path
        }

        public static bool UrlIsThread(string url)
        {
            Regex urlMatcher = new Regex(threadRegex);
            return urlMatcher.IsMatch(url);
        }

        protected override void Download()
        {
            try
            {
                if (!Directory.Exists(SaveTo))
                    Directory.CreateDirectory(SaveTo);

                if (Properties.Settings.Default.loadHTML)
                    DownloadHTMLPage();

                ImageLink[] URLs = GetImageLinks();

                for (int y = 0; y < URLs.Length; y++)
                    Utils.DownloadToDir(URLs[y], SaveTo);
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
            string JSONUrl = ("http://8kun.top/" + URL.Split('/')[3] + "/res/" + URL.Split('/')[5] + ".json").Replace(".html", ""); // thread JSON url
            string str = "";
            XmlNodeList xmlTim, xmlFilenames, xmlExt;

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

                // get single images
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");
                xmlFilenames = doc.DocumentElement.SelectNodes("/root/posts/item/filename");
                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");

                for (int i = 0; i < xmlExt.Count; i++)
                {
                    //exed = exed + "https://8kun.top/" + getURL().Split('/')[3] + "/src/" + xmlTim[i].InnerText + xmlExt[i].InnerText + "\n";
                    links.Add(new ImageLink("https://8kun.top/" + "/file_store/" + xmlTim[i].InnerText + xmlExt[i].InnerText, xmlFilenames[i].InnerText));
                }

                // get images of posts with multiple images
                xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/tim");
                xmlFilenames = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/filename");
                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/ext");

                for (int i = 0; i < xmlExt.Count; i++)
                {
                    //exed = exed + "https://8kun.top/" + getURL().Split('/')[3] + "/src/" + xmlTim[i].InnerText + xmlExt[i].InnerText + "\n";
                    links.Add(new ImageLink("https://8kun.top/" + "/file_store/" + xmlTim[i].InnerText + xmlExt[i].InnerText, xmlFilenames[i].InnerText));
                }
            }
            catch (WebException webEx)
            {
                if (((int)webEx.Status) == 7)   // 404
                    Gone = true;
                throw;
            }

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
            throw new NotImplementedException();
        }
    }
}
