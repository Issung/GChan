using System;
using System.Net.Http;

namespace GChan.Exceptions;

public class TooManyRequestsException(HttpResponseMessage httpResponseMessage) : Exception("Received 429 (Too Many Requests) status code.")
{
    public HttpResponseMessage HttpResponseMessage { get; set; } = httpResponseMessage;

    public TimeSpan? RetryAfter => HttpResponseMessage.Headers.RetryAfter?.Delta;
}