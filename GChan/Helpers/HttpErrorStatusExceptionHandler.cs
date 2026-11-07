using GChan.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Helpers;

/// <summary>
/// A <see cref="HttpMessageHandler"/> that throws exceptions for different http status codes.<br/>
/// <list type="bullet">
/// <item><see cref="TooManyRequestsException"/> is thrown for 429 responses.</item>
/// <item><see cref="HttpRequestException"/> is thrown for all other status codes, via <see cref="HttpResponseMessage.EnsureSuccessStatusCode"/></item>
/// </list>
/// </summary>
public class HttpErrorStatusExceptionHandler : DelegatingHandler
{
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("This project does not intend to use any synchronous networking.");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        // Special exception for 429 status code.
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new TooManyRequestsException(response);
        }

        // Allow 304 status code to go through.
        if (response.StatusCode != HttpStatusCode.NotModified)
        {
            response.EnsureSuccessStatusCode();
        }

        return response;
    }
}
