using System.Threading;
using System.Threading.Tasks;

namespace GChan
{
    public class ProcessResult
    {
        /// <summary>
        /// Should this download be retried.
        /// </summary>
        public bool Retry { get; }

        public ProcessResult(bool retry)
        {
            this.Retry = retry;
        }
    }

    /// <summary>
    /// An item that can be processed.
    /// </summary>
    public interface IProcessable
    {
        /// <summary>
        /// A cancellation token provided by the this processable, for when it believes it should no longer be processed.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Should this item be downloaded.<br/>
        /// Decision may have changed since being added to download manager.<br/>
        /// If this ever goes false it should never go back to being true.
        /// </summary>
        public bool ShouldProcess { get; }

        /// <summary>
        /// Perform download for this item.
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken for this <see cref="IProcessable"/> combined with another CancellationToken for program shutdown.</param>
        Task<ProcessResult> ProcessAsync(CancellationToken cancellationToken);
    }
}