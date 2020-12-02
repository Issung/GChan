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
            SaveTo = Properties.Settings.Default.SavePath + "\\" + SiteName + "\\" + url.Split('/')[3] + "\\" + url.Split('/')[5];
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
                MessageBox.Show(uaex.Message, $"No Permission to access folder {SaveTo}.");
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
            XmlNodeList xmlTims;
            XmlNodeList xmlFilenames;
            XmlNodeList xmlExts;

            try
            {
                using (var web = new WebClient())
                {
                    string Content = web.DownloadString(JSONUrl);
                    byte[] bytes = Encoding.ASCII.GetBytes(Content);

                    using (var stream = new MemoryStream(bytes))
                    {
                        var quotas = new XmlDictionaryReaderQuotas();
                        var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                        var xml = XDocument.Load(jsonReader);
                        str = xml.ToString();
                    }
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);

                // The /f/ board (flash) saves the files with their uploaded name.
                if (URL.Split('/')[3] == "f")
                    xmlTims = doc.DocumentElement.SelectNodes("/root/posts/item/filename");
                else
                    xmlTims = doc.DocumentElement.SelectNodes("/root/posts/item/tim");

                xmlFilenames = doc.DocumentElement.SelectNodes("/root/posts/item/filename");
                xmlExts = doc.DocumentElement.SelectNodes("/root/posts/item/ext");

                for (int i = 0; i < xmlExts.Count; i++)
                {
                    links.Add(new ImageLink(long.Parse(xmlTims[i].InnerText), baseURL + xmlTims[i].InnerText + xmlExts[i].InnerText, xmlFilenames[i].InnerText));
                }
            }
            catch (Exception)
            {
                Program.Log(true, $"Encountered an exception in Thread_4Chan.GetImageLinks(). Thread Board/ID/Subject: {BoardCode}{ID}/{Subject}");
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
                using (var web = new WebClient())
                {
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
                    string filename = Path.GetFileNameWithoutExtension(new ImageLink(baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText, xmlFilenames[i].InnerText).GenerateNewFilename((ImageFileNameFormat)Properties.Settings.Default.ImageFilenameFormat));

                    //Save thumbs for files that need it
                    if (rep.Split('.')[1] == "webm")
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

                    htmlPage = htmlPage.Replace("//is2.4chan.org/" + URL.Split('/')[3] + "/" + xmlTim[i].InnerText, xmlTim[i].InnerText); //bandaid fix for is2 urls
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
            string subject = NO_SUBJECT;

            try
            {
                string JSONUrl = "http://a.4cdn.org/" + URL.Split('/')[3] + "/thread/" + URL.Split('/')[5] + ".json";

                const string SUB_HEADER = "\"sub\":\"";
                const string SUB_ENDER = "\",";

                using (var web = new WebClient())
                {
                    string rawjson = web.DownloadString(JSONUrl);
                    int subStartIndex = rawjson.IndexOf(SUB_HEADER);

                    // If "Sub":" was found in json then there is a subject.
                    if (subStartIndex >= 0)
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
