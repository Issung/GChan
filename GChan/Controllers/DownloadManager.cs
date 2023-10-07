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
    /// TODO: Add "download" property to <see cref="IDownloadable{T}"/> so an item can skip downloading once it is attempted.<br/>
    /// TODO: Add <see cref="CancellationToken"/> to <see cref="IDownloadable{T}"/> and a way to cancel specific items / items that match a predicate.<br/>
    /// TODO: Add ability to clear the manager completely, for certain situations e.g. someone disables the setting to save thread html.<br/>
    /// TODO: Use async/tasks instead of threads.<br/>
    /// </remarks>
    public class DownloadManager<T> where T: IDownloadable<T>
    {
        public delegate void SuccessCallback(T item);
        public delegate void FailureCallback(T item, bool retry);

        private const int ConcurrentCount = 25;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan interval = TimeSpan.FromSeconds(1);

        private readonly ConcurrentDictionary<T, Thread> downloading = new();
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
                logger.Log(LogLevel.Info, "Queueing {item} for download.", item);
                waiting.Enqueue(item);
            }
        }

        /// <summary>
        /// A tick of the timer.
        /// </summary>
        /// <param name="_">Unnecessary paramater.</param>
        private void TimerTick(object _)
        {
            var skimCount = ConcurrentCount - downloading.Count;
            var items = Skim(skimCount);
            logger.Info("Skimming {skim_count} {type} from queue, got {skim_result_count}.", skimCount, typeName, items.Count);  // TODO: Appears to be double-logging.

            // TODO: If no images were found in queue set the timer to a slightly longer interval, to stop poll spamming.

            foreach (var image in items)
            {
                var newThread = new Thread(() => image.Download(DownloadSuccess, DownloadFailed));
                newThread.Start();
                downloading.TryAdd(image, newThread);
            }
        }

        /// <summary>
        /// Take some items off of the queue.
        /// </summary>
        private List<T> Skim(int amount)
        {
            var chunk = new List<T>(amount);

            while (chunk.Count < amount && waiting.TryDequeue(out var item))
            { 
                chunk.Add(item);
            }

            return chunk;
        }

        /// <summary>
        /// Called when a download has completed successfully.<br/>
        /// Removes <paramref name="item"/> from the downloading dict.
        /// </summary>
        private void DownloadSuccess(T item)
        {
            logger.Log(LogLevel.Debug, "Item {item} completed downloading succesfully.", item);

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
            logger.Log(LogLevel.Debug, "Item {item} completed downloading succesfully.", item);

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
    }
}
