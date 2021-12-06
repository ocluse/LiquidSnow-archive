using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Horus.IO
{
    /// <summary>
    /// Contains functionality for encrypting and decrypting a file
    /// </summary>
    public interface ICryptoFile : IDisposable
    {

        /// <summary>
        /// Decrypts and deserializes the contents of the file using the <see cref="IOSettings.Serializer"/>
        /// </summary>
        /// <returns>The resultant object from the decryption and deserialization operation</returns>
        Task<T> DeserializeAsync<T>();

        
        /// <summary>
        /// Decrypts the contents of the file and writes them to the provided <paramref name="stream"/>
        /// </summary>
        /// <param name="stream">The stream to write the decrypted data to</param>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        Task ReadAsync(Stream stream, IProgress<double> progress = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Decrypts the contents of the file and returns them as an array of bytes
        /// </summary>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns>The decrypted content of the file as bytes</returns>
        Task<byte[]> ReadBytesAsync(IProgress<double> progress = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Decrypts the contents of a file and returns them as a UTF8 encoded string
        /// </summary>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns>The decrypted content of the file as a UTF8 string</returns>
        Task<string> ReadTextAsync(IProgress<double> progress = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Serializes and encrypts an object, writing the resulting data to the file
        /// </summary>
        /// <param name="o">The object to be serialized and encrypted</param>
        Task SerializeAsync<T>(T o);
        
        /// <summary>
        /// Sets the key used for the cryptographic operations
        /// </summary>
        /// <param name="key">The new key to be used</param>
        /// <remarks>
        /// This method does not encrypt the file again using the new key.
        /// It merely sets the key to be used moving forward.
        /// </remarks>
        void SetKey(byte[] key);
        
        ///<inheritdoc cref="SetKey(byte[])"/>
        void SetKey(string key);

        /// <summary>
        /// Encrypts and writes data to the file
        /// </summary>
        /// <param name="stream">The stream to read from, the data read encrypted and written into the file</param>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <remarks>
        /// Reads from and encrypts data from <paramref name="stream"/>, writing the encrypted data to the file.
        /// The data in <paramref name="stream"/> will not be encrypted, instead, it is only read.
        /// The encrypted data is written to the underying stream of the <see cref="ICryptoFile"/>
        /// </remarks>
        Task WriteAsync(Stream stream, IProgress<double> progress = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Encrypts and writes bytes to the file
        /// </summary>
        /// <param name="buffer">The bytes to be encrypted and written into the file</param>
        /// <param name="progress">The subscriber notifed of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        Task WriteBytesAsync(byte[] buffer, IProgress<double> progress = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Encrypts and writes text to the file
        /// </summary>
        /// <param name="contents">The text to be encrypted and written into the file</param>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        Task WriteTextAsync(string contents, IProgress<double> progress = null, CancellationToken cancellationToken = default);
    }
}