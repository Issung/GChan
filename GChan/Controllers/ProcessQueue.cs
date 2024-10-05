using GChan.Helpers;
using GChan.Properties;
using GChan.Trackers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Controllers
{
    /// <summary>
    /// A queue for <see cref="IProcessable"/>s. Controls how often they are started.
    /// </summary>
    // TODO: Maybe need a high priority queue for UI interactions (threads/boards being added by user). Take from that queue first before falling back to main queue.
    public class ProcessQueue
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly Action<Tracker> addTrackerCallback;
        private readonly CancellationToken shutdownCancellationToken;
        private readonly ConcurrentQueue<IProcessable> queue = new();
        private readonly TaskPool<ProcessResult> pool;  // TODO: Do something with the completion listener.
        private readonly Task task;

        public ProcessQueue(
            Action<Tracker> addTrackerCallback,
            CancellationToken shutdownCancellationToken
        )
        {
            this.addTrackerCallback = addTrackerCallback;
            this.shutdownCancellationToken = shutdownCancellationToken;
            this.pool = new(HandleResult);

            //task = WorkAsync();    // Something is dodgy, if both this class & TaskPool use Task.Run only 1 gets to run.
            //Task.Run(WorkAsync);
            Task.Factory.StartNew(WorkAsync, TaskCreationOptions.LongRunning);
        }

        public void Enqueue(IProcessable download)
        {
            queue.Enqueue(download);
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

                var item = await DequeueAsync();

                var combinedToken = Utils.CombineCancellationTokens(shutdownCancellationToken, item.CancellationToken);

                pool.Enqueue(async () => await item.ProcessAsync(combinedToken));

                if (max1PerSecond)
                {
                    semaphore.Release();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    //await Task.Yield();
                }
            }
        }

        /// <summary>
        /// Holds control until a downloadable with <see cref="IProcessable.ShouldProcess"/> true is found.
        /// </summary>
        private async Task<IProcessable> DequeueAsync()
        {
            while (true)
            {
                if (queue.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                if (queue.TryDequeue(out var downloadable))
                {
                    if (downloadable.ShouldProcess)
                    {
                        return downloadable;
                    }
                }
            }
        }

        private void HandleResult(ProcessResult result)
        {
            if (!result.RemoveFromQueue)
            {
                Enqueue(result.Processable);
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
