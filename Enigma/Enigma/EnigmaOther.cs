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
        /// <exception cref="IOException">An I/O error occurs.</exception>
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

    
}
