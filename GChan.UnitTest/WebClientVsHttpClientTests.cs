using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

#pragma warning disable SYSLIB0014 // Type or member is obsolete (WebClient)

namespace GChan.UnitTest
{
    /// <summary>
    /// Not really tests, these are here so they can be run easily and the network request can be inspected with toos like Wireshark / Fiddler.<br/>
    /// They both do 2 requests so you can inspect cookie behaviour.
    /// </summary>
    public class WebClientVsHttpClientTests
    {
        private readonly Uri thread = new("https://a.4cdn.org/po/thread/570368.json");
        private const string userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 14_7_1) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.0 Safari/605.1.15";

        [Fact]
        public void TestWebClient()
        {
            var client = new WebClient();

            client.Headers.Add("User-Agent", userAgent);
            client.DownloadString(thread);

            client.Headers.Add("User-Agent", userAgent);    // Has to be re-added before the next request? Weird.
            client.DownloadString(thread);
        }

        [Fact]
        public async Task TestHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
            await client.GetStringAsync(thread);
            await client.GetStringAsync(thread);
        }
    }
}

#pragma warning restore SYSLIB0014 // Type or member is obsolete (WebClient)