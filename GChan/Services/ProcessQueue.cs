using GChan.Models.Trackers;
using GChan.Properties;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Services
{
    /// <summary>
    /// A queue for <see cref="IProcessable"/>s. Controls how often they are started.
    /// </summary>
    // TODO: Maybe need a high priority queue for UI interactions (threads/boards being added by user). Take from that queue first before falling back to main queue.
    // And maybe a low priority queue for thumbnails? IProcessable should have an enum property "Priority" that can change itself (e.g. a new thread is high, and after first scrape goes back to normal).
    public class ProcessQueue
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly Action<Tracker> addTrackerCallback;
        private readonly CancellationToken shutdownCancellationToken;
        private readonly ConcurrentQueue<IProcessable> queue = new();
        private readonly TaskPool<ProcessResult> pool;
        private readonly Task task;

        public ProcessQueue(
            Action<Tracker> addTrackerCallback,
            CancellationToken shutdownCancellationToken
        )
        {
            this.addTrackerCallback = addTrackerCallback;
            this.shutdownCancellationToken = shutdownCancellationToken;
            this.pool = new(HandleResult);

            task = Task.Factory.StartNew(WorkAsync, TaskCreationOptions.LongRunning);
        }

        public void Enqueue(IProcessable processable)
        {
            queue.Enqueue(processable);
        }

        public async Task WorkAsync()
        {
            while (!shutdownCancellationToken.IsCancellationRequested)
            {
                var max1PerSecond = Settings.Default.Max1RequestPerSecond;  // Save setting temporarily incase it changes mid-loop.

                if (max1PerSecond)
                {
                    await semaphore.WaitAsync();
                }

                var item = MaybeDequeue();

                if (item != null)
                {
                    var combinedToken = Utils.CombineCancellationTokens(shutdownCancellationToken, item.CancellationToken);

                    logger.Debug("Adding processable to process pool: {processable}.", item);
                    
                    pool.Enqueue(async () => await item.ProcessAsync(combinedToken));
                }

                if (max1PerSecond)
                {
                    semaphore.Release();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        /// <summary>
        /// Dequeues processables until one is found that desires being processed, or the queue is depleted (returns null).
        /// </summary>
        private IProcessable? MaybeDequeue()
        {
            while (true)
            {
                if (queue.TryDequeue(out var downloadable))
                {
                    if (downloadable.ShouldProcess)
                    {
                        return downloadable;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        private void HandleResult(ProcessResult result)
        {
            if (!result.RemoveFromQueue)
            {
                Enqueue(result.Processable);

                logger.Debug("Requeuing processable: {processable}.", result.Processable);
            }

            foreach (var newProcessable in result.NewProcessables)
            {
                // Board may return new threads as processables.
                if (newProcessable is Tracker tracker)
                {
                    addTrackerCallback(tracker);    // The callback will enqueue the tracker.
                }
                else
                {
                    Enqueue(newProcessable);
                }
            }
        }
    }
}
