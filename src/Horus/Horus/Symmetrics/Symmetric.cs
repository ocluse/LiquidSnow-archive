using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Horus.Symmetrics
{
    internal class Symmetric : ISymmetric
    {
        public EncryptionAlgorithm Algorithm { get; set; }

        public string Salt { get; set; }

        public PaddingMode PaddingMode { get; set; } = PaddingMode.PKCS7;

        public CipherMode CipherMode { get; set; } = CipherMode.CBC;

        public int KeySize { get; set; }

        public int Iterations { get; set; }

        public int BlockSize { get; set; }

        public async Task EncryptAsync(Stream input, Stream output, byte[] password, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            await RunAsync(input, output, password, true, progress, cancellationToken);
        }

        public async Task DecryptAsync(Stream input, Stream output, byte[] password, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            await RunAsync(input, output, password, false, progress, cancellationToken);
        }

        public void Run(Stream input, Stream output, byte[] password, bool forward)
        {
            using SymmetricAlgorithm alg = FromEncryptionAlgorithm(Algorithm);

            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, Salt.GetBytes<UTF8Encoding>(), Iterations);
            byte[] key = rdb.GetBytes(KeySize / 8);
            byte[] iv = rdb.GetBytes(BlockSize / 8);
            
            alg.Mode = CipherMode;
            alg.Padding = PaddingMode;
            alg.Key = key;
            alg.IV = iv;

            ICryptoTransform trans = forward ? alg.CreateEncryptor() : alg.CreateDecryptor();
            using CryptoStream csInput = new CryptoStream(input, trans, CryptoStreamMode.Read);

            //TODO: As soon as the projects targets a higher .NET Version, get rid of this dirty hack
            var prop = csInput
                .GetType()
                .GetField("_leaveOpen", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance);

            while (true)
            {
                byte[] buffer = new byte[8];
                int read = csInput.Read(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    break;
                }
                output.Write(buffer, 0, read);
            }
        }

        public Task RunAsync(Stream input, Stream output, byte[] password, bool forward, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            using SymmetricAlgorithm alg = FromEncryptionAlgorithm(Algorithm);
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, Salt.GetBytes<UTF8Encoding>(), Iterations);
            byte[] key = rdb.GetBytes(KeySize / 8);
            byte[] iv = rdb.GetBytes(BlockSize / 8);

            alg.Mode = CipherMode;
            alg.Padding = PaddingMode;
            alg.Key = key;
            alg.IV = iv;

            long workedOn = 0;
            ICryptoTransform trans = forward ? alg.CreateEncryptor() : alg.CreateDecryptor();

            using CryptoStream csInput = new CryptoStream(input, trans, CryptoStreamMode.Read);

            //TODO: As soon as the projects targets a higher .NET Version, get rid of this dirty hack
            var prop = csInput
                .GetType()
                .GetField("_leaveOpen", System.Reflection.BindingFlags.NonPublic 
                | System.Reflection.BindingFlags.Instance);

            prop.SetValue(csInput, true);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                byte[] buffer = new byte[8];
                int read = csInput.Read(buffer, 0, buffer.Length);

                output.Write(buffer, 0, read);

                workedOn += read;
                if (progress != null && input.CanSeek)
                {
                    progress.Report(workedOn / (double)input.Length);
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
                EncryptionAlgorithm.Aes => new AesManaged(),
                EncryptionAlgorithm.Rijndael => new RijndaelManaged(),
                _ => throw new InvalidOperationException("Algrothm unknown or unsupported")
            };
        }
    }
}
