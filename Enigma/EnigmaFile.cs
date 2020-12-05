using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Xml.Serialization;

namespace Thismaker.Enigma
{

    /// <summary>
    /// This class defines a file that is encrypted and decrypted by use of Thismaker Engima processes
    /// </summary>
    public class EnigmaFile
    {
        #region Props and Fields
        public string Path { get; set; }

        /// <summary>
        /// Returns the hash of the data contained. Should only be immediately data is set
        /// </summary>
        public string Hash { get; private set; }
        private byte[] Key;
        private byte[] Data;
        #endregion

        #region Initialization

        /// <summary>
        /// Creates an empty enigma file object with the specified key. 
        /// If you create a file this way, you must set the path before saving to disk. 
        /// You can still, however save to stream.
        /// </summary>
        /// <param name="key">The key used to unlock and lock the object</param>
        public EnigmaFile(string key)
        {
            SetKey(key);
        }

        /// <summary>
        /// Creates a new EnigmaFile and reads its contents from the specified path.
        /// </summary>
        /// <param name="path">The file path to the location where the EnigmaFile is saved/to be saved.
        /// This is not the path where the unencrypted data resides, to set the unencrypted data,
        /// create the EnigmaFile and use EnigmaFile.SetData(string) to specify the location 
        /// from where the file is to derive its data</param>
        /// <param name="key">The key to be used in encrypting/decrypting the EnigmaFile</param>
        public EnigmaFile(string path, string key)
        {
            Path = path;
            SetKey(key);
            Load();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the key for encrypting and decrypting the EnigmaFile
        /// </summary>
        /// <param name="key">The UTF8 string to be used as the key</param>
        public void SetKey(string key)
        {
            Key = key.GetBytes<UTF8Encoding>();
        }

        /// <summary>
        /// Sets the key for encrypting and decrypting the EnigmaFile
        /// </summary>
        /// <param name="key">A byte array that will be used in encryption/decryption</param>
        public void SetKey(byte[] key)
        {
            Key = key;
        }

        /// <summary>
        /// Used to return the .NET object whose data is stored inside the file.
        /// Must only be used if the data was set by calling EnigmaFile.Serialize<T>();
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="obj">The class object whose properties will be saved to</param>
        public void Deserialize<T>(out T obj)
        {
            //Create serializer and stream
            var xmlsData = new XmlSerializer(typeof(T));
            using var msData = new MemoryStream();
            //Decrypt the data to the stream
            Decrypt(msData);
            msData.Position = 0;

            //Deserialize
            obj = (T)xmlsData.Deserialize(msData);

        }

        /// <summary>
        /// Serializes the properties of a class into the file and saves(unless specified)
        /// the data on disk.
        /// </summary>
        /// <typeparam name="T">The type of the class</typeparam>
        /// <param name="obj">The object whose properties are to be serialized</param>
        /// <param name="save">If true, the data is written to disk, else it remains in memory</param>
        public void Serialize<T>(T obj, bool save=true)
        {
            //Create serializer
            var xmlsData = new XmlSerializer(typeof(T));
            
            //Create mem stream and serialize and set data
            using(var msData=new MemoryStream())
            {
                xmlsData.Serialize(msData, obj);

                var decrypted = msData.ToArray();
                SetData(decrypted);
            }

            //Save if necessary
            if (save && !string.IsNullOrEmpty(Path))
            {
                Save();
            }
        }
        
        /// <summary>
        /// Used to set data of the file from disk. Use this to load unencrypted data that you wish to subsequently encrypt
        /// </summary>
        /// <param name="srcPath"></param>
        public void SetData(string srcPath)
        {
            //Read the file
            var data = File.ReadAllBytes(srcPath);

            //Save the data:
            SetData(data);
        }

        /// <summary>
        /// Sets the unencrypted data of the file from the specified stream.
        /// </summary>
        /// <param name="srcStream"></param>
        public void SetData(Stream srcStream)
        {
            //Read the contents of the stream to memory:
            using var msData = new MemoryStream();
            srcStream.CopyTo(msData);
            
            //Save the data:
            SetData(msData.ToArray());
        }

        /// <summary>
        /// Sets the unencrypted data of the file from the specified byte array.
        /// </summary>
        /// <param name="data">The byte array containing data to be saved to the file</param>
        public void SetData(byte[] data)
        {
            //Hash the data:
            Hash = Enigma.GetHash(data).GetString<UTF8Encoding>();
            //Encrypt the data:
            Data = Enigma.AESEncrypt(data, Key);
        }

        /// <summary>
        /// Loads encrypted data from the disk into the file
        /// You don't have to call this method if the File was created by specifying the path
        /// </summary>
        public void Load()
        {
            var data = File.ReadAllBytes(Path);
            Load(data);
        }

        /// <summary>
        /// Loads encrypted data from the specified byte array. Use Load(Stream) for security instead
        /// </summary>
        /// <param name="data"></param>
        public void Load(byte[] data)
        {
            Data = new byte[data.Length];
            data.CopyTo(Data, 0);
        }

        /// <summary>
        /// Loads the encrypted data from the specified stream.
        /// </summary>
        /// <param name="srcStream"></param>
        public void Load(Stream srcStream)
        {
            var data = srcStream.ReadAllBytes();
            Load(data);
        }

        /// <summary>
        /// Saves the encrypted data to the filepath specified in the Path property of the EnigmaFile
        /// </summary>
        public void Save()
        {
            File.WriteAllBytes(Path,Data);
        }

        /// <summary>
        /// Saves the encrypted contents of the EnigmaFile to the specified stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.SetLength(0);
                stream.Position = 0;
            }
            stream.Write(Data, 0, Data.Length);
        }

        /// <summary>
        /// Saves the decrypted contents of the EnigmaFile to the specified path
        /// </summary>
        /// <param name="path"></param>
        public void Decrypt(string path)
        {
            var decrypted = Enigma.AESDecrypt(Data, Key);
            File.WriteAllBytes(path, decrypted);
        }

        /// <summary>
        /// Saves the decrypted contents of the EnigmaFile to the specified stream
        /// </summary>
        /// <param name="stream">The stream to save the data to</param>
        public void Decrypt(Stream stream)
        {
            var decrypted = Enigma.AESDecrypt(Data, Key);
            stream.SetLength(0);
            stream.Position = 0;
            stream.Write(decrypted, 0, decrypted.Length);
        }

        /// <summary>
        /// Checks the integrity of the data in the EnigmaFile 
        /// by comparing UTF8 String hashes of the integral and the data
        /// </summary>
        /// <param name="integral"></param>
        /// <returns></returns>
        public bool CheckIntegrity(string integral)
        {
            return Hash == integral;
        }

        #endregion

    }

    public enum EncryptionAlgorithm
    {
        AES
    }

   
}
