using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Thismaker.Horus.Symmetrics;

namespace Thismaker.Horus.IO
{

    /// <summary>
    /// A file that, in it's natural state, holds it's data in an encrypted format.
    /// </summary>
    public class CryptoFile : IDisposable
    {
        #region Private Fields
        private byte[] _key;
        
        private readonly Stream _stream;
        #endregion

        #region Initialization

        /// <summary>
        /// Creates a <see cref="CryptoFile"/> with the specified key, 
        /// and the underlying stream being a new <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="key">The key used to unlock and lock the object</param>
        public CryptoFile(string key)
        {
            _stream = new MemoryStream();
            SetKey(key);
        }

        /// <summary>
        /// Creates a <see cref="CryptoFile"/> with the specified key 
        /// in the provided <paramref name="stream"/>
        /// </summary>
        /// <param name="stream">The underlying stream for the <see cref="CryptoFile"/></param>
        /// <param name="key">The key used in encrypting/decrypting the <see cref="CryptoFile"/></param>
        public CryptoFile(Stream stream, string key)
        {
            _stream = stream;
            SetKey(key);
        }

        /// <summary>
        /// Creates a <see cref="CryptoFile"/> with the specified key,
        /// opening a <see cref="FileStream"/> at the specified path.
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="key">The key used in decrypting/encrypting</param>
        public CryptoFile(string path, string key)
        {
            _stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            SetKey(key);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The algorithm to be used by the procedure, failure to define one means the file resorts to
        /// <see cref="PredefinedSymmetric.AesFixed"/>
        /// </summary>
        public Symmetric Algorithm { get; set; }
        = PredefinedSymmetric.AesFixed;
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the key for encrypting and decrypting the <see cref="CryptoFile"/>
        /// </summary>
        /// <param name="key">The UTF8 string to be used as the key</param>
        public void SetKey(string key)
        {
            _key = key.GetBytes<UTF8Encoding>();
        }

        /// <summary>
        /// Sets the key for encrypting and decrypting the <see cref="CryptoFile"/>
        /// </summary>
        /// <param name="key">A byte array that will be used in encryption/decryption</param>
        public void SetKey(byte[] key)
        {
            Array.Clear(_key, 0, _key.Length);
            Array.Copy(key,0, _key,0, key.Length);
        }

        /// <summary>
        /// Deserializes the <see cref="CryptoFile"/> to the sepcified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the <see cref="CryptoFile"/> contents into</typeparam>
        /// <returns></returns>
        public async Task<T> DeserializeAsync<T>()
        {
            //Create serializer and stream
            var serializer = new XmlSerializer(typeof(T));
            using var msData = new MemoryStream();

            //Decrypt the data to the stream
            await ReadAsync(msData);
            msData.Position = 0;

            //Deserialize
            return (T)serializer.Deserialize(msData);
        }

        /// <summary>
        /// Uses XML serialization to save the contents of the provided <paramref name="o"/>
        /// to the <see cref="CryptoFile"/>'s underlying stream.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        /// <returns></returns>
        public async Task SerializeAsync(object o)
        {
            var serializer = new XmlSerializer(o.GetType());
            using var msData = new MemoryStream();
            serializer.Serialize(msData, o);
            msData.Position = 0;
            _stream.Position = 0;
            _stream.SetLength(0);
            await WriteAsync(msData);
        }

        /// <summary>
        /// Writes all the data in the <paramref name="buffer"/> to the underlying stream, encrypting it of course.
        /// </summary>
        /// <param name="buffer">The array of bytes to write</param>
        /// <param name="progress">The overall progress of the process</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task WriteAsync(byte[] buffer, IProgress<float> progress=null, CancellationToken cancellationToken=default)
        {
            using var msData = new MemoryStream(buffer);
            await WriteAsync(msData, progress, cancellationToken);

        }

        /// <summary>
        /// Writes all the data from a provided <paramref name="stream"/> to the <see cref="CryptoFile"/>
        /// </summary>
        /// <param name="stream">The source stream, where the plaintext resides</param>
        /// <param name="progress">If provided, returns a progress of the overall process</param>
        /// <param name="cancellationToken">A way to cancel the task</param>
        /// <returns></returns>
        public async Task WriteAsync(Stream stream, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            await Algorithm.EncryptAsync(input: stream, output: _stream, _key, progress, cancellationToken);
        }

        /// <summary>
        /// Saves the decrypted contents of the <see cref="CryptoFile"/> to the specified <paramref name="stream"/>
        /// </summary>
        /// <param name="stream">The stream to save the data to</param>
        public async Task ReadAsync(Stream stream, IProgress<float>progress=null, CancellationToken cancellationToken=default)
        {
            _stream.Position = 0;
            await Algorithm.DecryptAsync(input: _stream, output: stream, _key, progress, cancellationToken);
        }

        /// <summary>
        /// Closes and releases the underlying stream
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion

    }
}
