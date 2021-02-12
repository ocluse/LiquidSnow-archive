using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Thismaker.Horus
{
	static class Extensions
	{
		public static string CapitalizeAndCompress(this string strInput)
		{
			strInput = Regex.Replace(strInput, @"\s+", "");
			strInput = strInput.Trim();
			strInput = strInput.ToUpper();
			return strInput;
		}

		public static string ReplaceAt(this string strInput, int index, char c)
		{
			StringBuilder sb = new StringBuilder(strInput);
			sb[index] = c;
			return sb.ToString();
		}

		public static byte[] ToBytesUTF8(this string strInput)
		{
			return Encoding.UTF8.GetBytes(strInput);
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

		public static string ToUTF8(this string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static bool Contains(this string str, char c)
        {
			foreach(var ch in str){
				if (ch == c) return true;
            }
			return false;
        }
	}
}
