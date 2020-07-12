using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GChan.Trackers
{
    public abstract class Board : Tracker
    {
        protected Board(string url) : base(url)
        {
            Type = Type.Board;
        }

        public abstract string[] GetThreadLinks();
    }
}
