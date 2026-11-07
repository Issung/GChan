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
    /// Also raises change events via <see cref="SetChanged"/>.
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
            locker.EnterWriteLock();

            try
            {
                var didAdd = set.Add(item);

                if (didAdd)
                {
                    SetChanged?.Invoke();
                }

                return didAdd;
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }
        }

        public bool AddRange(IEnumerable<T> items)
        {
            locker.EnterWriteLock();

            try
            {
                var results = items.Select(set.Add).ToArray();

                var anyAdditions = results.Any(r => r == true);

                if (anyAdditions)
                {
                    SetChanged?.Invoke();
                }

                return anyAdditions;
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
                var setHadItems = set.Any();

                set.Clear();

                if (setHadItems)
                {
                    SetChanged?.Invoke();
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
                var didRemove = set.Remove(item);

                if (didRemove)
                {
                    SetChanged?.Invoke();
                }

                return didRemove;
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
