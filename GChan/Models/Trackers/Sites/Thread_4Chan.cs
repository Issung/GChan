using GChan.Helpers;
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

        protected override async Task<ThreadScrapeResults> ScrapeThreadImpl(
            bool saveHtml,
            bool saveThumbnails,
            CancellationToken cancellationToken
        )
        {
            string? html = null;
            Thumbnail[] thumbnails = Array.Empty<Thumbnail>();

            // TODO: Implement If-Modified-Since header header sending & handling.
            var jObject = await GetThreadJson(cancellationToken);

            var uploads = ScrapeUploads(jObject);

            // TODO: If thread was not modified since, skip this step.
            if (saveHtml)
            {
                html = saveHtml ? await GetThreadHtml(cancellationToken) : null;

                if (saveThumbnails)
                {
                    thumbnails = ScrapeThumbnails(jObject);

                    // TODO: Alter html with thumbnails knowledge to fix paths (extra data will need to be saved in thumbnails).
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

        // TODO: Implement If-Modified-Since header header sending & handling.
        private async Task<JObject> GetThreadJson(CancellationToken cancellationToken)
        {
            var client = Utils.GetHttpClient();
            var jsonUrl = $"http://a.4cdn.org/{BoardCode}/thread/{Id}.json";
            var json = await client.GetStringAsync(jsonUrl, cancellationToken);
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

            return uploads;
        }

        // TODO: Web request here that is non-async and not rate limited (via IProcessable pipeline).
        private string GetThreadSubject()
        {
            string subject = NO_SUBJECT;

            try
            {
                string JSONUrl = "http://a.4cdn.org/" + BoardCode + "/thread/" + Id + ".json";

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
