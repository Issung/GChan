using GChan.Data.Models;
using GChan.Forms;
using GChan.Properties;
using GChan.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace GChan.Models.Trackers
{
    public abstract class Board : Tracker, IProcessable, INotifyPropertyChanged
    {
        private int threadCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public int ThreadCount 
        {
            get => threadCount; 
            set
            {
                var update = threadCount != value;
                threadCount = value;

                if (update)
                {
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The greatest Thread ID added to tracking.<br/>
        /// This is used to ignore old thread ids in <see cref="ProcessAsync"/>.
        /// </summary>
        public long GreatestThreadId { get; set; }

        public bool ShouldProcess => !cancellationTokenSource.IsCancellationRequested;

        public DateTimeOffset? ReadyToProcessAt => LastAttempt + TimeSpan.FromSeconds(Settings.Default.MinSecondsBetweenScrapes);

        public ProcessPriority Priority => ProcessPriority.Default;

        protected Board(string url) : base(url)
        {
            Type = Type.Board;
            
        }

        protected Board(BoardData data) : base($"https://boards.4chan.org/{data.Code}/")
        {
            this.LastModified = data.LastScrape;
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if DEBUG
            logger.Trace($"NotifyPropertyChanged! Board.{propertyName}.");
#endif
            MainForm.StaticInvoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        public async Task<ProcessResult> ProcessAsync(ProcessableParams parameters, CancellationToken cancellationToken)
        {
            LastAttempt = DateTimeOffset.Now;

            var (threads, lastModified) = await GetThreadsImpl(cancellationToken);

            if (threads == null)
            {
                LastModified = lastModified;
                return new(removeFromQueue: false);
            }

            ThreadCount = threads.Length;

            var newThreads = threads.Where(t => t.Id > GreatestThreadId).ToArray();

            if (newThreads.Length == 0)
            {
                LastModified = lastModified;
                return new(removeFromQueue: false);
            }

            var greatestNewThreadId = newThreads.Max(t => t.Id);

            // Only record this scrape (GreatestThreadId, and LastModified which is sent as If-Modified-Since) once its threads
            // are in the thread list. A save that sees the new values then also sees the threads, and if adding them fails the
            // next scrape finds them again instead of skipping them or getting a 304.
            return new(
                removeFromQueue: false,
                newProcessables: newThreads,
                onNewProcessablesAdded: () =>
                {
                    GreatestThreadId = Math.Max(GreatestThreadId, greatestNewThreadId);
                    LastModified = lastModified;
                }
            );
        }

        /// <summary>
        /// Returns the board's threads and the server's last-modified time to store in <see cref="Tracker.LastModified"/>.<br/>
        /// Threads may be null if they have not been modified since <see cref="Tracker.LastModified"/>.
        /// </summary>
        protected abstract Task<(Thread[]? Threads, DateTimeOffset LastModified)> GetThreadsImpl(CancellationToken cancellationToken);

        public override string ToString()
        {
            return $"{SiteDisplayName} - /{BoardCode}/ - ({ThreadCount} Threads)";
        }

        new public ValueTask DisposeAsync() => base.DisposeAsync();
    }
}