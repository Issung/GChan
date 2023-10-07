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
using GChan.Helpers;

namespace GChan.Trackers
{
    public class Thread_8Kun : Thread
    {
        public const string threadRegex = @"8kun.top/[a-zA-Z0-9]*?/res/[0-9]*.[^0-9]*";
        public const string boardCodeRegex = @"(?<=(8kun.top/))[a-zA-Z0-9]+(?=(/res/))";
        public const string ID_CODE_REGEX = @"(?<=(res/))[0-9]+(?=(.html))";

        public Thread_8Kun(string url) : base(url)
        {
            SiteName = Board_8Kun.SITE_NAME;

            //Match simplifiedThreadUrlMatch = Regex.Match(url, threadRegex);
            //URL = "https://" + simplifiedThreadUrlMatch.Groups[0].Value;      // simplify thread url

            Match boardCodeMatch = Regex.Match(url, boardCodeRegex);
            BoardCode = boardCodeMatch.Groups[0].Value;

            Match idCodeMatch = Regex.Match(url, ID_CODE_REGEX);
            ID = idCodeMatch.Groups[0].Value;

            SaveTo = Path.Combine(Settings.Default.SavePath, SiteName, BoardCode, ID);

            if (subject == null)
                Subject = GetThreadSubject();
        }

        public static bool UrlIsThread(string url)
        {
            return Regex.IsMatch(url, threadRegex);
        }

        protected override ImageLink[] GetImageLinksImpl(bool includeAlreadySaved = false)
        {
            string Fpath0Url(string boardcode, string tim, string ext) => $"https://8kun.top/{boardcode}/src/{tim}{ext}";
            string Fpath1Url(string tim, string ext) => $"https://8kun.top/file_store/{tim}{ext}";


            using var webClient = new WebClient();
            var jsonUrl = $"http://8kun.top/{BoardCode}/res/{ID}.json"; // Thread JSON url
            var json = webClient.DownloadString(jsonUrl);
            var jObject = JObject.Parse(json);
            var links = new List<ImageLink>();

            // Get the numbers of replies that have 1 or more images.
            var nos = jObject.SelectTokens("posts[?(@.tim)].no").Select(x => x.Value<long>()).ToList();

            // Loop through each reply.
            foreach (var no in nos)
            {
                // Get the tims, filenames and extensions of each image in this reply.
                var tims = jObject.SelectTokens($"posts[?(@.no == {no})]..tim").Select(x => x.ToString()).ToList();
                var filenames = jObject.SelectTokens($"posts[?(@.no == {no})]..filename").Select(x => x.ToString()).ToList();
                var exts = jObject.SelectTokens($"posts[?(@.no == {no})]..ext").Select(x => x.ToString()).ToList();
                var fpaths = jObject.SelectTokens($"posts[?(@.no == {no})]..fpath").Select(x => x.ToString()).ToList();

                for (int j = 0; j < tims.Count; j++)
                {
                    if (exts[j] != "deleted")
                    {
                        var url = fpaths[j] == "0" ? 
                            Fpath0Url(BoardCode, tims[j], exts[j]) :
                            Fpath1Url(tims[j], exts[j]); // "1"

                        // Save image link using reply no (number) as tim because 8kun tims have letters and numbers in them. The reply number will work just fine.
                        links.Add(new ImageLink(no, url, filenames[j], no, this));
                    }
                }
            }

            FileCount = links.Count;
            return links.MaybeRemoveAlreadySavedLinks(includeAlreadySaved, SavedIds).ToArray();
        }

        protected override void DownloadHTMLPage()
        {
            var thumbs = new List<string>();
            var htmlPage = "";

            try
            {
                JObject jObject;
                using (var web = new WebClient())
                {
                    htmlPage = new WebClient().DownloadString(Url);

                    string JURL = Url.Replace(".html", ".json");

                    string json = web.DownloadString(JURL);
                    jObject = JObject.Parse(json);
                }

                // get single images
                var posts = jObject
                    .SelectTokens("posts[*]")
                    .Where(x => x["ext"] != null)
                    .ToList();
                
                foreach (var post in posts)
                {
                    var tim = post["tim"].ToString();
                    var ext = post["ext"].ToString();
                    //                        if(ext == ".webm")
                    //                            ext = ".jpg";
                    thumbs.Add("https://8kun.top/file_store/thumb/" + post["tim"] + ext);

                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/thumb/" + tim + ext, "thumb/" + tim + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/thumb/" + tim + ext, "=\"thumb/" + tim + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/" + tim + ext, "=\"" + tim + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/thumb/" + tim + ext, "thumb/" + tim + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/" + tim + ext, tim + ext);
                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/" + tim + ext, tim + ext);
                }

                // get images of posts with multiple images
                var extras = jObject
                    .SelectTokens("posts.extra_files[*]")
                    .Where(x => x["ext"] != null)
                    .ToList();
                
                foreach (var extra in extras)
                {
                    var tim = extra["tim"].ToString();
                    var ext = extra["ext"].ToString();
                    //                        if(ext == ".webm")
                    //                            ext = ".jpg";
                    thumbs.Add("https://8kun.top/file_store/thumb/" + tim + ext);

                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/thumb/" + tim + ext, "thumb/" + tim + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/thumb/" + tim + ext, "=\"thumb/" + tim + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/" + tim + ext, "=\"" + tim + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/thumb/" + tim + ext, "thumb/" + tim + ext);
                    htmlPage = htmlPage.Replace("https://media.8kun.top/file_store/" + tim + ext, tim + ext);
                    htmlPage = htmlPage.Replace("https://8kun.top/file_store/" + tim + ext, tim + ext);
                }

                htmlPage = htmlPage.Replace("=\"/", "=\"https://8kun.top/");

                for (int i = 0; i < thumbs.Count; i++)
                {
                    Utils.DownloadFile(thumbs[i], SaveTo + "\\thumb");
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
