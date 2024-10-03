using GChan;
using GChan.Trackers;
using System.Collections.Generic;

public class ThreadScrapeResults
{
    public string ThreadHtml { get; }
    public IEnumerable<Thumbnail> Thumbnails { get; }

    public ThreadScrapeResults(
        string threadHtml,
        IEnumerable<Thumbnail> thumbnails
    )
    {
        ThreadHtml = threadHtml;
        Thumbnails = thumbnails;
    }
}