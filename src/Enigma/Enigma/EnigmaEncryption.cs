using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Thismaker.Enigma
{
    public static partial class Enigma
    {
        #region Settings
        private static int _iterations = 2;
        private static int _keySize = 256;
        private static int _blockSize = 128;
        private static string _salt = "aselrias38490a32"; // Random

        #endregion

        /// <summary>
        /// Configures the properties that will be used in encryption and decryption.
        /// If this method is not invoked, default properties will be used, 
        /// it is advisable to run this method to create unique encryption results.
        /// The method should only be run once during the lifetime of <see cref="Enigma"/>
        /// </summary>
        /// <param name="config"></param>
        public static void ConfigureCrypter(EnigmaConfiguration config)
        {
            _iterations = config.DerivedIterations;
            _keySize = config.KeySize;
            _blockSize = config.BlockSize;
            _salt = config.Salt;
        }

        /// <summary>
        /// Returns the current configuration of Enigma, which can then be stored to use in a different
        /// session. Remember, it is important to store this information securely to prevent possible attacks
        /// </summary>
        /// <returns></returns>
        public static EnigmaConfiguration ExportConfiguration()
        {
            return new EnigmaConfiguration
            {
                DerivedIterations = _iterations,
                KeySize = _keySize,
                BlockSize = _blockSize,
                Salt = _salt
            };
        }

        /// <summary>
        /// Encrypts byte data using the specified algorithm and configuration
        /// </summary>
        /// <typeparam name="TAlg">A <see cref="SymmetricAlgorithm"/> that will be used in the Encryption Process</typeparam>
        /// <param name="input">The data to be encrypted</param>
        /// <param name="password">The password that will be used to generate the key based on the Enigma Configuration</param>
        /// <returns></returns>
        public static byte[] Encrypt<TAlg>(byte[] input, byte[] password) where TAlg : SymmetricAlgorithm, new()
        {
            byte[] encrypted;
            using (TAlg cipher = new TAlg())
            {
                byte[] saltBytes = _salt.GetBytes<UTF8Encoding>();

                var rdbEncrypt = new Rfc2898DeriveBytes(password, saltBytes, _iterations);
                byte[] key = rdbEncrypt.GetBytes(_keySize / 8);
                byte[] iv = rdbEncrypt.GetBytes(_blockSize / 8);


                cipher.Mode = CipherMode.CBC;
                cipher.Padding = PaddingMode.PKCS7;
                cipher.Key = key;
                cipher.IV = iv;

                ICryptoTransform encrypter = cipher.CreateEncryptor();

                using var msEncrypter = new MemoryStream();
                using (var msEncrypt = new MemoryStream(input))
                {
                    using var csEncrypt = new CryptoStream(msEncrypt, encrypter, CryptoStreamMode.Read);
                    using StreamWriter streamOut = new StreamWriter(msEncrypter);
                    csEncrypt.CopyTo(streamOut.BaseStream);
                }
                encrypted = msEncrypter.ToArray();
            }
            return encrypted;

        }

        /// <summary>
        /// Decrypts byte data using the specified algorithm
        /// </summary>
        /// <typeparam name="TAlg">A <see cref="SymmetricAlgorithm"/> that will be used in the Decryption Process</typeparam>
        /// <param name="input">The data to be decrypted</param>
        /// <param name="password">The password that will be used to generate the key based on the Enigma Configuration</param>
        /// <returns></returns>
        public static byte[] Decrypt<TAlg>(byte[] input, byte[] password) where TAlg : SymmetricAlgorithm, new()
        {
            byte[] saltBytes = _salt.GetBytes<UTF8Encoding>();

            

            using TAlg cipher = new TAlg();
            var rdbDecrypt = new Rfc2898DeriveBytes(password, saltBytes, _iterations);
            byte[] key = rdbDecrypt.GetBytes(_keySize / 8);
            byte[] iv = rdbDecrypt.GetBytes(_blockSize / 8);

            cipher.Mode = CipherMode.CBC;
            cipher.Padding = PaddingMode.PKCS7;
            cipher.Key = key;
            cipher.IV = iv;

            ICryptoTransform decrypter = cipher.CreateDecryptor();

            byte[] decrypted;
            using (var msOutput=new MemoryStream())
            {
                using(var msInput=new MemoryStream(input))
                {
                    using var srInput = new StreamReader(msInput);
                    using var csInput = new CryptoStream(srInput.BaseStream, decrypter, CryptoStreamMode.Read);
                    using var swOutput = new StreamWriter(msOutput);
                    csInput.CopyTo(swOutput.BaseStream);
                }
                decrypted = msOutput.ToArray();
                
            }
            return decrypted;
        }

        /// <summary>
        /// Runs the <see cref="Aes"/> encryption on the byte data
        /// </summary>
        /// <param name="data">The data to be encrypted</param>
        /// <param name="key">The key to be used</param>
        /// <returns></returns>
        public static byte[] AESEncrypt(byte[] data, byte[] key)
        {
            return Encrypt<AesManaged>(data, key);
        }

        /// <summary>
        /// Runs the <see cref="Aes"/> decryption on the byte data
        /// </summary>
        /// <param name="data">The data to be decrypted</param>
        /// <param name="key">The key to be used</param>
        /// <returns></returns>
        public static byte[] AESDecrypt(byte[] data, byte[] key)
        {
            return Decrypt<AesManaged>(data, key);
        }

        /// <summary>
        /// Runs an <see cref="Aes"/> encryption on the input string. Note that due to the sensitive nature
        /// of the obtained encryption, the resultant string will be encoded into Base64 and not the specified
        /// <typeparamref name="TEnc"/>. In general, the use of this method should be limited. 
        /// Use <see cref="AESEncrypt(byte[], byte[])"/>
        /// instead and handle the <see cref="byte"/> data appropriately.
        /// </summary>
        /// <typeparam name="TEnc">The <see cref="Encoding"/> that will be used to derive bytes from the string.
        /// Note that some encoding types may cause loss of data, so care must be taken</typeparam>
        /// <param name="input">The string to be encoded</param>
        /// <param name="key">The key to be used as password</param>
        /// <returns></returns>
        public static string AESEncrypt<TEnc>(string input, string key) where TEnc : Encoding
        {
            var d = input.GetBytes<TEnc>();
            var k = key.GetBytes<TEnc>();
            
            var result = AESEncrypt(d, k);

            return Convert.ToBase64String(result);

        }

        /// <summary>
        /// Runs an <see cref="Aes"/> decryption on the input string. Note that due to the sensitive nature of required
        /// data, the input string is first decoded to <see cref="byte[]"/> using Base64, therefore it must be a Base64 string.
        /// An exception will be thrown if the string is not a Base64. 
        /// The key on the other hand is decoded using <typeparamref name="TEnc"/>.
        /// In general, the use of this method should be limited. 
        /// Use <see cref="AESDecrypt(byte[], byte[])"/>
        /// instead and handle the <see cref="byte"/> data appropriately.
        /// </summary>
        /// <typeparam name="TEnc">The <see cref="Encoding"/> 
        /// that will be used to decode the resultant <see cref="byte[]"/> after decryption, <b>Not Before</b>
        /// decryption
        /// Note that some encoding types may cause loss of data, so care must be taken</typeparam>
        /// <param name="input">The string to be decoded</param>
        /// <param name="key">The key to be used as password</param>
        public static string AESDecrypt<TEnc>(string input, string key) where TEnc : Encoding
        {
            var d = Convert.FromBase64String(input);
            var k = key.GetBytes<TEnc>();

            var result = AESDecrypt(d, k);

            return result.GetString<TEnc>();
        }
    }

    public class EnigmaConfiguration
    {
        public int DerivedIterations { get; set; } = 1000;
        public string Salt { get; set; } = "asdfaiunasdva&92374913dsfk0483qt";
        public int KeySize { get; set; } = 256;
        public int BlockSize { get; set; } = 128;
    }
}
