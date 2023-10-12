using GChan.Controllers;

namespace GChan
{
    public interface IDownloadable<T> where T : IDownloadable<T>
    {
        /// <summary>
        /// Should this item be downloaded.<br/>
        /// Decision may have changed since being added to download manager.
        /// </summary>
        public bool ShouldDownload { get; }

        /// <summary>
        /// Perform download for this item.
        /// </summary>
        void Download(
            DownloadManager<T>.SuccessCallback successCallback,
            DownloadManager<T>.FailureCallback failureCallback
        );
    }
}