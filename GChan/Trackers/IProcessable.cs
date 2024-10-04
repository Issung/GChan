using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GChan
{
    public class ProcessResult
    {
        /// <summary>
        /// The processable this result comes from.
        /// </summary>
        public IProcessable Processable { get; }

        /// <summary>
        /// Should this processable be removed from the queue.
        /// </summary>
        public bool RemoveFromQueue { get; }

        /// <summary>
        /// A collection of new processables to be added to the processing queue.
        /// </summary>
        public IEnumerable<IProcessable> NewProcessables { get; }

        public ProcessResult(
            IProcessable processable,
            bool removeFromQueue,
            IEnumerable<IProcessable> newProcessables = null
        )
        {
            this.Processable = processable;
            this.RemoveFromQueue = removeFromQueue;
            this.NewProcessables = newProcessables ?? Enumerable.Empty<IProcessable>();
        }
    }

    /// <summary>
    /// An item that can be processed.
    /// </summary>
    public interface IProcessable : IAsyncDisposable
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