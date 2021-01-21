using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace Thismaker.Enigma
{
    

    /// <summary>
    /// A managed class to be used for the sole purpose of mordern encryption.
    /// Data encrypted using the "default" methods of this class may not be decryptable 
    /// on other environments, due to use of default settings. It is recommended that you use "manual"
    /// methods if you wish to export the encrypted data externally. If, however you will be encrypting/decrypting locally, 
    /// or to a different environment that uses Enigma, it is highly recommended to use the "default" methods.
    /// This class may rely on System.Security.Cryptography.
    /// Classical encryption options can be found under the static Classical class.
    /// </summary>
    public static partial class Enigma
    {
        #region Randomnization
        private static readonly Random _random = new Random();

        public static string Random(int size=8, bool lowerCase=false)
        {
            var builder = new StringBuilder(size);

            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();

        }

        public static int Random(int min, int max)
        {
            return _random.Next(min, max);
        }

        #endregion

        #region Misc
        public static string ComputeFileHash(string path)
        {
            using var alSHA = SHA256.Create();
            using var stream = File.OpenRead(path);
            var bytes = alSHA.ComputeHash(stream);
            string result = "";
            foreach (var b in bytes) result += b.ToString("X2");
            return result;
        }

        public static byte[] GetHash(byte[] inputData)
        {
            using var alSHA = SHA256.Create();
            return alSHA.ComputeHash(inputData);
        }

        public static byte[] GetHash(string inputString)
        {
            return GetHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        #endregion
    }

    
}
