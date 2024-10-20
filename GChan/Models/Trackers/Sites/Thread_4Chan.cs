﻿using GChan.Helpers;
using GChan.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Models.Trackers.Sites
{
    public class Thread_4Chan : Thread
    {
        public const string THREAD_REGEX = "boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/[0-9]*";
        public const string BOARD_CODE_REGEX = "(?<=((chan|channel).org/))[a-zA-Z0-9]+(?=(/))?";
        public const string ID_CODE_REGEX = "(?<=(thread/))[0-9]*(?=(.*))";

        public Thread_4Chan(string url, string subject = null, int? fileCount = null) : base(url)
        {
            SiteName = Board_4Chan.SITE_NAME;

            var match = Regex.Match(url, @"boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/\d*");
            Url = "http://" + match.Groups[0].Value;

            var boardCodeMatch = Regex.Match(url, BOARD_CODE_REGEX);
            BoardCode = boardCodeMatch.Groups[0].Value;

            var idCodeMatch = Regex.Match(url, ID_CODE_REGEX);
            Id = long.Parse(idCodeMatch.Groups[0].Value);

            SaveTo = Path.Combine(Settings.Default.SavePath, SiteName, BoardCode, Id.ToString());

            Subject = subject ?? GetThreadSubject();
            FileCount = fileCount;
        }

        public static bool UrlIsThread(string url)
        {
            return Regex.IsMatch(url, THREAD_REGEX);
        }

        protected override async Task<ThreadScrapeResults?> ScrapeThreadImpl(
            bool saveHtml,
            bool saveThumbnails,
            CancellationToken cancellationToken
        )
        {
            var html = (string?)null;
            var uploads = Array.Empty<Upload>();
            var thumbnails = Array.Empty<Thumbnail>();

            var jObject = await GetThreadJson(cancellationToken);

            if (jObject == null)
            {
                // Thread has not been modified since last scrape.
                return null;
            }    

            uploads = ScrapeUploads(jObject);

            if (saveHtml)
            {
                if (Settings.Default.Max1RequestPerSecond)  // Bit of hackery, as we're doing 2 network requests within 1 processable.
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }

                html = saveHtml ? await GetThreadHtml(cancellationToken) : null;

                html = FixThreadHtmlLinks(html, uploads);

                if (saveThumbnails)
                {
                    thumbnails = ScrapeThumbnails(jObject);
                }
            }

            return new(html, uploads, thumbnails);
        }

        private async Task<string> GetThreadHtml(CancellationToken cancellationToken)
        {
            var client = Utils.GetHttpClient();
            var htmlPage = await client.GetStringAsync(Url, cancellationToken);
            htmlPage = htmlPage.Replace("f=\"to\"", "f=\"penis\"");
            return htmlPage;
        }

        /// <returns>Returns null if the thread was not modified since the last scrape.</returns>
        private async Task<JObject?> GetThreadJson(CancellationToken cancellationToken)
        {
            var client = Utils.GetHttpClient();
            var jsonUrl = $"http://a.4cdn.org/{BoardCode}/thread/{Id}.json";
            var json = await client.GetStringAsync(jsonUrl, LastScrape, cancellationToken);

            if (json == null)
            {
                return null;
            }

            var jObject = JObject.Parse(json);
            return jObject;
        }

        private Thumbnail[] ScrapeThumbnails(JObject jObject)
        {
            var thumbs = jObject
                .SelectTokens("posts[*]")
                .Where(post => post["ext"] != null)
                .Select(post =>
                {
                    var no = post["no"].Value<long>();
                    var tim = post["tim"].Value<long>();
                    var ext = post["ext"].ToString();

                    // Only save thumbs for filetypes that need it.
                    if (ext == ".webm") // TODO: Figure out if flash files/pdfs need special handling like webms, and then figure out a better method to check for this condition.
                    {
                        var thumbUrl = $"http://t.4cdn.org/{BoardCode}/{tim}s.jpg";
                        //return (thumbUrl, no);
                        return new Thumbnail(this, no, thumbUrl);
                    }

                    return null;
                })
                .Where(t => t != null) // Filter nulls
                .ToArray();

            return thumbs;
        }

        private Upload[] ScrapeUploads(JObject jObject)
        {
            var baseUrl = $"http://i.4cdn.org/{BoardCode}/";
            var timPath = BoardCode == "f" ? "filename" : "tim";    // The /f/ board (flash) saves the files with their uploaded name.

            var uploads = jObject
                .SelectTokens("posts[*]")
                .Where(post => post["ext"] != null)
                .Select(post =>
                    new Upload(
                        post[timPath].Value<long>(),
                        baseUrl + Uri.EscapeDataString(post[timPath].Value<string>()) + post["ext"],  // Require escaping for the flash files stored with arbitrary string names.
                        post["filename"].Value<string>(),
                        post["no"].Value<long>(),
                        this
                    )
                )
                .ToArray();

            return uploads;
        }

        /// <summary>
        /// Return an altered version of <paramref name="html"/> that fixes js/css, thumbnail, and image/video links.
        /// </summary>
        // TODO: A lot of string manipulation going on here. StringBuilder may be better. Setup benchmark & compare overhead.
        private string FixThreadHtmlLinks(string html, Upload[] uploads)
        {
            var baseUrl = $"//i.4cdn.org/{BoardCode}/";

            foreach (var upload in uploads)
            {
                if (upload.Extension == ".webm") // TODO: Need a better way to check this, flash files and pdfs can also be present.
                {
                    html = html.Replace(upload.Tim.ToString() + upload.Extension, upload.Filename);
                    html = html.Replace($"{baseUrl}{upload.Filename}", $"thumb/{upload.Tim}");
                }
                else
                {
                    var thumbName = upload.Tim + "s";
                    html = html.Replace($"{thumbName}.jpg", upload.Tim + upload.Extension);
                    html = html.Replace($"/{thumbName}", thumbName);

                    html = html.Replace($"{baseUrl}{upload.Tim}", upload.Tim.ToString());
                    html = html.Replace(upload.Tim.ToString() + upload.Extension, upload.Filename);
                }

                html = html.Replace($"//is2.4chan.org/{BoardCode}/{upload.Tim}", upload.Tim.ToString());
                html = html.Replace($"/{upload.Tim}{upload.Extension}", upload.Tim + upload.Extension);
            }

            // 4chan uses urls starting with // (uses current protocol), when the user views it locally the protocol will be file:// which won't work, so we need to place http prefixes in.
            // This allows the locally viewed page to reference the js/css hosted on 4chan, fixing the styling and making it a bit functional.
            html = html.Replace("=\"//", "=\"http://");

            // Alter all content links like "http://is2.4chan.org/tv/123.jpg" to become local like "123.jpg".
            html = html.Replace($"http://is2.4chan.org/{BoardCode}/", string.Empty);

            return html;
        }

        // TODO: Web request here that is non-async and not rate limited (via IProcessable pipeline).
        private string GetThreadSubject()
        {
            string subject = NO_SUBJECT;

            try
            {
                string jsonUrl = "http://a.4cdn.org/" + BoardCode + "/thread/" + Id + ".json";

                const string SUB_HEADER = "\"sub\":\"";
                const string SUB_ENDER = "\",";

                using (var web = Utils.CreateWebClient())
                {
                    string rawjson = web.DownloadString(jsonUrl);
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
