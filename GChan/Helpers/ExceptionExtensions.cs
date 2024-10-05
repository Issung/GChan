using GChan.Models.Trackers;
using System;
using System.Linq;
using System.Net;

namespace GChan.Helpers
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Does the <see cref="WebException"/> indicate content is no longer available (404 / 410 status code).
        /// </summary>
        /// <param name="httpWebResponse">If the exception comes from a http response, is set to the cast of <see cref="WebException.Response"/>.</param>
        public static bool IsGone(this WebException exception, out HttpWebResponse httpWebResponse)
        {
            var isProtocolError = exception.Status == WebExceptionStatus.ProtocolError;
            httpWebResponse = exception.Response as HttpWebResponse;
            var isGoneStatusCode = Tracker.GoneStatusCodes.Contains(httpWebResponse?.StatusCode);

            return isProtocolError && (httpWebResponse != null && isGoneStatusCode);
        }

        public static bool IsGone(this StatusCodeException exception)
        {
            return Tracker.GoneStatusCodes.Contains(exception.StatusCode);
        }
    }
}
