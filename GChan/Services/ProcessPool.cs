using GChan.Properties;
using Nito.AsyncEx;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace GChan.Services
{
    /// <summary>
    /// Runs processables, with no more than <see cref="MaxConcurrentTasks"/> running at once.<br/>
    /// The pool has no backlog of its own: callers wait for <see cref="WaitForCapacityAsync"/> before calling <see cref="Start"/>,
    /// so ordering, spacing and backoff stay the responsibility of <see cref="ProcessQueue"/>.
    /// </summary>
    public class ProcessPool
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        /// <param name="CancellationTokenSource">
        /// Source of the token passed to the processable. Owned by the pool once started, and disposed when the processable completes.
        /// </param>
        public record Item(IProcessable Processable, ProcessableParams Params, CancellationTokenSource CancellationTokenSource)
        {
            public CancellationToken CancellationToken => CancellationTokenSource.Token;
        }

        /// <summary>
        /// Listener for when a task completes, successfully or not. <paramref name="completedTask"/> is always completed.<br/>
        /// Called before the task's slot is freed, so any state it sets (e.g. a rate-limit backoff) is visible before another task can start.
        /// </summary>
        public delegate void OnComplete(Item item, Task<ProcessResult> completedTask);

        public int MaxConcurrentTasks => Math.Max(1, Settings.Default.MaximumConcurrentDownloads);

        /// <summary>Is the pool currently running at capacity.</summary>
        public bool Full => runningCount >= MaxConcurrentTasks;

        /// <summary>Are any tasks currently running.</summary>
        public bool Processing => runningCount > 0;

        private readonly OnComplete completionListener;

        /// <summary>Set whenever a running task finishes.</summary>
        private readonly AsyncAutoResetEvent taskFinished = new();

        private int runningCount;

        public ProcessPool(OnComplete completionListener)
        {
            this.completionListener = completionListener;
        }

        /// <summary>
        /// Waits until another task can be started. If <see cref="MaxConcurrentTasks"/> changes this picks it up on the next finished task.
        /// </summary>
        public async Task WaitForCapacityAsync(CancellationToken cancellationToken)
        {
            while (Full)
            {
                await taskFinished.WaitAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Waits until no tasks are running.
        /// </summary>
        public async Task WaitForIdleAsync(CancellationToken cancellationToken)
        {
            while (Processing)
            {
                await taskFinished.WaitAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Starts processing <paramref name="item"/> immediately, on the thread pool.
        /// </summary>
        public void Start(Item item)
        {
            Interlocked.Increment(ref runningCount);
            _ = RunAsync(item);
        }

        private async Task RunAsync(Item item)
        {
            var task = Task.Run(() => item.Processable.ProcessAsync(item.Params, item.CancellationToken));

            try
            {
                await task;
            }
            catch
            {
                // Inspected by the completion listener.
            }

            try
            {
                completionListener(item, task);
            }
            catch (Exception e)
            {
                logger.Error(e, "An error occured while trying to notify process queue of process completion for {processable}.", item.Processable);
            }
            finally
            {
                item.CancellationTokenSource.Dispose();
                Interlocked.Decrement(ref runningCount);
                taskFinished.Set();
            }
        }
    }
}
