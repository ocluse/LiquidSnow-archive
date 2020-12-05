using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Thismaker.Core.Utils
{
    //public class ObservableStack : Stack, INotifyPropertyChanged, INotifyCollectionChanged
    //{
    //    public ObservableStack(IEnumerable collection) : base(collection) { }
    //    public ObservableStack() { }

    //    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    //    public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

    //    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, List items, int? index = null)
    //    {
    //        if (index.HasValue)
    //        {
    //            CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, items, index.Value));
    //        }
    //        else
    //        {
    //            CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, items));
    //        }
    //        OnPropertyChanged(GetPropertyName(() => Count));
    //    }

    //    protected virtual void OnPropertyChanged(string propName)
    //    {
    //        PropertyChanged(this, new PropertyChangedEventArgs(propName));
    //    }

    //    public new virtual void Clear()
    //    {
    //        base.Clear();
    //        OnCollectionChanged(NotifyCollectionChangedAction.Reset, null);
    //    }

    //    public new virtual T Pop()
    //    {
    //        var result = base.Pop();
    //        OnCollectionChanged(NotifyCollectionChangedAction.Remove, new List() { result }, base.Count);
    //        return result;
    //    }

    //    public new virtual void Push(T item)
    //    {
    //        base.Push(item);
    //        OnCollectionChanged(NotifyCollectionChangedAction.Add, new List() { item }, base.Count - 1);
    //    }

    //    public new virtual void TrimExcess()
    //    {
    //        base.TrimExcess();
    //        OnPropertyChanged(GetPropertyName(() => Count));
    //    }

    //    String GetPropertyName(Expression> propertyId)
    //    {
    //        return ((MemberExpression)propertyId.Body).Member.Name;
    //    }

    //}
}
