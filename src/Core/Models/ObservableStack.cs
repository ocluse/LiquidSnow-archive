using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Thismaker.Core.Models
{
    /// <summary>
    /// A see <see cref="Stack{T}"/> that raises events when changed.
    /// </summary>
    public class ObservableStack<T> : Stack<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// Creates a new instance with the provided capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public ObservableStack(int capacity) : base(capacity) { }
        
        /// <summary>
        /// Creates a new instance from the provided collection.
        /// </summary>
        /// <param name="collection"></param>
        public ObservableStack(IEnumerable<T> collection) : base(collection) { }
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ObservableStack() { }

        /// <summary>
        /// Raised when changes occur to the underlying collection.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        /// <summary>
        /// Raised when changes occur to the properties of the stack
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<inheritdoc cref="Stack{T}.Clear"/>
        public new void Clear()
        {
            var oldItems = new List<T>(this);
            base.Clear();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, oldItems);
            CollectionChanged?.Invoke(this, e);
        }

        ///<inheritdoc cref="Stack{T}.Pop"/>
        public new T Pop()
        {
            var item = base.Pop();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            CollectionChanged?.Invoke(this, e);
            return item;
        }

        ///<inheritdoc cref="Stack{T}.Push"/>
        public new virtual void Push(T item)
        {
            base.Push(item);
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            CollectionChanged?.Invoke(this, e);
        }

        ///<inheritdoc cref="Stack{T}.TrimExcess"/>
        public new virtual void TrimExcess()
        {
            base.TrimExcess();
            var e = new PropertyChangedEventArgs(nameof(Count));

            PropertyChanged?.Invoke(this, e);
        }
    }
}
