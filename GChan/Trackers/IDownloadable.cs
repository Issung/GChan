using GChan.Controllers;

namespace GChan
{
    public interface IDownloadable<T> where T : IDownloadable<T>
    {
        void Download(
            DownloadManager<T>.SuccessCallback successCallback,
            DownloadManager<T>.FailureCallback failureCallback
        );
    }
}