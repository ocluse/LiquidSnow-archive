using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Server.CosmosDb
{
    internal static class Extensions
    {
        //TODO this should be removed ASAP, when targeting is increased
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
