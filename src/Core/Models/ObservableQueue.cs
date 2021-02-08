using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Thismaker.Core.Utils
{
    /// <summary>
    /// A <see cref="Queue{T}"/> that raises events whenenevr it changes. Useful for binding scenarios
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableQueue<T>:Queue<T>,INotifyPropertyChanged,INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableQueue() : base() { }

        public ObservableQueue(int capacity) : base(capacity) { }

        public ObservableQueue(IEnumerable<T> collection) : base(collection) { }

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            CollectionChanged?.Invoke(this, e); 
        }

        public new T Dequeue()
        {
            var item = base.Dequeue();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);

            CollectionChanged?.Invoke(this, e);
            return item;
        }

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
