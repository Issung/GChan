using System;

#nullable enable

namespace GChan.Models.Trackers
{
    public class ThreadScrapeResults
    {
        /// <summary>
        /// Will only be saved if not null.
        /// </summary>
        public string? ThreadHtml { get; }

        public Upload[] Uploads { get; }

        public Thumbnail[] Thumbnails { get; }

        public ThreadScrapeResults(
            string? threadHtml,
            Upload[] uploads,
            Thumbnail[] thumbnails
        )
        {
            ThreadHtml = threadHtml;
            Uploads = uploads ?? Array.Empty<Upload>();
            Thumbnails = thumbnails ?? Array.Empty<Thumbnail>();
        }
    }
}