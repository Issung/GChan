using GChan.Models;
using GChan.Properties;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GChan.Helpers
{
    /// <summary>
    /// A pool in which tasks can be thrown in at will. Ensures that only <see cref="MaxConcurrentTasks"/> are ever running at once.
    /// </summary>
    public class TaskPool<T>
    {
        /// <summary>
        /// A delegate that creates a task to be run.
        /// </summary>
        public delegate Task<T> Factory();

        /// <summary>
        /// Listener for when a task completes. That could mean either success or failure.
        /// </summary>
        public delegate void OnComplete(Task<T> task);

        public int MaxConcurrentTasks => Settings.Default.MaximumConcurrentDownloads;

        /// <summary>Is the task pool currently running at capacity.</summary>
        public bool Full => runningTasks.Count >= MaxConcurrentTasks;

        private readonly ConcurrentQueue<Factory> queue = new();
        private readonly ConcurrentHashSet<Task<T>> runningTasks = new();
        private readonly OnComplete completionListener;

        public TaskPool(OnComplete completionListener)
        {
            this.completionListener = completionListener;

#pragma warning disable CS4014 // Run in background
            Run();
#pragma warning restore CS4014
        }

        public void Enqueue(Factory factory)
        {
            queue.Enqueue(factory);
        }

        private async Task Run()
        {
            while (true)
            {
                // If the MaxConcurrentTasks decreases or increases this should handle it just fine.
                if (runningTasks.Count < MaxConcurrentTasks && MaybeDequeue(out var factory))
                {
                    var newTask = factory();
                    runningTasks.Add(newTask);
                }

                var firstCompletedTask = await Task.WhenAny(runningTasks);
                runningTasks.Remove(firstCompletedTask);
                completionListener(firstCompletedTask);
            }
        }

        private bool MaybeDequeue(out Factory factory)
        {
            return queue.TryDequeue(out factory);
        }
    }
}
