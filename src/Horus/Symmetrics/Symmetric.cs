using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Horus.Symmetrics
{
    public class Symmetric
    {
        public EncryptionAlgorithm Algorithm { get; set; }

        public string Salt { get; set; }

        public byte[] SaltBytes
        {
            get=>Salt.GetBytes<UTF8Encoding>();
        }

        public int KeySize { get; set; }

        public int Iterations { get; set; }

        public int BlockSize { get; set; }

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

        public async Task DecryptAsync(Stream input, Stream output, byte[] password, IProgress<float> progress=null, CancellationToken cancellationToken = default)
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

        public async Task RunAsync(Stream input, Stream output, byte[] password, bool forward, IProgress<float> progress = null, CancellationToken cancellationToken=default)
        {
            using var alg = FromEncryptionAlgorithm(Algorithm);
            var rdb = new Rfc2898DeriveBytes(password, SaltBytes, Iterations);
            var key = rdb.GetBytes(KeySize / 8);
            var iv = rdb.GetBytes(BlockSize / 8);

            alg.Mode = CipherMode.CBC;
            alg.Padding = PaddingMode.PKCS7;
            alg.Key = key;
            alg.IV = iv;

            float workedOn = 0;
            var trans = forward ? alg.CreateEncryptor() : alg.CreateDecryptor();
            using var csInput = new CryptoStream(input, trans, CryptoStreamMode.Read);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var buffer = new byte[8];
                int read = await csInput.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    break;
                }
                await output.WriteAsync(buffer, 0, read);

                workedOn += read;
                if(progress!=null && input.CanSeek)
                {
                    progress.Report(workedOn / input.Length);
                }
            }
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
