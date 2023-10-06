using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace GChan.Controllers
{
    /// <summary>
    /// Class that manages a file download pool.
    /// </summary>
    public class FileDownloadController
    {
        private const int ConcurrentCount = 25;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan interval = TimeSpan.FromSeconds(1);

        private readonly ConcurrentDictionary<ImageLink, Thread> downloading = new();
        private readonly ConcurrentQueue<ImageLink> waiting = new();
        private readonly Timer timer;

        public FileDownloadController()
        { 
            timer = new(TimerTick, null, interval, interval);
        }

        public void Queue(ImageLink link)
        {
            waiting.Enqueue(link);
        }

        public void Queue(IEnumerable<ImageLink> links)
        {
            foreach (var link in links)
            {
                logger.Log(LogLevel.Info, "Queueing {image_link} for download.", link);
                waiting.Enqueue(link);
            }
        }

        /// <summary>
        /// A tick of the timer.
        /// </summary>
        /// <param name="_">Unnecessary paramater.</param>
        private void TimerTick(object _)
        {
            var skimCount = ConcurrentCount - downloading.Count;
            logger.Log(LogLevel.Info, "Skimming {skim_count} images from queue.", skimCount);
            var items = Skim(skimCount);

            // TODO: If no images were found in queue set the timer to a slightly longer interval, to stop poll spamming.

            foreach (var image in items)
            {
                var newThread = new Thread(() => image.Download(DownloadComplete));
                newThread.Start();
                downloading.TryAdd(image, newThread);
            }

            // TODO: Find all failed threads, remove them, set them back into the waiting list.
            //foreach (...)
            //{ 
            //
            //}
        }

        /// <summary>
        /// Take some links off of the queue.
        /// </summary>
        private List<ImageLink> Skim(int amount)
        {
            var chunk = new List<ImageLink>(amount);

            while (chunk.Count < amount && waiting.TryDequeue(out var item))
            { 
                chunk.Add(item);
            }

            return chunk;
        }

        /// <summary>
        /// Called when a download has completed successfully.<br/>
        /// Removes <paramref name="imageLink"/> from the downloading list.
        /// </summary>
        private void DownloadComplete(ImageLink imageLink)
        {
            logger.Log(LogLevel.Info, "Link {image_link} completed downloading succesfully.", imageLink);
            downloading.TryRemove(imageLink, out var _);
        }
    }
}
