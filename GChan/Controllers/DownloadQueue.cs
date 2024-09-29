using GChan.Properties;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Controllers
{
    public class DownloadQueue
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly CancellationToken cancellationToken;
        private readonly ConcurrentQueue<IDownloadable> queue = new();

        public DownloadQueue(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

#pragma warning disable CS4014 // Run Task in background.
            WorkAsync();
#pragma warning restore CS4014
        }

        public void Enqueue(IDownloadable download)
        {
            queue.Enqueue(download);
        }

        public async Task WorkAsync()
        {
            bool max1PerSecond;

            while (!cancellationToken.IsCancellationRequested)
            {
                max1PerSecond = Settings.Default.Max1RequestPerSecond;

                if (max1PerSecond)
                {
                    await semaphore.WaitAsync();
                }

                var item = await DequeueAsync();
                var result = await item.DownloadAsync();

                HandleResult(item, result);

                if (max1PerSecond)
                {
                    semaphore.Release();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        /// <summary>
        /// Holds control until a downloadable with <see cref="IDownloadable.ShouldDownload"/> true is found.
        /// </summary>
        private async Task<IDownloadable> DequeueAsync()
        {
            while (true)
            {
                if (queue.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                if (queue.TryDequeue(out var downloadable))
                {
                    if (downloadable.ShouldDownload)
                    {
                        return downloadable;
                    }
                }
            }
        }

        private void HandleResult(IDownloadable item, DownloadResult result)
        {
            if (result.Retry)
            {
                queue.Enqueue(item);
            }
        }
    }
}
