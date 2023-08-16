using GChan.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GChan.Trackers
{
    public class Thread_4Chan : Thread
    {
        public const string THREAD_REGEX = "boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/[0-9]*";
        public const string BOARD_CODE_REGEX = "(?<=((chan|channel).org/))[a-zA-Z0-9]+(?=(/))?";
        public const string ID_CODE_REGEX = "(?<=(thread/))[0-9]*(?=(.*))";

        public Thread_4Chan(string url) : base(url)
        {
            SiteName = Board_4Chan.SITE_NAME;

            Match match = Regex.Match(url, @"boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/\d*");
            Url = "http://" + match.Groups[0].Value;

            Match boardCodeMatch = Regex.Match(url, BOARD_CODE_REGEX);
            BoardCode = boardCodeMatch.Groups[0].Value;

            Match idCodeMatch = Regex.Match(url, ID_CODE_REGEX);
            ID = idCodeMatch.Groups[0].Value;

            SaveTo = Settings.Default.SavePath + "\\" + SiteName + "\\" + BoardCode + "\\" + ID;

            if (subject == null)
                Subject = GetThreadSubject();
        }

        public static bool UrlIsThread(string url)
        {
            return Regex.IsMatch(url, THREAD_REGEX);
        }

        protected override ImageLink[] GetImageLinks()
        {
            List<ImageLink> links = new List<ImageLink>();
            string JSONUrl = "http://a.4cdn.org/" + BoardCode + "/thread/" + ID + ".json";
            string baseURL = "http://i.4cdn.org/" + BoardCode + "/";

            try
            {
                JObject jObject;
                using (var web = new WebClient())
                {
                    string json = web.DownloadString(JSONUrl);
                    jObject = JObject.Parse(json);
                }

                // The /f/ board (flash) saves the files with their uploaded name.
                var timPath = BoardCode == "f" ? "filename" : "tim";
                links = jObject
                    .SelectTokens("posts[*]")
                    .Where(x => x["ext"] != null)
                    .Select(x =>
                        new ImageLink(x[timPath].Value<long>(),
                            baseURL + x[timPath] + x["ext"],
                            x["filename"].ToString(),
                            x["no"].Value<long>()))
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Encountered an exception in GetImageLinks() {this}.");
                throw;
            }

            FileCount = links.Count;
            return links.ToArray();
        }

        protected override void DownloadHTMLPage()
        {
            List<string> thumbs = new List<string>();
            string htmlPage = "";
            string baseURL = "//i.4cdn.org/" + BoardCode + "/";
            string JURL = "http://a.4cdn.org/" + BoardCode + "/thread/" + ID + ".json";

            try
            {
                JObject jObject;

                using (var web = new WebClient())
                {
                    //Add a UserAgent to prevent 403
                    web.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

                    htmlPage = web.DownloadString(Url);

                    //Prevent the html from being destroyed by the anti adblock script
                    htmlPage = htmlPage.Replace("f=\"to\"", "f=\"penis\"");
                    
                    string json = web.DownloadString(JURL);
                    jObject = JObject.Parse(json);
                }

                var posts = jObject
                    .SelectTokens("posts[*]")
                    .Where(x => x["ext"] != null)
                    .ToList();
                
                foreach (var post in posts)
                {
                    string old = baseURL + post["tim"] + post["ext"];
                    string replacement = post["tim"] + (string) post["ext"];
                    htmlPage = htmlPage.Replace(old, replacement);

                    //get the actual filename saved
                    string filename = Path.GetFileNameWithoutExtension(
                        new ImageLink(post["tim"].Value<long>(),
                                old,
                                post["filename"].ToString(),
                                post["no"].Value<long>())
                            .GenerateFilename((ImageFileNameFormat)Settings.Default.ImageFilenameFormat));

                    //Save thumbs for files that need it
                    if (replacement.Split('.')[1] == "webm")
                    {
                        old = "//t.4cdn.org/" + BoardCode + "/" + post["tim"] + "s.jpg";
                        thumbs.Add("http:" + old);

                        htmlPage = htmlPage.Replace(post["tim"].ToString(), filename);
                        htmlPage = htmlPage.Replace("//i.4cdn.org/" + BoardCode + "/" + filename, "thumb/" + post["tim"]);
                    }
                    else
                    {
                        string thumbName = replacement.Split('.')[0] + "s";
                        htmlPage = htmlPage.Replace(thumbName + ".jpg", replacement.Split('.')[0] + "." + replacement.Split('.')[1]);
                        htmlPage = htmlPage.Replace("/" + thumbName, thumbName);

                        htmlPage = htmlPage.Replace("//i.4cdn.org/" + BoardCode + "/" + post["tim"], post["tim"].ToString());
                        htmlPage = htmlPage.Replace(post["tim"].ToString(), filename); //easy fix for images
                    }

                    htmlPage = htmlPage.Replace("//is2.4chan.org/" + BoardCode + "/" + post["tim"], post["tim"].ToString()); //bandaid fix for is2 urls
                    htmlPage = htmlPage.Replace("/" + replacement, replacement);
                }

                htmlPage = htmlPage.Replace("=\"//", "=\"http://");

                //Save thumbs for files that need it
                for (int i = 0; i < thumbs.Count; i++)
                    Utils.DownloadFile(thumbs[i], SaveTo + "\\thumb");

                if (!string.IsNullOrWhiteSpace(htmlPage))
                    File.WriteAllText(SaveTo + "\\Thread.html", htmlPage);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override string GetThreadSubject()
        {
            string subject = NO_SUBJECT;

            try
            {
                string JSONUrl = "http://a.4cdn.org/" + BoardCode + "/thread/" + ID + ".json";

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
                                subject = Utils.SanitiseSubject(WebUtility.HtmlDecode(subject));
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
