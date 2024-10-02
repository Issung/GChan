using GChan;
using System.Collections.Generic;

public class ThreadScrapeResults
{
    public string ThreadHtml { get; }
    public IEnumerable<Asset> Thumbnails { get; }

    public ThreadScrapeResults(
        string threadHtml,
        IEnumerable<Asset> imageLinks
    )
    {
        ThreadHtml = threadHtml;
        Thumbnails = imageLinks;
    }
}