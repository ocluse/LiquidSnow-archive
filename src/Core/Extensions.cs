
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

public static class GlobalExtensions
{
    /// <summary>
    /// Quickly check if a character array is the same as the input string
    /// </summary>
    /// <param name="value">The string to see if is same as the character array</param>
    /// <returns>true if the character array is equal to the provided string</returns>
    public static bool IsString(this char[] chars, string value)
    {
        var test = new string(chars);
        return test == value;
    }

    /// <summary>
    /// Quickly tests if a string is similar to a character array.
    /// </summary>
    /// <param name="chars">The character array to test whether is the same as the string</param>
    /// <returns>true if the string is equal to the character array</returns>
    public static bool IsCharArray(this string value, char[] chars)
    {
        var test = new string(chars);
        return test == value;
    }

    /// <summary>
    /// Checks if a double is a perfect square
    /// </summary>
    /// <param name="input"></param>
    /// <returns>true if a double is a perfect square, i.e the squareroot is an integer.</returns>
    public static bool IsPerfectSquare(this double input)
    {
        var sqrt = Math.Sqrt(input);
        return Math.Abs(Math.Ceiling(sqrt) - Math.Floor(sqrt)) < double.Epsilon;
    }

    /// <summary>Similar to <see cref="string.Substring(int,int)"/>, only for arrays. Returns a new
    /// array containing <paramref name="length"/> items from the specified
    /// <paramref name="startIndex"/> onwards.</summary>
    public static T[] Subarray<T>(this T[] array, int startIndex, int length)
    {
        if (array == null)
            throw new ArgumentNullException("array");
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException("startIndex", "startIndex cannot be negative.");
        if (length < 0 || startIndex + length > array.Length)
            throw new ArgumentOutOfRangeException("length", "length cannot be negative or extend beyond the end of the array.");
        T[] result = new T[length];
        Array.Copy(array, startIndex, result, 0, length);
        return result;
    }

    /// <summary>
    /// Gets the bytes of a string when encoded using the specified method
    /// </summary>
    /// <typeparam name="T">The type of Encoding to be used</typeparam>
    /// <param name="str">The string whose bytes are to be returned</param>
    /// <returns></returns>
    public static byte[] GetBytes<T>(this string str) where T : Encoding
    {
        if (typeof(T) == typeof(ASCIIEncoding))
        {
            return Encoding.ASCII.GetBytes(str);
        }
        else if (typeof(T) == typeof(UTF8Encoding))
        {
            return Encoding.UTF8.GetBytes(str);
        }
        else if (typeof(T) == typeof(UTF7Encoding))
        {
            return Encoding.UTF7.GetBytes(str);
        }
        else if (typeof(T) == typeof(UTF32Encoding))
        {
            return Encoding.UTF32.GetBytes(str);
        }
        else if (typeof(T) == typeof(UnicodeEncoding))
        {
            return Encoding.Unicode.GetBytes(str);
        }

        throw new ArgumentException("The encoding provided is unkown/unsupported");
    }

    /// <summary>
    /// Gets the string represented by a byte array
    /// </summary>
    /// <typeparam name="T">The encoding to use</typeparam>
    /// <returns>A string represented by the encoding in the provided bytes</returns>
    public static string GetString<T>(this byte[] ba) where T : Encoding
    {
        if (typeof(T) == typeof(ASCIIEncoding))
        {
            return Encoding.ASCII.GetString(ba);
        }
        else if (typeof(T) == typeof(UTF8Encoding))
        {
            return Encoding.UTF8.GetString(ba);
        }
        else if (typeof(T) == typeof(UTF7Encoding))
        {
            return Encoding.UTF7.GetString(ba);
        }
        else if (typeof(T) == typeof(UTF32Encoding))
        {
            return Encoding.UTF32.GetString(ba);
        }
        else if (typeof(T) == typeof(UnicodeEncoding))
        {
            return Encoding.Unicode.GetString(ba);
        }

        throw new ArgumentException("The encoding provided is unknown/unsupported");
    }

