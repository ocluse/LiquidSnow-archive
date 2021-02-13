using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Thismaker.Horus
{
	static class Extensions
	{
		/// <summary>
		/// Makes all letters uppercase and removes any spaces.
		/// Useful for most classical encryption stuff
		/// </summary>
		/// <param name="strInput"></param>
		/// <returns></returns>
		public static string CapitalizeAndCompress(this string strInput)
		{
			strInput = Regex.Replace(strInput, @"\s+", "");
			strInput = strInput.Trim();
			strInput = strInput.ToUpper();
			return strInput;
		}

		/// <summary>
		/// Relaces a character in the string, with the provided string,
		/// returning a string with the character replaced.
		/// </summary>
		public static string ReplaceAt(this string strInput, int index, char c)
		{
			StringBuilder sb = new StringBuilder(strInput);
			sb[index] = c;
			return sb.ToString();
		}


		public static byte[] ToBytesBase64(this string strInput)
        {
			return Convert.FromBase64String(strInput);
        }

		public static string ToBase64(this string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		/// <summary>
		/// True if the string contains the specified character
		/// </summary>
		public static bool Contains(this string str, char c)
        {
			foreach(var ch in str){
				if (ch == c) return true;
            }
			return false;
        }
	}
}
