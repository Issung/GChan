using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GChan.Models
{
    public delegate void SetChangedEventHandler();

    /// <summary>
    /// A thread-safe hash-set (credit <see href="https://stackoverflow.com/a/18923091/8306962">Ben Mosher</see>).<br/>
    /// Also raises change events via <see cref="SetChanged"/>, after releasing the lock: handlers marshal to the UI thread,
    /// and holding the lock while waiting on it would deadlock with the UI thread reading the set.
    /// </summary>
    public class ConcurrentHashSet<T> : IEnumerable<T>, IDisposable
    {
        protected readonly ReaderWriterLockSlim locker = new(LockRecursionPolicy.SupportsRecursion);
        protected readonly HashSet<T> set = [];

        public event SetChangedEventHandler SetChanged;

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
            bool didAdd;

            locker.EnterWriteLock();

            try
            {
                didAdd = set.Add(item);
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }

            if (didAdd)
            {
                SetChanged?.Invoke();
            }

            return didAdd;
        }

        public bool AddRange(IEnumerable<T> items)
        {
            bool anyAdditions;

            locker.EnterWriteLock();

            try
            {
                var results = items.Select(set.Add).ToArray();

                anyAdditions = results.Any(r => r == true);
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }

            if (anyAdditions)
            {
                SetChanged?.Invoke();
            }

            return anyAdditions;
        }

        public void Clear()
        {
            bool setHadItems;

            locker.EnterWriteLock();

            try
            {
                setHadItems = set.Any();

                set.Clear();
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }

            if (setHadItems)
            {
                SetChanged?.Invoke();
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
            bool didRemove;

            locker.EnterWriteLock();

            try
            {
                didRemove = set.Remove(item);
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }

            if (didRemove)
            {
                SetChanged?.Invoke();
            }

            return didRemove;
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

        /// <summary>
        /// Enumerates a snapshot taken under the lock, so other threads can modify the set during enumeration.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)ToArray()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
