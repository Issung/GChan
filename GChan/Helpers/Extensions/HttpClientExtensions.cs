using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace GChan.Helpers.Extensions
{
    /// <param name="Content">The response body, or <c>null</c> if the resource was not modified (304).</param>
    /// <param name="LastModified">
    /// The server's time for this version of the resource, to send as <c>If-Modified-Since</c> next time.<br/>
    /// Taken from the <c>Last-Modified</c> header, falling back to the <c>Date</c> header, then to the local time the request was sent.
    /// </param>
    public record ConditionalGetResult(string? Content, DateTimeOffset LastModified);

    public static class HttpClientExtensions
    {
        /// <summary>
        /// GET <paramref name="requestUri"/> as a string, sending <paramref name="ifModifiedSince"/> as <c>If-Modified-Since</c>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        public static async Task<ConditionalGetResult> GetStringIfModifiedAsync(
            this HttpClient client,
            string requestUri,
            DateTimeOffset? ifModifiedSince,
            CancellationToken cancellationToken
        )
        {
            using var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(requestUri),
                Method = HttpMethod.Get,
                Headers =
                {
                    IfModifiedSince = ifModifiedSince,
                },
            };

            // Taken before sending, so if the server gives no time, nothing posted while the request is in flight is skipped next time.
            var sentAt = DateTimeOffset.Now;

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var lastModified = response.Content.Headers.LastModified ?? response.Headers.Date ?? sentAt;

            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                // A 304 may omit Last-Modified, in which case keep what we asked with rather than the response date.
                return new(null, response.Content.Headers.LastModified ?? ifModifiedSince ?? lastModified);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return new(content, lastModified);
        }
    }
}
