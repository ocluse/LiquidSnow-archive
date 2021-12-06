using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Horus.Symmetrics
{
    /// <summary>
    /// Contains utility methods that symplify <see cref="SymmetricAlgorithm"/> cryptographic operations
    /// </summary>
    public interface ISymmetric
    {
        /// <summary>
        /// The algorithm used in the cryptographic operation
        /// </summary>
        EncryptionAlgorithm Algorithm { get; set; }
        
        /// <summary>
        /// The block size of the cryptographic opertaion
        /// </summary>
        int BlockSize { get; set; }
        
        /// <summary>
        /// The block cipher mode used for the cryptographic operation
        /// </summary>
        CipherMode CipherMode { get; set; }
        
        /// <summary>
        /// The iterations the deriver will run when generating the  Key and IV
        /// </summary>
        int Iterations { get; set; }
        
        /// <summary>
        /// The size of the key used for the cryptographic operation
        /// </summary>
        int KeySize { get; set; }
        
        /// <summary>
        /// The padding mode applied during the cryptographic operation
        /// </summary>
        PaddingMode PaddingMode { get; set; }
        
        /// <summary>
        /// The value used to salt the Key and IV derivation
        /// </summary>
        string Salt { get; set; }

        /// <summary>
        /// Decrypts the contents of the <paramref name="input"/> and writes them to the <paramref name="output"/>
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> containing the encrypted data</param>
        /// <param name="output">The destination <see cref="Stream"/> where the decrypted data will be written</param>
        /// <param name="password">The password used to decrypt the data</param>
        /// <param name="progress">The subscriber notified of the operation progress</param>
        /// <param name="cancellationToken">The token used to monitor for cancellation requests</param>
        Task DecryptAsync(Stream input, Stream output, byte[] password, IProgress<double> progress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Encrypts the contents of the <paramref name="input"/> and writes them to the <paramref name="output"/>
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> containing the data to be encrypted</param>
        /// <param name="output">The destination <see cref="Stream"/> where the encrypted data will be written</param>
        /// <param name="password">The password used to encrypt the data</param>
        /// <param name="progress">The subscriber notifed of the operation progress</param>
        /// <param name="cancellationToken">The token used to monitor for cancellation requests</param>
        /// <returns></returns>
        Task EncryptAsync(Stream input, Stream output, byte[] password, IProgress<double> progress = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Run the cryptographic operation synchronously
        /// </summary>
        /// <param name="input">The source of data to be operated on</param>
        /// <param name="output">The destination <see cref="Stream"/> where the result of the operation will be written</param>
        /// <param name="password">The password used for the cryptographic operation</param>
        /// <param name="forward">If true, runs the algorithm in encryption mode, if false, runs the algorithm in decryption mode</param>
        void Run(Stream input, Stream output, byte[] password, bool forward);

        /// <summary>
        /// Runs the cryptographic operation asynchronously
        /// </summary>
        /// <param name="input">The source of data to be operated on</param>
        /// <param name="output">The destination <see cref="Stream"/> where the result of the operation will be written</param>
        /// <param name="password">The password used for the cryptographic operation</param>
        /// <param name="forward">If true, runs the algorithm in encryption mode, if false, runs the algorithm in decryption mode</param>
        /// <param name="progress">The subscriber notifed of the operation progress</param>
        /// <param name="cancellationToken">The token used to monitor for cancellation requests</param>
        Task RunAsync(Stream input, Stream output, byte[] password, bool forward, IProgress<double> progress = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The symmetric cryptographic algorithm used by <see cref="ISymmetric"/>
    /// </summary>
    public enum EncryptionAlgorithm
    {
        /// <summary>
        /// The Advanced Encryption Standard algorithm
        /// </summary>
        Aes,

        /// <summary>
        /// The Rijndael algorithm
        /// </summary>
        Rijndael
    }
}