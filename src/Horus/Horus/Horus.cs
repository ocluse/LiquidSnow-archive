using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Thismaker.Horus
{
    /// <summary>
    /// Contains utility methods for generating IDs, random numbers and calculating hashes
    /// </summary>
    public class Horus
    {
        #region Generation
        /// <summary>
        /// Generates an arguably unique string of characters to be used for unique identification. 
        /// The uniqueness strength boils down to the <see cref="IdKind"/> used.
        /// Some IDs may not be suitable for certain scenarios
        /// </summary>
        /// <param name="kind">The kind of ID to be generated</param>
        /// <param name="length">The length of the string to be generated, only applicable for <see cref="IdKind.Standard"/> up to 36 characters and <see cref="IdKind.Random"/> for unlimited characters</param>
        public static string GenerateId(IdKind kind = IdKind.Guid, int length = 12)
        {
            return kind switch
            {
                IdKind.DateTime => GenerateDateTimeId(),
                IdKind.Guid => GenerateGuid(),
                IdKind.Hash => GenerateHashedId(),
                IdKind.Random => Random(length, true),
                _ => GenerateStandardId(length),
            };
        }

        private static string GenerateDateTimeId()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
            return id.ToUpper();
        }

        private static string GenerateStandardId(int count)
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(count)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }

        private static string GenerateGuid()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        private static string GenerateHashedId()
        {
            string start = GenerateId();
            return GetHash(start);
        }
        #endregion

        #region Randomnization
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates a random integer
        /// </summary>
        /// <param name="min">The minimum inclusive value</param>
        /// <param name="max">The maximum exclusive value</param>
        /// <returns>A randomly generated integer</returns>
        public static int Random(int min, int max)
        {
            return _random.Next(min, max);
        }

        /// <summary>
        /// Generates a random string of the specified <paramref name="length"/>
        /// </summary>
        /// <param name="length">The number of characters to include in the string</param>
        /// <param name="lowerCase">If true, returns lowercase characters, otherwise uppercase</param>
        /// <returns>A randomly generated string</returns>
        public static string Random(int length = 8, bool lowerCase = false)
        {
            StringBuilder builder = new StringBuilder(length);

            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;
            for (int i = 0; i < length; i++)
            {
                char @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();

        }

        #endregion

        #region Hash

        /// <summary>
        /// Computes the SHA256 hash value of file
        /// </summary>
        /// <param name="path">The path of the file whose hash value is to be computed</param>
        /// <returns>A string representing the hash value of the file</returns>
        public static string ComputeFileHash(string path)
        {
            using SHA256 alg = SHA256.Create();
            using FileStream stream = File.OpenRead(path);
            byte[] bytes = alg.ComputeHash(stream);

            return GetHexString(bytes);
        }

        /// <summary>
        /// Computes the hash of an array of bytes
        /// </summary>
        /// <param name="data">The data to be hashed</param>
        /// <returns>The resulting byte array from the operation</returns>
        public static byte[] GetHash(byte[] data)
        {
            using SHA256 alg = SHA256.Create();
            
            return alg.ComputeHash(data);
        }

        /// <summary>
        /// Computes the hash of a string
        /// </summary>
        /// <param name="input">The string to be hashed</param>
        /// <returns>A string representing the hash of the <paramref name="input"/></returns>
        public static string GetHash(string input)
        {
            byte[] bytes = GetHash(input.GetBytes<UTF8Encoding>());
            
            return GetHexString(bytes);
        }

        private static string GetHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        #endregion
    }

    /// <summary>
    /// The type of ID to be generated by <see cref="Horus.GenerateId(IdKind, int)"/>
    /// </summary>
    public enum IdKind
    {
        /// <summary>
        /// Generates an ID based on <see cref="System.Guid"/>
        /// </summary>
        Guid,
        /// <summary>
        /// Generates a GUID that is transformed into an alphanumeric squence.
        /// </summary>
        Standard,

        /// <summary>
        /// ID Generated by this method is simply the <see cref="Guid"/> but passed through a Hash function for 256 characters
        /// </summary>
        Hash,

        /// <summary>
        /// Generates an ID based on the <see cref="System.DateTime"/>, conflicts may occur depending on the time, but are rare.
        /// </summary>
        DateTime,

        /// <summary>
        /// Generates an ID based on <see cref="System.Random"/>
        /// </summary>
        Random
    }
}
