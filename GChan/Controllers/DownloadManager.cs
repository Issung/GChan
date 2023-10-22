using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GChan.Controllers
{
    /// <summary>
    /// Class that manages a file download pool.<br/>
    /// <typeparamref name="T"/> must provide a good implementation of <see cref="object.GetHashCode"/>.<br/>
    /// </summary>
    /// <remarks>
    /// TODO: Add ability to clear the manager completely, for certain situations e.g. someone disables the setting to save thread html.<br/>
    /// TODO: Use async/tasks instead of threads.<br/>
    /// </remarks>
    public class DownloadManager<T> : IDisposable where T: IDownloadable<T>
    {
        public delegate void SuccessCallback(T item);
        public delegate void FailureCallback(T item, bool retry);

        private const int ConcurrentCount = 25;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan interval = TimeSpan.FromSeconds(1);

        private readonly ConcurrentDictionary<T, Download> downloading = new();
        private readonly ConcurrentQueue<T> waiting = new();
        private readonly bool removeSuccessfulItems;
        private readonly Timer timer;
        private readonly string typeName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="removeSuccessfulItems">
        /// Should items that successfully download be removed from the download manager?<br/>
        /// If false, after a successful download will enter the back of the queue again, for later re-downloading.
        /// </param>
        public DownloadManager(bool removeSuccessfulItems)
        { 
            this.removeSuccessfulItems = removeSuccessfulItems;
            this.timer = new(TimerTick, null, TimeSpan.Zero, interval);
            this.typeName = typeof(T).Name + "s";
        }

        public void Queue(T item)
        {
            if (!waiting.Contains(item))    // TODO: What is the performance of this?
            { 
                waiting.Enqueue(item);
            }
        }

        public void Queue(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                logger.Trace("Queueing {item} for download.", item);
                waiting.Enqueue(item);
            }
        }

        /// <summary>
        /// Cancel a download of an item if it is currently downloading.<br/>
        /// To cancel downloads of items that are queued for download, set <see cref="IDownloadable{T}.ShouldDownload"/> to false.
        /// </summary>
        public void Cancel(T item)
        {
            if (downloading.TryRemove(item, out var download))
            {
                download.Dispose();
            }
        }

        /// <summary>
        /// Cancel all currently downloading items that match <paramref name="predicate"/>.<br/>
        /// To cancel downloads of items that are queued for download, set <see cref="IDownloadable{T}.ShouldDownload"/> to false.
        /// </summary>
        /// <remarks>
        /// TODO: Not thread-safe because the dictionary could change over the loop.
        /// </remarks>
        public void Cancel(Func<T, bool> predicate)
        {
            foreach (var kvp in downloading) 
            {
                var item = kvp.Key;
                var download = kvp.Value;

                if (predicate(item))
                {
                    downloading.TryRemove(kvp.Key, out var _);
                    download.Cancel();
                }
            }
        }

        public void Clear()
        {
            while (downloading.Count > 0) 
            {
                var kvp = downloading.FirstOrDefault();
                downloading.TryRemove(kvp.Key, out _);
                kvp.Value.Dispose();
            }
        }

        /// <summary>
        /// A tick of the timer.
        /// </summary>
        /// <param name="_">Unnecessary paramater.</param>
        private void TimerTick(object _)
        {
            var dequeueCount = ConcurrentCount - downloading.Count;
            var items = Dequeue(dequeueCount);
            logger.Trace("Dequeue {dequeue_count} {type} from queue, got {dequeue_result_count}.", dequeueCount, typeName, items.Count);  // TODO: Appears to be double-logging.

            // TODO: If no images were found in queue set the timer to a slightly longer interval, to stop poll spamming.

            foreach (var item in items)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var thread = new Thread(() => item.Download(DownloadSuccess, DownloadFailed, cancellationTokenSource.Token));

                var download = new Download(thread, cancellationTokenSource);
                thread.Start();

                downloading.TryAdd(item, download);
            }
        }

        /// <summary>
        /// Take some items off of the queue.
        /// </summary>
        private List<T> Dequeue(int amount)
        {
            var chunk = new List<T>(amount);

            while (chunk.Count < amount && waiting.TryDequeue(out var item))
            {
                if (item.ShouldDownload)
                {
                    chunk.Add(item);
                }
                else
                {
                    // If shouldn't download don't add to chunk. The item has already been removed from the queue.
                }
            }

            return chunk;
        }

        /// <summary>
        /// Called when a download has completed successfully.<br/>
        /// Removes <paramref name="item"/> from the downloading dict.
        /// </summary>
        private void DownloadSuccess(T item)
        {
            logger.Trace("Item {item} completed downloading succesfully.", item);

            if (downloading.TryRemove(item, out var _))
            {
                // If manager is not supposed remove items after a successful download, add back onto the queue.
                if (!removeSuccessfulItems)
                {
                    waiting.Enqueue(item);
                }
            }
            else
            { 
                logger.Warn("DownloadSuccess callback was called with {item} but was not in the downloading dictionary.", item);
            }
        }

        /// <summary>
        /// Called when a download was unable to complete.<br/>
        /// Removes <paramref name="item"/> from the downloading dict and requeues it pending download.<br/>
        /// If the failure is permanent (e.g. image is gone) then <paramref name="retry"/> can be set to false.
        /// </summary>
        private void DownloadFailed(T item, bool retry)
        {
            logger.Trace("Item {item} downloading failed.", item);

            if (downloading.TryRemove(item, out var _))
            {
                if (retry)
                { 
                    waiting.Enqueue(item);
                }
            }
            else
            {
                logger.Warn("DownloadFailed callback was called with {item} but was not in the downloading dictionary.", item);
            }
        }

        public void Dispose()
        {
            timer.Dispose();

            foreach (var kvp in downloading)
            {
                var download = kvp.Value;
                download.Cancel();
                download.Dispose();
            }
        }
    }
}
