using System;
using System.Collections.Generic;

namespace Thismaker.Esna
{
    internal static class Extensions
    {
        internal const string DIR_STRUCT_ID = "_directory.structure";

        //TODO this should be removed ASAP, when targeting .NET level is increased
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }
    }
}