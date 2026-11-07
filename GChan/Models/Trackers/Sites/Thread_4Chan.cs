using GChan.Data;
using GChan.Data.Models;
using GChan.Helpers.Extensions;
using GChan.Properties;
using GChan.Services;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace GChan.Models.Trackers.Sites
{
    public class Thread_4Chan : Thread
    {
        public const string THREAD_REGEX = "boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/[0-9]*";
        public const string BOARD_CODE_REGEX = "(?<=((chan|channel).org/))[a-zA-Z0-9]+(?=(/))?";
        public const string ID_CODE_REGEX = "(?<=(thread/))[0-9]*(?=(.*))";

        public Thread_4Chan(string url) : base(url, ProcessPriority.High)  // High priority for first scrape to fetch subject, filecount, etc for GUI display.
        {
            Site = Site._4chan;
            ConfigureFromUrl(url);
        }

        /// <summary>
        /// For use from boards scraping.
        /// </summary>
        /// <param name="subject">Null if the catalog has no usable subject, it is then extracted on the first scrape.</param>
        public Thread_4Chan(string url, string? subject, int fileCount) : base(url)
        {
            Site = Site._4chan;
            ConfigureFromUrl(url);
            Subject = subject;
            FileCount = fileCount;
        }

        /// <summary>
        /// For use when loading from database.
        /// </summary>
        public Thread_4Chan(ThreadData data) : base($"https://boards.4chan.org/{data.BoardCode}/thread/{data.Id}/")
        {
            this.Site = Site._4chan;
            this.BoardCode = data.BoardCode;
            this.Id = data.Id;
            this.Subject = data.Subject;
            this.FileCount = data.FileCount;
            this.SavedAssetIds.AddRange(data.SavedAssetIds);
            this.SeenAssetIds.AddRange(data.SavedAssetIds);

            // Only populate LastModified if it appears that all uploads have been saved.
            // This will cause the next dequeue of this thread to be downloaded regardless of when the last scrape was and
            // will remedy issue #2 mentioned by vlad-patras here: https://github.com/Issung/GChan/issues/49#issuecomment-2491144815 in which the
            // application is closed & reopened, a thread remains unchanged before removal but had files undownloaded.
            // TODO: This won't cover the case of some thumbnails not being saved.
            if (FileCount == SavedAssetIds.Count(a => a.Type == AssetType.Upload))
            {
                this.LastModified = data.LastScrape;
            }

            SaveTo = Path.Combine(Settings.Default.SavePath, Site.ToString().TrimStart('_'), BoardCode, Id.ToString());
        }

        public static bool UrlIsThread(string url)
        {
            return Regex.IsMatch(url, THREAD_REGEX);
        }

        protected override async Task<(ThreadScrapeResults? Results, DateTimeOffset LastModified)> ScrapeThreadImpl(
            bool saveHtml,
            bool saveThumbnails,
            CancellationToken cancellationToken
        )
        {
            var html = (string?)null;
            var uploads = Array.Empty<Upload>();
            var thumbnails = Array.Empty<Thumbnail>();

            var (jObject, lastModified) = await GetThreadJson(cancellationToken);

            if (jObject == null)
            {
                // Thread has not been modified since last scrape.
                return (null, lastModified);
            }

            if (subject == null)
            {
                var firstPost = jObject.SelectToken("posts[0]") ?? throw new Exception("A thread must have a first post.");
                Subject = ExtractSubject(firstPost);
            }
            uploads = ScrapeUploads(jObject);

            if (saveHtml)
            {
                if (Settings.Default.Max1RequestPerSecond)  // Bit of hackery, as we're doing 2 network requests within 1 processable.
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }

                html = await GetThreadHtml(cancellationToken);

                html = FixThreadHtmlLinks(html, uploads);

                if (saveThumbnails)
                {
                    thumbnails = ScrapeThumbnails(jObject);
                }
            }

            return (new(html, uploads, thumbnails), lastModified);
        }

        private async Task<string> GetThreadHtml(CancellationToken cancellationToken)
        {
            var client = Utils.GetHttpClient();
            var htmlPage = await client.GetStringAsync(Url, cancellationToken);
            htmlPage = htmlPage.Replace("f=\"to\"", "f=\"penis\"");
            return htmlPage;
        }

        /// <returns>The thread JSON (null if the thread was not modified since the last scrape) and the server's last-modified time.</returns>
        private async Task<(JObject? Json, DateTimeOffset LastModified)> GetThreadJson(CancellationToken cancellationToken)
        {
            var client = Utils.GetHttpClient();
            var jsonUrl = $"https://a.4cdn.org/{BoardCode}/thread/{Id}.json";
            var (json, lastModified) = await client.GetStringIfModifiedAsync(jsonUrl, LastModified, cancellationToken);

            if (json == null)
            {
                return (null, lastModified);
            }

            return (JObject.Parse(json), lastModified);
        }

        private Thumbnail[] ScrapeThumbnails(JObject jObject)
        {
            var thumbs = jObject
                .SelectTokens("posts[*]")
                .Where(post => post["ext"] != null)
                .Select(post =>
                {
                    var no = post["no"]!.Value<long>();
                    var tim = post["tim"]!.Value<long>();
                    var ext = post["ext"]!.ToString();

                    // Only save thumbs for filetypes that need it.
                    if (ext == ".webm") // TODO: Figure out if flash files/pdfs need special handling like webms, and then figure out a better method to check for this condition.
                    {
                        var thumbUrl = $"https://t.4cdn.org/{BoardCode}/{tim}s.jpg";
                        //return (thumbUrl, no);
                        return new Thumbnail(this, no, thumbUrl);
                    }

                    return null;
                })
                .OfType<Thumbnail>() // Filter nulls
                .ToArray();

            return thumbs;
        }

        private Upload[] ScrapeUploads(JObject jObject)
        {
            var baseUrl = $"https://i.4cdn.org/{BoardCode}/";
            var timPath = BoardCode == "f" ? "filename" : "tim";    // The /f/ board (flash) saves the files with their uploaded name.

            var uploads = jObject
                .SelectTokens("posts[*]")
                .Where(post => post["ext"] != null)
                .Select(post =>
                    new Upload(
                        post[timPath]!.Value<long>(),
                        baseUrl + Uri.EscapeDataString(post[timPath]!.Value<string>()!) + post["ext"],  // Require escaping for the flash files stored with arbitrary string names.
                        post["filename"]!.Value<string>(),
                        post["no"]!.Value<long>(),
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
            html = html.Replace("=\"//", "=\"https://");

            // Alter all content links like "https://is2.4chan.org/tv/123.jpg" to become local like "123.jpg".
            html = html.Replace($"https://is2.4chan.org/{BoardCode}/", string.Empty);

            return html;
        }

        private void ConfigureFromUrl(string url)
        {
            var match = Regex.Match(url, @"boards.(4chan|4channel).org/[a-zA-Z0-9]*?/thread/\d*");
            Url = "https://" + match.Groups[0].Value;

            var boardCodeMatch = Regex.Match(url, BOARD_CODE_REGEX);
            BoardCode = boardCodeMatch.Groups[0].Value;

            var idCodeMatch = Regex.Match(url, ID_CODE_REGEX);
            Id = long.Parse(idCodeMatch.Groups[0].Value);

            SaveTo = Path.Combine(Settings.Default.SavePath, Site.ToString().TrimStart('_'), BoardCode, Id.ToString());
        }

        /// <summary>
        /// Picks a subject from a thread's opening post (from the thread JSON or the board catalog, which share the same fields).
        /// </summary>
        /// <returns>The sanitised subject, or null if the post has nothing suitable.</returns>
        public static string? ExtractSubject(JToken post)
        {
            var sub = WebUtility.HtmlDecode(post["sub"]?.Value<string>());  // Subject is HTML encoded, so we must decode.

            var name = post["name"]?.Value<string>();
            var nameOk = name != "Anonymous";   // Use name if not "Anonymous". Users often mistakenly put the subject in the name field.

            var comment = WebUtility.HtmlDecode(post["com"]?.Value<string>()); // "com" (comment) is the text of the post. It is HTML encoded, so we must decode.
            var commentOk = comment != null && comment.Length < 64 && !comment.Contains("<br>");    // Use the comment as the subject if it is present, not too long and doesn't have any linebreaks.

            var result = sub ?? (nameOk ? name : null) ?? (commentOk ? comment : null);

            return result == null ? null : Utils.SanitiseSubject(result);
        }
    }
}
