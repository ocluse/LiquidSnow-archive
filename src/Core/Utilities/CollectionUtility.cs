using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Thismaker.Core.Utilities
{
    static class CollectionUtility
    {
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
    }
}
