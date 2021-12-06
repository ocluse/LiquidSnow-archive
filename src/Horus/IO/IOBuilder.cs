using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Horus.IO
{
    /// <summary>
    /// Contains utility methods for creating instances of <see cref="ICryptoFile"/> and <see cref="ICryptoContainer"/>
    /// </summary>
    public class IOBuilder
    {
        /// <summary>
        /// Create an instance of a <see cref="ICryptoFile"/> with the underlying <see cref="Stream"/> being a new <see cref="MemoryStream"/>
        /// </summary>
        /// <param name="key">The key used to encrypt/decrypt the file's contents</param>
        /// <remarks>
        /// Because the underlying stream is created in memory, it is important to dispose the instance once it is no longer needed to prevent memory leaks.
        /// Also, it is important to remember that any data written into the file will be lost upon exiting the environment or upon disposal of the instance.
        /// </remarks>
        /// <returns>An <see cref="ICryptoFile"/> with a <see cref="MemoryStream"/> as the underlying <see cref="Stream"/></returns>
        public static ICryptoFile CreateFile(string key)
        {
            return new CryptoFile(key);
        }

        /// <summary>
        /// Creates an instance of a <see cref="ICryptoFile"/>
        /// </summary>
        /// <param name="key">The key used to encrypt/decrypt the contents of the file</param>
        /// <param name="stream">The underlying <see cref="Stream"/> of the file</param>
        /// <returns>An <see cref="ICryptoFile"/> that writes to and reads from the provided <paramref name="stream"/></returns>
        /// <remarks>
        /// While this method is named "Create" it does not overwrite any existing data in the <paramref name="stream"/>.
        /// Instead, the <paramref name="stream"/> will be opened ready to be read from.
        /// </remarks>
        public static ICryptoFile CreateFile(string key, Stream stream)
        {
            return new CryptoFile(key, stream);
        }

        /// <summary>
        /// Creates an instance of a <see cref="ICryptoFile"/> 
        /// </summary>
        /// <param name="key">The key used to encrypt/decrypt the contents of the file</param>
        /// <param name="path">The path of the file</param>
        /// <returns>
        /// An <see cref="ICryptoFile"/> that writes to and reads from the provided file <paramref name="path"/>
        /// </returns>
        /// <remarks>
        /// If no file exists at the specified path, a new one will be created, otherwise, the existing file will be opened
        /// </remarks>
        public static ICryptoFile CreateFile(string key, string path)
        {
            return new CryptoFile(key, path);
        }

        /// <summary>
        /// Creates an instance of <see cref="ICryptoContainer"/> with the underlying stream being a new <see cref="MemoryStream"/>
        /// </summary>
        /// <param name="key">The key used to encrypt/decrypt the items in the container</param>
        /// <remarks>
        /// Because the underlying stream is created in memory, it is important to dispose the instance once it is no longer needed to prevent memory leaks.
        /// Also, it is important to remember that any data written into the container will be lost upon exiting the environment or upon disposal of the instance.
        /// </remarks>
        /// <returns>An <see cref="ICryptoContainer"/> with a <see cref="MemoryStream"/> as the underlying <see cref="Stream"/></returns>
        public static ICryptoContainer CreateContainer(string key)
        {
            return new CryptoContainer(key);
        }

        /// <summary>
        /// Creates an instance of a <see cref="ICryptoContainer"/>
        /// </summary>
        /// <param name="key">The key used to encrypt/decrypt the items in the container</param>
        /// <param name="stream">The underlying <see cref="Stream"/> of the container</param>
        /// <returns>A <see cref="ICryptoContainer"/> that reads to and writes from theh provided <paramref name="stream"/></returns>
        /// <remarks>
        /// While this method is named "Create" it does not overwrite any existing data in the <paramref name="stream"/>.
        /// Instead, the <paramref name="stream"/> will be opened ready to be read from.
        /// </remarks>
        public static ICryptoContainer CreateContainer(string key, Stream stream)
        {
            return new CryptoContainer(stream, key);
        }
    }
}
