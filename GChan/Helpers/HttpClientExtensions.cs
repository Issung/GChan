using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Helpers
{
    public class StatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public StatusCodeException(HttpStatusCode statusCode) : base($"Unexpected http status code {statusCode}.")
        {
            StatusCode = statusCode;
        }
    }

    public static class HttpClientExtensions
    {
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="StatusCodeException"/>
        public static async Task<byte[]> GetByteArrayAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new StatusCodeException(response.StatusCode);
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="StatusCodeException"/>
        public static async Task<string> GetStringAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new StatusCodeException(response.StatusCode);
            }

            // TODO: Use CancellationToken variant (.NET 5 onwards)
            return await response.Content.ReadAsStringAsync();
        }

        /// <returns><c>null</c> if <see cref="HttpResponseMessage.StatusCode"/> is <see cref="HttpStatusCode.NotModified"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="StatusCodeException"/>
        public static async Task<string?> GetStringAsync(
            this HttpClient client,
            string requestUri,
            DateTimeOffset? ifModifiedSince,
            CancellationToken cancellationToken
        )
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(requestUri),
                Method = HttpMethod.Get,
                Headers =
                {
                    IfModifiedSince = ifModifiedSince,
                },
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new StatusCodeException(response.StatusCode);
            }

            // TODO: Use CancellationToken variant (.NET 5 onwards)
            return await response.Content.ReadAsStringAsync();
        }
    }
}
