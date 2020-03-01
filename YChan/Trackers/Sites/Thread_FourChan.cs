using Newtonsoft.Json.Linq;
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
    public class Thread_4Chan : Thread
    {
        public const string threadRegex = "boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/[0-9]*";

        public Thread_4Chan(string url) : base(url)
        {
            SiteName = "4chan";

            Match match = Regex.Match(url, @"boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/\d*");
            URL = "http://" + match.Groups[0].Value;
            SaveTo = Properties.Settings.Default.path + "\\" + SiteName + "\\" + url.Split('/')[3] + "\\" + url.Split('/')[5];
            if (subject == null)
                subject = GetThreadSubject();
        }

        public static bool UrlIsThread(string url)
        {
            Regex urlMatcher = new Regex(threadRegex);
            return (urlMatcher.IsMatch(url));
        }

        protected override void Download()
        {
            try
            {
                if (!Directory.Exists(SaveTo))
                    Directory.CreateDirectory(SaveTo);

                ImageLink[] images = GetImageLinks();

                Parallel.ForEach(images, (link) =>
                {
                    Utils.DownloadToDir(link, SaveTo);
                });
            }
            catch (WebException webEx)
            {
                Program.Log(webEx);
                var httpWebResponse = (webEx.Response as HttpWebResponse);
                if (webEx.Status == WebExceptionStatus.ProtocolError || (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.NotFound))
                {
                    Program.Log(true, $"WebException encountered in FourChan.download(). Gone marked as true. {URL}");
                    Gone = true;
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                MessageBox.Show(uaex.Message, "No Permission to access folder");
                Program.Log(uaex);
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        protected override ImageLink[] GetImageLinks()
        {
            List<ImageLink> links = new List<ImageLink>();
            string JSONUrl = "http://a.4cdn.org/" + URL.Split('/')[3] + "/thread/" + URL.Split('/')[5] + ".json";
            string baseURL = "http://i.4cdn.org/" + URL.Split('/')[3] + "/";
            string str = "";
            XmlNodeList xmlTim;
            XmlNodeList xmlFilenames;
            XmlNodeList xmlExt;

            try
            {
                string Content = new WebClient().DownloadString(JSONUrl);
                byte[] bytes = Encoding.ASCII.GetBytes(Content);

                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);

                // The /f/ board (flash) saves the files with their uploaded name.
                if (URL.Split('/')[3] == "f")
                    xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/filename");
                else
                    xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");

                xmlFilenames = doc.DocumentElement.SelectNodes("/root/posts/item/filename");
                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");

                for (int i = 0; i < xmlExt.Count; i++)
                {
                    links.Add(new ImageLink(baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText, xmlFilenames[i].InnerText));
                }
            }
            catch (Exception ex)
            {
                Program.Log(true, $"Encountered an exception in FourChan.getLinks(). Thread Board/ID/Subject: {BoardCode}{ID}/{Subject}");
                throw;
            }

            fileCount = links.Count;
            return links.ToArray();
        }

        protected override void DownloadHTMLPage()
        {
            List<string> thumbs = new List<string>();
            string htmlPage = "";
            string str = "";
            string baseURL = "//i.4cdn.org/" + URL.Split('/')[3] + "/";
            string JURL = "http://a.4cdn.org/" + URL.Split('/')[3] + "/thread/" + URL.Split('/')[5] + ".json";

            try
            {
                //Add a UserAgent to prevent 403
                WebClient web = new WebClient();
                web.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

                htmlPage = web.DownloadString(URL);

                //Prevent the html from being destroyed by the anti adblock script
                htmlPage = htmlPage.Replace("f=\"to\"", "f=\"penis\"");

                string json = web.DownloadString(JURL);
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                XmlNodeList xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");
                XmlNodeList xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");
                XmlNodeList xmlFilenames = doc.DocumentElement.SelectNodes("/root/posts/item/filename");

                for (int i = 0; i < xmlExt.Count; i++)
                {
                    string old = baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText;
                    string rep = xmlTim[i].InnerText + xmlExt[i].InnerText;
                    htmlPage = htmlPage.Replace(old, rep);

                    //get the actual filename saved
                    string filename = Path.GetFileNameWithoutExtension(new ImageLink(baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText, xmlFilenames[i].InnerText).GenerateNewFilename((ImageFileNameFormat)Properties.Settings.Default.imageFilenameFormat));

                    //Save thumbs for files that need it
                    if (rep.Split('.')[1] == "webm" /*|| rep.Split('.')[1] == ""*/)
                    {
                        old = "//t.4cdn.org/" + URL.Split('/')[3] + "/" + xmlTim[i].InnerText + "s.jpg";
                        thumbs.Add("http:" + old);

                        htmlPage = htmlPage.Replace(xmlTim[i].InnerText, filename);
                        htmlPage = htmlPage.Replace("//i.4cdn.org/" + URL.Split('/')[3] + "/" + filename, "thumb/" + xmlTim[i].InnerText);
                    }
                    else
                    {
                        string thumbName = rep.Split('.')[0] + "s";
                        htmlPage = htmlPage.Replace(thumbName + ".jpg", rep.Split('.')[0] + "." + rep.Split('.')[1]);
                        htmlPage = htmlPage.Replace("/" + thumbName, thumbName);

                        htmlPage = htmlPage.Replace("//i.4cdn.org/" + URL.Split('/')[3] + "/" + xmlTim[i].InnerText, xmlTim[i].InnerText);
                        htmlPage = htmlPage.Replace(xmlTim[i].InnerText, filename); //easy fix for images
                    }

                    htmlPage = htmlPage.Replace("/" + rep, rep);
                }

                htmlPage = htmlPage.Replace("=\"//", "=\"http://");

                //Save thumbs for files that need it
                for (int i = 0; i < thumbs.Count; i++)
                    Utils.DownloadToDir(thumbs[i], SaveTo + "\\thumb");

                if (!string.IsNullOrWhiteSpace(htmlPage))
                    File.WriteAllText(SaveTo + "\\Thread.html", htmlPage);
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        protected override string GetThreadSubject()
        {
            string subject;

            try
            {
                string JSONUrl = "http://a.4cdn.org/" + URL.Split('/')[3] + "/thread/" + URL.Split('/')[5] + ".json";
                string Content = new WebClient().DownloadString(JSONUrl);

                dynamic data = JObject.Parse(Content);

                subject = data.posts[0].sub.ToString();
            }
            catch
            {
                subject = NO_SUBJECT;
            }

            return Utils.CleanSubjectString(subject);
        }
    }
}
