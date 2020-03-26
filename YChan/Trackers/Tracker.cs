using System.Text.RegularExpressions;

namespace GChan.Trackers
{
    public enum Type { Board, Thread };

    public abstract class Tracker
    {
        public string URL { get; protected set; }

        public string SaveTo { get; protected set; }

        public Type Type { get; protected set; }

        public string SiteName { get; protected set; }

        public string BoardCode { get; protected set; }

        protected Tracker(string url)
        {
            URL = url;
            BoardCode = FindBoardCode();
        }

        protected string FindBoardCode()
        {
            Regex regex = new Regex(@"(\/(\w|\d)+\/)");

            // Temporarily add a / incase url doesnt end with one.
            var matches = regex.Matches(URL + "/");

            if (matches.Count > 0)
            {
                return matches[0].Value;
            }
            else
            {
                return null;
            }
        }
    }
}
