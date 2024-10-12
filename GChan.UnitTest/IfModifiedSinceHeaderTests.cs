using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GChan.UnitTest
{
    /// <summary>
    /// Verification of 4chans behaviour with the thread endpoint and the If-Modified-Since header.<br/>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-Modified-Since"/>
    /// </summary>
    public class IfModifiedSinceHeaderTests
    {
        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36";
        private readonly Uri thread = new Uri("https://a.4cdn.org/po/thread/570368.json");
        private readonly HttpClient client = MakeClient();


        [Fact]
        public async Task Test_NoHeaderPresent_Returns200WithContent()
        {
            var response = await client.GetAsync(thread);
            response.StatusCode.Should().Be(HttpStatusCode.OK, because: "request without If-Modified-Since header should return 200");

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Match("{*}", because: "request without If-Modified-Since header should return JSON content");
        }

        [Fact]
        public async Task Test_IfModifiedSinceHeaderPresent_Returns304WithoutContent()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = thread,
                Headers =
                {
                    IfModifiedSince = DateTimeOffset.UtcNow,
                },
            };

            var response = await client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.NotModified);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().BeEmpty();
        }

        private static HttpClient MakeClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            return client;
        }
    }
}
