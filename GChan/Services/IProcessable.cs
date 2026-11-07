using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Thread = GChan.Models.Trackers.Thread;

namespace GChan.Services
{
    /// <summary>
    /// Hooks that processables might need to call for functionality outside of themselves.
    /// </summary>
    public interface IProcessableHooks
    {
        /// <summary>
        /// A hook for threads to call to remove themselves from the thread list.
        /// </summary>
        Task ThreadRemoveSelf(Thread thread);
    }

    public record ProcessableParams(IProcessableHooks Hooks);

    public class ProcessResult
    {
        /// <summary>
        /// Should this processable be removed from the queue.
        /// </summary>
        public bool RemoveFromQueue { get; }

        /// <summary>
        /// A collection of new processables to be added to the processing queue.
        /// </summary>
        public IEnumerable<IProcessable> NewProcessables { get; }

        /// <summary>
        /// Called once all of <see cref="NewProcessables"/> have been added (new trackers to their lists, others to the queue).
        /// Not called if adding them fails, in which case the processable is retried.
        /// </summary>
        public Action OnNewProcessablesAdded { get; }

        public ProcessResult(
            bool removeFromQueue,
            IEnumerable<IProcessable> newProcessables = null,
            Action onNewProcessablesAdded = null
        )
        {
            RemoveFromQueue = removeFromQueue;
            NewProcessables = newProcessables ?? [];
            OnNewProcessablesAdded = onNewProcessablesAdded;
        }
    }

    /// <summary>
    /// The priority queue this is used with will dequeue the lowest priority first, so sort with highest priority first.
    /// </summary>
    public enum ProcessPriority
    {
        High,
        Default,
        Low,
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
        /// Should this item be processed<br/>
        /// Decision may have changed since being added to queue.<br/>
        /// If this ever goes false it should never go back to being true, thus it should be discarded from the queue.
        /// </summary>
        public bool ShouldProcess { get; }

        /// <summary>
        /// When will this processable by ready to process.<br/>
        /// Set to null for any time, otherwise the item is deferred until it is next found in the queue with a ready time.
        /// </summary>
        public DateTimeOffset? ReadyToProcessAt { get; }

        /// <summary>
        /// Decides what internal queue this processable is placed upon in <see cref="ProcessQueue"/>.
        /// Any default priority processables will be processed before low priority processables.
        /// </summary>
        public ProcessPriority Priority { get; }

        /// <summary>
        /// Perform download for this item.<br/>
        /// Must never throw, must always return a <see cref="ProcessResult"/>.
        /// </summary>
        /// <param name="parameters"></param>
        // TODO: Make sure the never throw rule is done in all implementations.
        /// <param name="cancellationToken">The CancellationToken for this <see cref="IProcessable"/> combined with another CancellationToken for program shutdown.</param>
        Task<ProcessResult> ProcessAsync(ProcessableParams parameters, CancellationToken cancellationToken);
    }
}