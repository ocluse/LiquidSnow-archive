using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Horus.Symmetrics
{
    /// <summary>
    /// A class to help deal with SymmetricAlgorithm encryption,
    /// by implementing it and simplifying the concepts
    /// </summary>
    public class Symmetric
    {
        /// <summary>
        /// The Algorithm to use in the process
        /// </summary>
        public EncryptionAlgorithm Algorithm { get; set; }
        
        /// <summary>
        /// The salt added in deriving the key bytes and IV bytes
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// The <see cref="Salt"/> represented as bytes.
        /// </summary>
        public byte[] SaltBytes
        {
            get=>Salt.GetBytes<UTF8Encoding>();
        }

        /// <summary>
        /// The size of the algorithm's key. Usually 256
        /// </summary>
        public int KeySize { get; set; }

        /// <summary>
        /// The number of itereations that the key deriver will run while generating
        /// the key and the IV.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// The size of each block. e.g 128
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// Runs the algorithm in forward mode.
        /// </summary>
        /// <param name="input">The stream with the data to encrypt</param>
        /// <param name="output">The stream where the encrypted data will be saved</param>
        /// <param name="password">The password to use in generating the key and IV</param>
        /// <param name="progress">If assigned, will report on progress of the process, 
        /// if <paramref name="input"/> is seekable</param>
        /// <param name="cancellationToken">If provided, can be used to cancel the task</param>
        /// <returns></returns>
        public async Task EncryptAsync(Stream input, Stream output, byte[] password, IProgress<float> progress=null, CancellationToken cancellationToken = default)
        {
            try
            {
                await RunAsync(input, output, password, true, progress, cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Runs the algorithm in reverse mode.
        /// </summary>
        /// <param name="input">The stream with the data to decrypt</param>
        /// <param name="output">The stream where the decrypted data will be saved</param>
        /// <param name="password">The password to use in generating the key and IV</param>
        /// <param name="progress">If assigned, will report on progress of the process, 
        /// if <paramref name="input"/> is seekable</param>
        /// <param name="cancellationToken">If provided, can be used to cancel the task</param>
        /// <returns></returns>
        public async Task DecryptAsync(Stream input, Stream output, byte[] password, IProgress<float> progress=null, CancellationToken cancellationToken = default)
        {
            try
            {
                await RunAsync(input, output, password, false, progress, cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Runs the algorithm, synchronously.
        /// </summary>
        /// <param name="input">The stream with data the algorithm will act on</param>
        /// <param name="output">The stream where the ouput of the algorithm will be saved</param>
        /// <param name="password">Used in generating the key and IV</param>
        /// <param name="forward">If true, similar to encrypt, otherwise decrypt</param>
        public void Run(Stream input, Stream output, byte[] password, bool forward)
        {
            using var alg = FromEncryptionAlgorithm(Algorithm);

            var rdb = new Rfc2898DeriveBytes(password, SaltBytes, Iterations);
            var key = rdb.GetBytes(KeySize / 8);
            var iv = rdb.GetBytes(BlockSize / 8);

            alg.Mode = CipherMode.CBC;
            alg.Padding = PaddingMode.PKCS7;
            alg.Key = key;
            alg.IV = iv;

            var trans = forward ? alg.CreateEncryptor() : alg.CreateDecryptor();
            using var csInput = new CryptoStream(input, trans, CryptoStreamMode.Read);
            while (true)
            {
                var buffer = new byte[8];
                int read=csInput.Read(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    break;
                }
                output.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Runs the aglorithm asynchronously. Recommended for large files
        /// </summary>
        /// <param name="input">The stream with data the algorithm will act on</param>
        /// <param name="output">The stream where the ouput of the algorithm will be saved</param>
        /// <param name="password">Used in generating the key and IV</param>
        /// <param name="forward">If true, similar to encrypt, otherwise decrypt</param>
        /// <param name="progress">If provided, will report on the progress of the algorithm</param>
        /// <param name="cancellationToken">If provided, can be used to cancel the operation</param>
        /// <returns></returns>
        public Task RunAsync(Stream input, Stream output, byte[] password, bool forward, IProgress<float> progress = null, CancellationToken cancellationToken=default)
        {
            using var alg = FromEncryptionAlgorithm(Algorithm);
            var rdb = new Rfc2898DeriveBytes(password, SaltBytes, Iterations);
            var key = rdb.GetBytes(KeySize / 8);
            var iv = rdb.GetBytes(BlockSize / 8);

            alg.Mode = CipherMode.CBC;
            alg.Padding = PaddingMode.PKCS7;
            alg.Key = key;
            alg.IV = iv;

            long workedOn = 0;
            var trans = forward ? alg.CreateEncryptor() : alg.CreateDecryptor();
            using var csInput = new CryptoStream(input, trans, CryptoStreamMode.Read);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var buffer = new byte[8];
                int read = csInput.Read(buffer, 0, buffer.Length);
                
                output.Write(buffer, 0, read);

                workedOn += read;
                if(progress!=null && input.CanSeek)
                {
                    progress.Report(workedOn /(float) input.Length);
                }

                if (read == 0)
                {
                    break;
                }
            }

            return Task.CompletedTask;
        }

        private static SymmetricAlgorithm FromEncryptionAlgorithm(EncryptionAlgorithm algorithm)
        {
            return algorithm switch
            {
                EncryptionAlgorithm.AES => new AesManaged(),
                EncryptionAlgorithm.Rijndael => new RijndaelManaged(),
                _ => throw new InvalidOperationException("Algrothm unknown or unsupported")
            };
        }
    }

    public enum EncryptionAlgorithm
    {
        AES, Rijndael
    }

}
