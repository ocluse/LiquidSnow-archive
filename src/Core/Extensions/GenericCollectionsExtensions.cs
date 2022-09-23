namespace System.Collections.Generic
{
    /// <summary>
    /// Contains extensions for the System.Collections.Generic namespace.
    /// </summary>
    public static class GenericCollectionsExtensions
    {
        /// <summary>
        /// Moves an item at the specified index to the new index
        /// </summary>
        /// <param name="list">The list to perform the operation on</param>
        /// <param name="oldIndex">The current index of the item</param>
        /// <param name="newIndex">The new index of the item</param>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            
            var item = list[oldIndex];

            list.RemoveAt(oldIndex);

            if (newIndex == list.Count)
            {
                list.Add(item);
            }
            else
            {
                list.Insert(newIndex, item);
            }
        }

        /// <summary>
        /// Moves an item in the list to the specified index
        /// </summary>
        /// <param name="list">The list to perform the operation on</param>
        /// <param name="item">The item to move</param>
        /// <param name="newIndex">The index to move the item to</param>
        public static void Move<T>(this IList<T> list, T item, int newIndex)
        {   
            var oldIndex = list.IndexOf(item);
            
            if (oldIndex == -1) 
                throw new NullReferenceException("Item not found in list");
            
            list.Move(oldIndex, newIndex);
        }

        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var random = new Random();
                var r = random.Next(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        /// <summary>
        /// Rotates the items on a list i.e offsets the postitions of the items, wrapping where necessary
        /// </summary>
        /// <param name="list">The list to perform the operation on</param>
        /// <param name="offset">How much to shift the items</param>
        /// <returns></returns>
        public static void Rotate<T>(this List<T> list, int offset)
        {
            if (offset >= 0)
            {
                for (; offset > 0; offset--)
                {
                    T first = list[0];
                    list.RemoveAt(0);
                    list.Add(first);
                }

            }
            else
            {
                for (; offset <= 0; offset++)
                {
                    var index = list.Count - 1;
                    T last = list[index];
                    list.RemoveAt(index);
                    list.Insert(0, last);
                }
            }        
        }
    }
}