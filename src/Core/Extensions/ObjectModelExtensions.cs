using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    public static class ObjectModelExtensions
    {
        /// <summary>
        /// Adds a range of items to the collection
        /// </summary>
        /// <param name="items">The items to add</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (items == null) return;
            foreach (var i in items)
            {
                collection.Add(i);
            }
        }


        /// <summary>
        /// Removes all the items provided from the collection
        /// </summary>
        /// <param name="items">The items to remove</param>
        public static void RemoveAll<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (items == null) return;
            foreach (var i in items)
            {
                collection.Remove(i);
            }
        }

        /// <summary>
        /// Removes all the items from the collection that match the predicate,
        /// returning the number of items removed
        /// </summary>
        public static int RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            var removable = collection.Where(predicate).ToList();

            foreach (var itemToRemove in removable)
            {
                collection.Remove(itemToRemove);
            }

            return removable.Count;
        }

        /// <summary>
        /// Finds all items in the collection that match the provided predicate.
        /// This is a simple LINQ function, I honestly have no idea why I added it here.
        /// Oh wait to make it look like a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindAll<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            return collection.Where(predicate);
        }

        /// <summary>
        /// Finds the first item that matches the predicate, otherwise it returns null or an 
        /// equivalent default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Find<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            return collection.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Checks whether any item in the collection matches the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Exists<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            return collection.Any(predicate);
        }

        /// <summary>
        /// Sorts the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the collection.</typeparam>
        /// <param name="collection">The collection to sort.</param>
        /// <param name="comparison">The comparison used for sorting.</param>
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison = null)
        {
            var sortableList = new List<T>(collection);
            if (comparison == null)
                sortableList.Sort();
            else
                sortableList.Sort(comparison);

            for (var i = 0; i < sortableList.Count; i++)
            {
                var oldIndex = collection.IndexOf(sortableList[i]);
                var newIndex = i;
                if (oldIndex != newIndex)
                    collection.Move(oldIndex, newIndex);
            }
        }


    }
}