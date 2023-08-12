using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GChan.Controllers
{
    /// <summary>
    /// Class that manages file downloading.
    /// </summary>
    public class FileDownloadController
    {
        private const int SkimCount = 10;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan interval = TimeSpan.FromSeconds(10);

        private readonly ConcurrentDictionary<ImageLink, Thread> tasks = new();
        private readonly ConcurrentQueue<ImageLink> queue = new();
        private readonly Timer timer;


        public FileDownloadController()
        { 
            timer = new(Skim, null, interval, interval);
        }

        /// <summary>
        /// Take some links off of the queue, 
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Skim(object state)
        {
            var chunk = new List<ImageLink>(SkimCount);

            while (chunk.Count < SkimCount && queue.TryDequeue(out var item))
            { 
                chunk.Add(item);
            }

            foreach (var link in chunk)
            { 
                
            }
        }

        public void Queue(ImageLink link)
        {
            queue.Enqueue(link);
        }

        public bool Remove(ImageLink imageLink) 
        {
            return tasks.TryRemove(imageLink, out var _);
        }
    }
}
