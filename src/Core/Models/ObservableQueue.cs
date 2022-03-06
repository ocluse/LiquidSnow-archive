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
        /// <summary>
        /// Raised when an item is queued or dequeued, or if the queue structure changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        /// <summary>
        /// Raised when a property of the queue changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates an instance of the observable queue.
        /// </summary>
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

        ///<inheritdoc cref="Queue{T}.Enqueue(T)"/>
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            CollectionChanged?.Invoke(this, e); 
        }

        ///<inheritdoc cref="Queue{T}.Dequeue"/>
        public new T Dequeue()
        {
            var item = base.Dequeue();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);

            CollectionChanged?.Invoke(this, e);
            return item;
        }

        ///<inheritdoc cref="Queue{T}.Clear"/>
        public new void Clear()
        {
            var oldItems = new List<T>(this);
            base.Clear();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, oldItems);

            CollectionChanged?.Invoke(this, e);
        }

        ///<inheritdoc cref="Queue{T}.TrimExcess"/>
        public new void TrimExcess()
        {
            base.TrimExcess();
            var e = new PropertyChangedEventArgs(nameof(Count));
            PropertyChanged?.Invoke(this, e);
        }
    }
}
