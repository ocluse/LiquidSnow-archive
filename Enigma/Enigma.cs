using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

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
        private const int AesKeySize=16;

        public static string GenerateID()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
            return id.ToUpper();
        }

        public static string GenerateSeededID()
        {
            var start = GenerateID();
            var rnd = new Random();
            var rndInt=rnd.Next(0, 1000);
            start = $"{ rndInt}{ start}";
            return start;


        }

        public static string GenerateHashID()
        {
            var start = GenerateSeededID();
            return GetHashString(start);
        }

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

            if (data == null || key == null) throw new ArgumentNullException();
            byte[] encrypted;
            key = GetHash(key);
            using var aes = new AesCryptoServiceProvider { BlockSize = 128, KeySize = 256, Key = key, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
            aes.GenerateIV();
            var iv = aes.IV;
            using var encrypter = aes.CreateEncryptor(aes.Key, iv);
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encrypter, CryptoStreamMode.Write);
            using var bwEncrypt = new BinaryWriter(csEncrypt);
            msEncrypt.Write(iv, 0, iv.Length);
            bwEncrypt.Write(data);
            csEncrypt.FlushFinalBlock();
            encrypted = msEncrypt.ToArray();
            return encrypted;

        }

        public static byte[] AESDecrypt(byte[] data, byte[] key)
        {
            if (data == null || key == null) throw new ArgumentNullException();
            byte[] decrypted;
            key = GetHash(key);
            var iv = new byte[AesKeySize];
            Array.Copy(data, 0, iv, 0, iv.Length);
            
            using (var aes = new AesCryptoServiceProvider {BlockSize=128, KeySize=256, Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 })
            {
                using var msDecrypt = new MemoryStream();
                using var csDecrypt = new CryptoStream(msDecrypt, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Write);
                using var bwDecrypt = new BinaryWriter(csDecrypt);
                bwDecrypt.Write(data, iv.Length, data.Length - iv.Length);
                csDecrypt.FlushFinalBlock();
                decrypted = msDecrypt.ToArray();
            }

           
            
            return decrypted;
            
        }
    }
}
