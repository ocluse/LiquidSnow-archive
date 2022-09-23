using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Extensions methods for global typenames
/// </summary>
public static class GlobalExtensions
{
    /// <summary>
    /// Returns the largest prime factor of a number
    /// </summary>
    /// <returns></returns>
    public static ulong MaxFactor(this ulong n)
    {
        unchecked
        {
            while (n > 3 && 0 == (n & 1)) n >>= 1;

            uint k = 3;
            ulong k2 = 9;
            ulong delta = 16;
            while (k2 <= n)
            {
                if (n % k == 0)
                {
                    n /= k;
                }
                else
                {
                    k += 2;
                    if (k2 + delta < delta) return n;
                    k2 += delta;
                    delta += 8;
                }
            }
        }

        return n;
    }

    /// <summary>
    /// Quickly check if a character array is the same as the input string
    /// </summary>
    /// <param name="chars">An array of characters to determine if is equal to a string</param>
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
    /// <param name="value">The string to check if is equal to an array of characters</param>
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
    public static string GetString<T>(this byte[] ba) where T : Encoding, new()
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
    /// Converts an integer to it's equivalent byte array.
    /// </summary>
    /// <returns>A byte array represented by the int.</returns>
    public static byte[] GetBytes(this int val)
    {
        return BitConverter.GetBytes(val);
    }

    /// <summary>
    /// Converts a byte array to it's equivalent byte array.
    /// </summary>
    /// <returns>An int representing the byte array</returns>
    public static int GetInt(this byte[] bytes)
    {
        var i = BitConverter.ToInt32(bytes, 0);
        return i;
    }

    ///<inheritdoc cref="GetPropValue{T}(object, string)"/>
    public static object GetPropValue(this object obj, string propertyName)
    {
        var prop = obj.GetType().GetProperty(propertyName);
        return prop.GetValue(obj);
    }

    /// <summary>
    /// Gets the value of a property with the provided name in the object.
    /// </summary>
    /// <typeparam name="T">The type to cast the property to</typeparam>
    /// <param name="obj">The object whose property is to be obtained</param>
    /// <param name="propertyName">The name of the property to retrieve</param>
    /// <returns>The value of the property otained from the object</returns>
    public static T GetPropValue<T>(this object obj, string propertyName)
    {
        return (T)GetPropValue(obj, propertyName);
    }

    /// <summary>
    /// Sets the value of a property with the provided name to the provided value
    /// </summary>
    /// <param name="obj">The object whose property is to be set</param>
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
    /// <returns>A string that has been converted to block format</returns>
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
    /// Converts a string to the KebabCase where saces are replaced with dashes.
    /// </summary>
    /// <remarks>
    /// The returned string will always be lowercase. The method also inserts a dash between two capitalised letters, for example HelloWorld becomes hello-world.
    /// </remarks>
    /// <returns>The current string in Kebab case</returns>
    public static string ToKebabCase(this string value)
    {
        return value == null
                ? null
                : Regex.Replace(value,
                                 "([a-z])([A-Z])",
                                 "$1-$2",
                                 RegexOptions.CultureInvariant,
                                 TimeSpan.FromMilliseconds(100)).ToLowerInvariant().Replace(' ', '-');
    }

    /// <summary>
    /// Checks if a string is composed of letters and digits only.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>True if a string is composed of letters and numbers only.</returns>
    public static bool IsAlphaNumeric(this string s)
    {
        return s.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Checks whether the bytes of this array are the same as those of the second array
    /// </summary>
    /// <param name="a1">The source array</param>
    /// <param name="array">The array to check</param>
    /// <returns>True if the bytes are the same</returns>
    public static bool Compare(this byte[] a1, byte[] array)
    {
        if (a1.Length != array.Length)
            return false;

        for (int i = 0; i < a1.Length; i++)
            if (a1[i] != array[i])
                return false;

        return true;
    }

    /// <summary>
    /// Checks if a character exists in a string.
    /// </summary>
    /// <returns>
    /// True if the character exists in the string.
    /// </returns>
    public static bool Contains(this string str, char c)
    {
        foreach (var ch in str)
        {
            if (ch == c) return true;
        }
        return false;
    }
}
