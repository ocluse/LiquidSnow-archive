using System;
using System.Collections.Generic;
using System.Text;

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

        public void Add(char item)
        {
            if (Contains(item)) return;
            items.Add(item);
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
