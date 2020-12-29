namespace GChan.Trackers
{
    public abstract class Board : Tracker
    {
        protected int threadCount;

        public int ThreadCount { get { return threadCount; } }

        public int LargestAddedThreadNo { get; set; }

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
