using System;
using System.Collections.Generic;
using System.Linq;

namespace Thismaker.Core.Utilities
{
    /// <summary>
    /// Utility methods for working with enums.
    /// </summary>
    public class EnumUtility
    {
        /// <summary>
        /// Gets all the values of an <see cref="Enum"/> as an <see cref="IEnumerable{T}"/> list
        /// Useful for iterating all the defined values of an enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
#if NET
        [Obsolete("Use Enum.GetValues<T> instead starting .NET 5", true)]
#endif
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