    /// <summary>
    /// Returns the current value of a property with the provided name in the object.
    /// </summary>
    /// <param name="propertyName">The name of the property to retrieve</param>
    /// <returns></returns>
    public static object GetPropValue(this object obj, string propertyName)
    {
        var prop = obj.GetType().GetProperty(propertyName);
        return prop.GetValue(obj);
    }

    /// <summary>
    /// Returns the value of a property with the provided name in the object.
    /// This method casts the value obtained to <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to cast the property to</typeparam>
    /// <param name="obj"></param>
    /// <param name="propertyName">The name of the property to retrieve</param>
    /// <returns>the value of the property</returns>
    public static T GetPropValue<T>(this object obj, string propertyName)
    {
        return (T)GetPropValue(obj, propertyName);
    }

    /// <summary>
    /// Sets the value of a property with the provided name to the provided value
    /// </summary>
    /// <param name="value">The value to set to the property</param>
    /// <param name="propertyName">The name of the property</param>
    public static void SetPropValue(this object obj, string propertyName, object value)
    {
        var prop = obj.GetType().GetProperty(propertyName);
        prop.SetValue(obj, value);
    }

    /// <summary>
    /// Converts a string to the block format, where the first letter is capitalized and the rest are converted to small letters.
    /// </summary>
    /// <returns></returns>
    public static string ToBlock(this string s)
    {
        // Check for empty string.  
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.  
        return char.ToUpper(s[0]) + s.Substring(1).ToLower();
    }

    /// <summary>
    /// Returns true if a string is composed of letters and numbers only.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsAlphaNumeric(this string s)
    {
        return s.All(char.IsLetterOrDigit);
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

        public static int RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            var removable = collection.Where(predicate).ToList();

            foreach (var itemToRemove in removable)
            {
                collection.Remove(itemToRemove);
            }

            return removable.Count;
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

namespace System.Collections.Generic
{

    public static class GenericCollectionsExtensions
    {
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];

            list.RemoveAt(oldIndex);

            if (newIndex > oldIndex) newIndex--;
            // the actual index could have shifted due to the removal

            list.Insert(newIndex, item);
        }

        public static void Move<T>(this IList<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1)
                {
                    list.RemoveAt(oldIndex);

                    if (newIndex > oldIndex) newIndex--;
                    // the actual index could have shifted due to the removal

                    list.Insert(newIndex, item);
                }
            }

        }

        public static IEnumerable<T[]> SearchPattern<T>(this IEnumerable<T> seq, params Func<T[], T, bool>[] matches)
        {
            Contract.Requires(seq != null);
            Contract.Requires(matches != null);
            Contract.Requires(matches.Length > 0);

            // No need to create a new array if seq is already one
            var seqArray = seq as T[] ?? seq.ToArray();

            // Check every applicable position for the matching pattern
            for (int j = 0; j <= seqArray.Length - matches.Length; j++)
            {
                // If this position matches...
                if (Enumerable.Range(0, matches.Length).All(i =>
                    matches[i](seqArray.Subarray(j, i), seqArray[i + j])))
                {
                    // ... yield it
                    yield return seqArray.Subarray(j, matches.Length);

                    // and jump to the item after the match so we don’t get overlapping matches
                    j += matches.Length - 1;
                }
            }
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
        /// Rotates the items on a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static List<T> Rotate<T>(this List<T> list, int offset)
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

            return list;
            
        }

        /// <summary>
        /// Rotates a list by using the LINQ namespace. you may want to avoid this if you intend
        /// to strip assemblies or the likes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static List<T> RotateLinquish<T>(this List<T> list, int offset)
        {
            return list.Skip(offset).Concat(list.Take(offset)).ToList();
        }
    }
}

namespace System.IO

{
    public static class SystemIOExtensions
    {
        /// <summary>
        /// Reads the contents of the stream into a byte array.
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>A byte array containing the contents of the stream.</returns>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static byte[] ReadAllBytes(this Stream source)
        {
            long originalPosition = source.Position;
            source.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];
                int totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = source.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = source.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                source.Position = originalPosition;
            }
        }
    }
}