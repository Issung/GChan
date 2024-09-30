using System.Threading;
using System.Threading.Tasks;

namespace GChan
{
    public class DownloadResult
    {
        /// <summary>
        /// Should this download be retried.
        /// </summary>
        public bool Retry { get; }

        public DownloadResult(bool retry)
        {
            this.Retry = retry;
        }
    }

    public interface IDownloadable
    {
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Should this item be downloaded.<br/>
        /// Decision may have changed since being added to download manager.
        /// </summary>
        public bool ShouldDownload { get; }

        /// <summary>
        /// Perform download for this item.
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken for this <see cref="IDownloadable"/> combined with another CancellationToken for program shutdown.</param>
        Task<DownloadResult> DownloadAsync(CancellationToken cancellationToken);
    }
}