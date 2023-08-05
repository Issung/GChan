using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;

namespace GChan.Controls
{
    /// <summary>
    /// Binding list that gives list changed events, also supports sorting for the WinForms UI.
    /// Attempts to be thread-safe as possible within reason.
    /// </summary>
    public class SortableBindingList<T> : BindingList<T>
    {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;

        private readonly object ilock = new();

        public SortableBindingList()
            : base(new List<T>())
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        public SortableBindingList(IEnumerable<T> enumeration)
            : base(new List<T>(enumeration))
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return propertyDescriptor; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return listSortDirection; }
        }

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        public new void Add(T item)
        {
            lock (ilock)
            {
                base.Add(item);
            }
        }

        public new T this[int index]
        {
            get
            {
                lock (ilock)
                {
                    return base[index];
                }
            }
            set
            {
                lock (ilock)
                {
                    base[index] = value;
                }
            }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> itemsList = (List<T>)Items;

            Type propertyType = property.PropertyType;
            PropertyComparer<T> comparer;
            if (!comparers.TryGetValue(propertyType, out comparer))
            {
                comparer = new PropertyComparer<T>(property, direction);
                comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            propertyDescriptor = property;
            listSortDirection = direction;
            isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
            propertyDescriptor = base.SortPropertyCore;
            listSortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {
            int count = Count;
            for (int i = 0; i < count; ++i)
            {
                T element = this[i];
                if (property.GetValue(element).Equals(key))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <returns>Item removed from <paramref name="index"/>.</returns>
        public new T RemoveAt(int index)
        {
            lock (ilock) 
            {
                var item = this[index];
                base.RemoveAt(index);
                return item;
            }
        }

        public new void Remove(T item)
        {
            lock (ilock)
            {
                if (Contains(item))
                {
                    base.Remove(item);
                }
            }
        }

        /// <returns>Removed items.</returns>
        public T[] RemoveAll(Predicate<T> match)
        {
            lock (ilock)
            {
                var itemsToRemove = new List<T>();

                foreach (T item in this)
                {
                    if (match(item))
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (T item in itemsToRemove)
                {
                    this.Remove(item);
                }

                return itemsToRemove.ToArray();
            }
        }

        /// <summary>
        /// Lock the list and foreach loop over the items (don't do long operations with this).
        /// </summary>
        public void ForEachLocked(Action<T> action)
        {
            lock (ilock)
            { 
                foreach (var item in this)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Thread Safe implementation.
        /// </summary>
        public List<T> ToList()
        {
            lock (ilock)
            {
                return Enumerable.ToList(this);
            }
        }

        /// <summary>
        /// Thread Safe implementation.
        /// </summary>
        public T[] ToArray()
        {
            lock (ilock)
            {
                return Enumerable.ToArray(this);
            }
        }

        public T[] Copy() => ToArray();
    }
}