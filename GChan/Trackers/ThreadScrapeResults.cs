using GChan;
using System.Collections.Generic;

public class ThreadScrapeResults
{
    public string ThreadHtml { get; }
    public IEnumerable<ImageLink> ImageLinks { get; }

    public ThreadScrapeResults(
        string threadHtml,
        IEnumerable<ImageLink> imageLinks
    )
    {
        ThreadHtml = threadHtml;
        ImageLinks = imageLinks;
    }
}