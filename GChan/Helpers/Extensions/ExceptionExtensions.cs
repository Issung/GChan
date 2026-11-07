using GChan.Models.Trackers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace GChan.Helpers.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Does the <see cref="WebException"/> indicate content is no longer available (404 / 410 status code).
        /// </summary>
        public static bool IsGone(this HttpRequestException exception)
        {
            return Tracker.GoneStatusCodes.Contains(exception.StatusCode);
        }
    }
}
