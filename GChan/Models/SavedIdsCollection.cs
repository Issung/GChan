using System;
using System.Linq;

namespace GChan.Models
{
    /// <summary>
    /// Thread safe collection of <see cref="long"/>s. For saving downloaded image ids.
    /// </summary>
    public class SavedIdsCollection : ConcurrentHashSet<long>
    {
        public SavedIdsCollection() 
        { 
        
        }

        /// <param name="list">Comma delimited list of ints.</param>
        public SavedIdsCollection(string list)
        {
            LoadStringList(list);
        }

        /// <param name="list">Comma delimited list of ints.</param>
        public void LoadStringList(string list)
        {
            var splits = list.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var items = splits.Select(long.Parse);

            locker.EnterWriteLock();

            try
            {
                foreach (var item in items)
                {
                    set.Add(item);
                }
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }
        }

        public string ToStringList()
        {
            var array = ToArray();
            return string.Join(",", array);
        }
    }
}
