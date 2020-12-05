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
    public static class Enigma
    {
        #region Settings

        private static int _iterations = 2;
        private static int _keySize = 256;

        private static string _hash = "SHA1";
        private static string _salt = "aselrias38490a32"; // Random
        private static string _vector = "8947az34awl34kjq"; // Random

        private const int AesKeySize = 16;
        #endregion

        #region Generation

        /// <summary>
        /// Generates an arguably unique string of characters to be used for unique identification. 
        /// The uniqueness strength boils down to the kind used, but the kind used depends on the scenario.
        /// Some IDs may not be suitable for different scenarios
        /// </summary>
        /// <param name="kind">The kind of ID to be generated, determines the strength as well as the look</param>
        /// <param name="length">The length of the string to be generated, only applicable when the kind is set to Standard or Random</param>
        /// <returns></returns>
        public static string GenerateID(IDKind kind = IDKind.GUID, int length=12)
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

        #region Encryption
        public static byte[] Encrypt<T>(byte[] input, byte[] key) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = _vector.GetBytes<ASCIIEncoding>();
            byte[] saltBytes = _salt.GetBytes<ASCIIEncoding>();

            byte[] encrypted;
            using (T cipher = new T())
            {
                var pdrEncrypt = new PasswordDeriveBytes(key, saltBytes, _hash, _iterations);
                byte[] keyBytes = pdrEncrypt.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;
                cipher.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encrypter = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (var to = new MemoryStream())
                    {
                        using (var writer = new CryptoStream(to, encrypter, CryptoStreamMode.Write))
                        {
                            writer.Write(input, 0, input.Length);
                            writer.FlushFinalBlock();
                        }
                        encrypted = to.ToArray();
                    }
                }
                cipher.Clear();
            }
            return encrypted;
        }

        public static byte[] Decrypt<T>(byte[] input, byte[] key) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = _vector.GetBytes<ASCIIEncoding>();
            byte[] saltBytes = _salt.GetBytes<ASCIIEncoding>();

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (T cipher = new T())
            {
                var pdrDecrypt = new PasswordDeriveBytes(key, saltBytes, _hash, _iterations);
                byte[] keyBytes = pdrDecrypt.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;
                cipher.Padding = PaddingMode.PKCS7;
                try
                {
                    using (var decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (var from = new MemoryStream(input))
                        {
                            using (var csDecrypt = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                var dec = new byte[input.Length];
                                decryptedByteCount = csDecrypt.Read(dec, 0, dec.Length);
                                decrypted = new byte[decryptedByteCount];

                                for (int i = 0; i < decrypted.Length; i++)
                                {
                                    decrypted[i] = dec[i];
                                }
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }

                cipher.Clear();
            }

            return decrypted;
        }

        public static string AESEncrypt(string input, string key)
        {
            input = input.ToBase64();
            key = key.ToBase64();

            var d = input.ToBytesBase64();
            var k = key.ToBytesBase64();

            var result = Convert.ToBase64String(AESEncrypt(d, k));
            //result = result.ToUTF8();

            return result;

        }

        public static string AESDecrypt(string input, string key)
        {
            //input = input.ToBase64();
            key = key.ToBase64();

            var d = input.ToBytesBase64();
            var k = key.ToBytesBase64();

            var result = Convert.ToBase64String(AESDecrypt(d, k));
            result = result.ToUTF8();

            return result;
        }

        public static byte[] AESEncrypt(byte[] data, byte[] key)
        {

            return Encrypt<AesManaged>(data, key);

        }

        public static byte[] AESDecrypt(byte[] data, byte[] key)
        {
            return Decrypt<AesManaged>(data, key);

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

            throw new ArgumentException("The encoding provided is invalid");
        }

        public static string GetString<T>(this byte[] ba) where T : Encoding
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

            throw new ArgumentException("The encoding provided is invalid");
        }

        /// <summary>
        /// Reads the contents of the stream into a byte array.
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>A byte array containing the contents of the stream.</returns>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        public static byte[] ReadAllBytes(this Stream source)
        {
            long originalPosition = source.Position;
            source.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];
                int totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = source.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = source.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                source.Position = originalPosition;
            }
        }

        #endregion
    }

    public enum IDKind {
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
