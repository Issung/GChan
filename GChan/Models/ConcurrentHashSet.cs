using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GChan.Models
{
    /// <summary>
    /// Credit: Ben Mosher https://stackoverflow.com/a/18923091/8306962
    /// </summary>
    public class ConcurrentHashSet<T> : IEnumerable<T>, IDisposable
    {
        protected readonly ReaderWriterLockSlim locker = new(LockRecursionPolicy.SupportsRecursion);
        protected readonly HashSet<T> set = new();

        public int Count
        {
            get
            {
                locker.EnterReadLock();

                try
                {
                    return set.Count;
                }
                finally
                {
                    if (locker.IsReadLockHeld)
                    {
                        locker.ExitReadLock();
                    }
                }
            }
        }

        public bool Add(T item)
        {
            locker.EnterWriteLock();

            try
            {
                return set.Add(item);
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            locker.EnterWriteLock();

            try
            {
                foreach (T item in items)
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

        public void Clear()
        {
            locker.EnterWriteLock();
            try
            {
                set.Clear();
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }
        }

        public bool Contains(T item)
        {
            locker.EnterReadLock();

            try
            {
                return set.Contains(item);
            }
            finally
            {
                if (locker.IsReadLockHeld)
                {
                    locker.ExitReadLock();
                }
            }
        }

        public bool Remove(T item)
        {
            locker.EnterWriteLock();

            try
            {
                return set.Remove(item);
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }
        }

        public T[] ToArray() 
        {
            locker.EnterReadLock();

            try
            {
                return set.ToArray();
            }
            finally
            {
                if (locker.IsReadLockHeld)
                {
                    locker.ExitReadLock();
                }
            }
        }

        /// <remarks>Is this correct? What happens if the enumerator changes during enumeration? Should we return a copy with ToArray() instead?</remarks>
        public IEnumerator<T> GetEnumerator()
        {
            return set.GetEnumerator();
        }

        /// <remarks>Is this correct? What happens if the enumerator changes during enumeration? Should we return a copy with ToArray() instead?</remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return set.GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                locker?.Dispose();
            }
        }

        ~ConcurrentHashSet()
        {
            Dispose(false);
        }
    }
}
