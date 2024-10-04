﻿using GChan.Helpers;
using GChan.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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

            var match = Regex.Match(url, @"boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/\d*");
            Url = "http://" + match.Groups[0].Value;

            var boardCodeMatch = Regex.Match(url, BOARD_CODE_REGEX);
            BoardCode = boardCodeMatch.Groups[0].Value;

            var idCodeMatch = Regex.Match(url, ID_CODE_REGEX);
            ID = idCodeMatch.Groups[0].Value;

            SaveTo = Path.Combine(Settings.Default.SavePath, SiteName, BoardCode, ID);

            if (subject == null)
            {
                Subject = GetThreadSubject();
            }
        }

        public static bool UrlIsThread(string url)
        {
            return Regex.IsMatch(url, THREAD_REGEX);
        }

        protected override async Task<Upload[]> ScrapeUploadsImpl(CancellationToken cancellationToken)
        {
            var baseUrl = $"http://i.4cdn.org/{BoardCode}/";
            var jsonUrl = $"http://a.4cdn.org/{BoardCode}/thread/{ID}.json";

            var client = Utils.GetHttpClient();
            var json = await client.GetStringAsync(jsonUrl, cancellationToken);
            var jObject = JObject.Parse(json);

            // The /f/ board (flash) saves the files with their uploaded name.
            var timPath = BoardCode == "f" ? "filename" : "tim";

            var uploads = jObject
                .SelectTokens("posts[*]")
                .Where(x => x["ext"] != null)
                .Select(x =>
                    new Upload(
                        x[timPath].Value<long>(),
                        baseUrl + Uri.EscapeDataString(x[timPath].Value<string>()) + x["ext"],  // Require escaping for the flash files stored with arbitrary string names.
                        x["filename"].Value<string>(),
                        x["no"].Value<long>(),
                        this
                    )
                )
                .ToArray();

            FileCount = uploads.Length;

            return uploads;
        }

        // TODO: Separate thread scrape & thumbnail scrape to save on string traversal & manipulation. If thread scraping is on get the html. If thumb scraping is on, pass the thread html to the get thumbnails method, etc.
        protected override async Task<ThreadScrapeResults> ScrapeThreadImpl(CancellationToken cancellationToken)
        {
            var thumbUrls = new List<string>();
            var baseUrl = $"//i.4cdn.org/{BoardCode}/";
            var jsonUrl = $"http://a.4cdn.org/{BoardCode}/thread/{ID}.json";

            var client = Utils.GetHttpClient();
            var htmlPage = await client.GetStringAsync(Url, cancellationToken);
            htmlPage = htmlPage.Replace("f=\"to\"", "f=\"penis\"");

            var json = await client.GetStringAsync(jsonUrl, cancellationToken);

            var posts = JObject
                .Parse(json)
                .SelectTokens("posts[*]")
                .Where(x => x["ext"] != null)
                .ToList();

            foreach (var post in posts)
            {
                var tim = post["tim"].ToString();
                var ext = post["ext"].ToString();
                var oldUrl = baseUrl + tim + ext;
                var newFilename = Path.GetFileNameWithoutExtension(
                    new Upload(
                        post["tim"].Value<long>(),
                        oldUrl,
                        post["filename"].ToString(),
                        post["no"].Value<long>(),
                        this
                    ).GenerateFilename((ImageFileNameFormat)Settings.Default.ImageFilenameFormat)
                );

                htmlPage = htmlPage.Replace(oldUrl, tim + ext);

                if (ext == ".webm")
                {
                    var thumbUrl = $"//t.4cdn.org/{BoardCode}/{tim}s.jpg";
                    thumbUrls.Add($"http:{thumbUrl}");

                    htmlPage = htmlPage.Replace(tim, newFilename);
                    htmlPage = htmlPage.Replace($"{baseUrl}{newFilename}", $"thumb/{tim}");
                }
                else
                {
                    var thumbName = tim + "s";
                    htmlPage = htmlPage.Replace($"{thumbName}.jpg", tim + ext);
                    htmlPage = htmlPage.Replace($"/{thumbName}", thumbName);

                    htmlPage = htmlPage.Replace($"{baseUrl}{tim}", tim);
                    htmlPage = htmlPage.Replace(tim, newFilename);
                }

                htmlPage = htmlPage.Replace($"//is2.4chan.org/{BoardCode}/{tim}", tim);
                htmlPage = htmlPage.Replace($"/{tim}{ext}", tim + ext);
            }

            // 4chan uses double slash urls (copy current protocol), when the user views it locally the protocol will no longer be http, so build it in.
            // This is used for javascript references.
            htmlPage = htmlPage.Replace("=\"//", "=\"http://");

            // Alter all content links like "http://is2.4chan.org/tv/123.jpg" to become local like "123.jpg".
            htmlPage = htmlPage.Replace($"http://is2.4chan.org/{BoardCode}/", string.Empty);

            var thumbAssets = thumbUrls.Select(thumbUrl => new Thumbnail(this, thumbUrl));
            return new(htmlPage, thumbAssets);
        }

        private string GetThreadSubject()
        {
            string subject = NO_SUBJECT;

            try
            {
                string JSONUrl = "http://a.4cdn.org/" + BoardCode + "/thread/" + ID + ".json";

                const string SUB_HEADER = "\"sub\":\"";
                const string SUB_ENDER = "\",";

                using (var web = Utils.CreateWebClient())
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
