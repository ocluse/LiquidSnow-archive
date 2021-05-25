using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Thismaker.Core.Models
{
    /// <summary>
    /// A <see cref="Queue{T}"/> that raises events whenever it is modified through adding or removing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableQueue<T>:Queue<T>,INotifyPropertyChanged,INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableQueue() : base() { }

        /// <summary>
        /// Creates a queue with the capacity to hold the specified number of items
        /// </summary>
        /// <param name="capacity">The capacity for the queue</param>
        public ObservableQueue(int capacity) : base(capacity) { }

        /// <summary>
        /// Initializes a queue copying the items in the collection
        /// </summary>
        /// <param name="collection">the collection to copy the items from</param>
        public ObservableQueue(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        /// Queues a new item, raising the collection changed event
        /// </summary>
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            CollectionChanged?.Invoke(this, e); 
        }

        /// <summary>
        /// Dequeues an item, raising the collection changed event
        /// </summary>
        /// <returns></returns>
        public new T Dequeue()
        {
            var item = base.Dequeue();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);

            CollectionChanged?.Invoke(this, e);
            return item;
        }

        /// <summary>
        /// Clears the items in the queue notifying collection change
        /// </summary>
        public new void Clear()
        {
            var oldItems = new List<T>(this);
            base.Clear();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, oldItems);

            CollectionChanged?.Invoke(this, e);
        }

        public new void TrimExcess()
        {
            base.TrimExcess();
            var e = new PropertyChangedEventArgs(nameof(Count));
            PropertyChanged?.Invoke(this, e);
        }
    }
}
