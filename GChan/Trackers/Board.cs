namespace GChan.Trackers
{
    public abstract class Board : Tracker
    {
        protected int threadCount;

        public int ThreadCount 
        { 
            get 
            { 
                return threadCount; 
            } 
        }

        /// <summary>
        /// The greatest Thread ID added to tracking.<br/>
        /// This is used to ignore old thread ids in <see cref="GetThreadLinks"/>.
        /// </summary>
        public long GreatestThreadId { get; set; }

        protected Board(string url) : base(url)
        {
            Type = Type.Board;
        }

        public abstract string[] GetThreadLinks();

        public override string ToString()
        {
            return $"{SiteName} - /{BoardCode}/ - ({ThreadCount} Threads)";
        }
    }
}