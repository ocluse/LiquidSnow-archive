
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class GlobalExtensions
{
    public static object GetPropValue(this object obj, string name)
    {
        foreach (string part in name.Split('.'))
        {
            if (obj == null) { return null; }

            Type type = obj.GetType();
            PropertyInfo info = type.GetProperty(part);
            if (info == null) { return null; }

            obj = info.GetValue(obj, null);
        }
        return obj;
    }

    public static T GetPropValue<T>(this object obj, string name)
    {
        object retval = GetPropValue(obj, name);
        if (retval == null) { return default; }

        // throws InvalidCastException if types are incompatible
        return (T)retval;
    }
}

namespace System.Collections.ObjectModel
{
    public static class ObjectModelExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> list)
        {
            if (list == null) return;
            foreach (var i in list)
            {
                collection.Add(i);
            }
        }

        public static void RemoveAll<T>(this ObservableCollection<T> collection, IEnumerable<T> list)
        {
            if (list == null) return;
            foreach(var i in list)
            {
                collection.Remove(i);
            }
        }

        public static IEnumerable<T> FindAll<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            return collection.Where(predicate);
        }

        public static T Find<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            return collection.FirstOrDefault(predicate);
        }

        public static bool Exists<T>(this ObservableCollection<T> collection, Func<T, bool>predicate)
        {
            return collection.Any(predicate);
        }
    }
}

namespace System.Collections.Generic
{
    public static class GenericCollectionsExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var random = new System.Random();
                var r = random.Next(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}