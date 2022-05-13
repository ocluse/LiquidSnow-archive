using System.Collections.Generic;

namespace Thismaker.Horus.Classical
{
    public partial class Alphabet : IList<char>
    {
        /// <summary>
        /// Adds all the characters in the collection to the alphabet,
        /// returns the number of characters added to the alphabet.
        /// </summary>
        public int AddAll(IEnumerable<char> characters)
        {
            var added=0;
            foreach (var c in characters)
            {
                if(Add(c))added++;
            }

            return added;
        }

        /// <summary>
        /// Adds all the characters in the string to the collection,
        /// returning the number of added characters
        /// </summary>
        public int AddAll(string characters)
        {
            return AddAll(characters.ToCharArray());
        }

        /// <summary>
        /// Removes all the characters in the string from the alphabet,
        /// returning the number of removed characters
        /// </summary>
        public int RemoveAll(string characters)
        {
            return RemoveAll(characters.ToCharArray());
        }

        /// <summary>
        /// Removes all the characters in the collection from the alphabet,
        /// returning the number of characters removed.
        /// </summary>
        public int RemoveAll(IEnumerable<char> characters)
        {
            var removed = 0;
            foreach(var c in characters)
            {
                if(Remove(c))removed++;
            }
            return removed;
        }

        /// <summary>
        /// The number of characters in the alphabet.
        /// </summary>
        public int Count =>_items.Count;

        /// <summary>
        /// Always returns false for alphabets.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns a character at the provided index
        /// </summary>
        /// <param name="index">The index of the character</param>
        /// <returns></returns>
        public char this[int index] 
        { 
            get => _items[index]; 
            set => _items[index]=value; }

        ///<inheritdoc/>
        public IEnumerator<char> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the character, or -1 if not found in the alphabet
        /// </summary>
        public int IndexOf(char item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Inserts a character at the specified position, if the character
        /// exists in the alphabet, nothing happens.
        /// </summary>
        /// <param name="index">The index to insert the character</param>
        /// <param name="item">The character to insert</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public void Insert(int index, char item)
        {
            if (_items.Contains(item)) return;
            _items.Insert(index, item);
        }

        /// <summary>
        /// Removes the character at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        /// <summary>
        /// Adds the character to the alphabet, returning true if successful
        /// </summary>
        public bool Add(char item)
        {
            if (Contains(item)) return false;
            _items.Add(item);
            return true;
        }

        void ICollection<char>.Add(char item)
        {
            if (Contains(item)) return;
            _items.Add(item);
            return;
        }

        /// <summary>
        /// Clear all characters in the alphabet
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Checks if the alphabet contains the character
        /// </summary>
        public bool Contains(char item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Copies the characters in the alphabet to the provided character array
        /// </summary>
        /// <param name="array">The array to copy the characters to</param>
        /// <param name="arrayIndex">The index to start copying the characters</param>
        public void CopyTo(char[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a character from the alphabet, returning true if successful
        /// </summary>
        public bool Remove(char item)
        {
            return _items.Remove(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

    }
}
