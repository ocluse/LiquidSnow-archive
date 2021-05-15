using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly:InternalsVisibleTo("Thismaker.Horus.Classical")]
[assembly: InternalsVisibleTo("Thismaker.Horus.IO")]

namespace Thismaker.Horus
{
    public class Horus
    {
        #region Generation
        /// <summary>
        /// Generates an arguably unique string of characters to be used for unique identification. 
        /// The uniqueness strength boils down to the <see cref="IDKind"/> used, depending on the scenario.
        /// Some IDs may not be suitable for different scenarios
        /// </summary>
        /// <param name="kind">The kind of ID to be generated, determines the strength as well as the look</param>
        /// <param name="length">The length of the string to be generated, only applicable when the kind is set to Standard or Random</param>
        /// <returns></returns>
        public static string GenerateID(IDKind kind = IDKind.GUID, int length = 12)
        {
            return kind switch
            {
                IDKind.DateTime => GenerateDTID(),
                IDKind.GUID => GenerateGUID(),
                IDKind.Hash => GenerateSHAID(),
                IDKind.Random => Random(length, true),
                _ => GenerateSTDID(length),
            };
        }

        private static string GenerateDTID()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
            return id.ToUpper();
        }

        private static string GenerateSTDID(int count)
        {
            var builder = new StringBuilder();
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

        private static string GenerateGUID()
        {
            var guid = Guid.NewGuid();
            return guid.ToString();
        }

        private static string GenerateSHAID()
        {
            var start = GenerateID();
            return GetHashString(start);
        }
        #endregion

        #region Randomnization
        private static readonly Random _random = new Random();

        public static int Random(int min, int max)
        {
            return _random.Next(min, max);
        }

        /// <summary>
        /// Returns a random string of the specified <paramref name="size"/>
        /// </summary>
        /// <param name="size">The number of characters to include in the string</param>
        /// <param name="lowerCase">If true, returns lowercase characters, otherwise uppercase</param>
        public static string Random(int size = 8, bool lowerCase = false)
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

        #endregion

        #region Hash

        /// <summary>
        /// Quickly computes the hash of a file
        /// </summary>
        /// <param name="path">Path to the file whose hash is to be calculated</param>
        /// <returns></returns>
        public static string ComputeFileHash(string path)
        {
            using var alSHA = SHA256.Create();
            using var stream = File.OpenRead(path);
            var bytes = alSHA.ComputeHash(stream);
            string result = "";
            foreach (var b in bytes) result += b.ToString("X2");
            return result;
        }

        /// <summary>
        /// Returns the hash of an array of data
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static byte[] GetHash(byte[] inputData)
        {
            using var alSHA = SHA256.Create();
            return alSHA.ComputeHash(inputData);
        }

        /// <summary>
        /// Returns the hash of a string. No idea why you'd want that,
        /// but hey I made it so....
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static byte[] GetHash(string inputString)
        {
            return GetHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Hashes a string.
        /// </summary>
        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        #endregion
    }

    public enum IDKind
    {
        /// <summary>
        /// Generates the strongest ID, should be used for most default scenarios
        /// </summary>
        GUID,
        /// <summary>
        /// Generates a standard ID, of 12 characters in length, conflicts are rare. Characters are all alphanumeric
        /// </summary>
        Standard,

        /// <summary>
        /// ID Generated by this method is simply the GUID but passed through a Hash function for 256 characters
        /// </summary>
        Hash,

        /// <summary>
        /// Generates an ID based on the System.DateTime, conflicts may occur depending on the time, but are rare. May contain hyphen
        /// </summary>
        DateTime,

        /// <summary>
        /// Generates an ID based on the System.Random, conflicts are a matter of pure randomnity, not suitable for conflict-sensitive scenarios
        /// </summary>
        Random,
    }
}
