using System.Collections.Generic;

namespace Thismaker.Horus.Classical
{
    public partial class Alphabet : IList<char>
    {
        public int Count =>items.Count;

        public bool IsReadOnly => false;

        public char this[int index] 
        { 
            get => items[index]; 
            set => items[index]=value; }

        public IEnumerator<char> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public int IndexOf(char item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, char item)
        {
            items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public bool Add(char item)
        {
            if (Contains(item)) return false;
            items.Add(item);
            return true;
        }

        void ICollection<char>.Add(char c)
        {

        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(char item)
        {
            return items.Contains(item);
        }

        public void CopyTo(char[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(char item)
        {
            return items.Remove(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

    }
}
