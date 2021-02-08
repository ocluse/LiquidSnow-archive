using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Thismaker.Core.Utils
{
    /// <summary>
    /// A see <see cref="Stack{T}"/> that raises events when changed. Useful for binding and stuff
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableStack<T> : Stack<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public ObservableStack(int capacity) : base(capacity) { }
        public ObservableStack(IEnumerable<T> collection) : base(collection) { }
        public ObservableStack() { }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public new void Clear()
        {
            var oldItems = new List<T>(this);
            base.Clear();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, oldItems);
            CollectionChanged?.Invoke(this, e);
        }

        public new T Pop()
        {
            var item = base.Pop();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            CollectionChanged?.Invoke(this, e);
            return item;
        }

        public new virtual void Push(T item)
        {
            base.Push(item);
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            CollectionChanged?.Invoke(this, e);
        }

        public new virtual void TrimExcess()
        {
            base.TrimExcess();
            var e = new PropertyChangedEventArgs(nameof(Count));

            PropertyChanged?.Invoke(this, e);
        }
    }
}
