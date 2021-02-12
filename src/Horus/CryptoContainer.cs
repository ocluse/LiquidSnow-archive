using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Xml.Serialization;
using Thismaker.Core.Utilities;
using System;
using System.Threading.Tasks;

namespace Thismaker.Horus
{
    /// <summary>
    /// An Enigma Container is a self-managing collection for storing, accessing and updating files. 
    /// Files within are encrypted using a common password
    /// </summary>
    public sealed class CryptoContainer : IDisposable
    {
        #region Properties

        public List<ContainerObject> Files { get; set; } = new List<ContainerObject>();

        /// <summary>
        /// The path is the path(including the file name) to the Enigma Container file. 
        /// Set during initialization
        /// </summary>
        public string Path { get; private set; }

        //Password set only once!
        private readonly string Password;

        //private ZipArchive zaContainer;
        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new EnigmaContainer object to interact with an existing or create a container file
        /// </summary>
        /// <param name="path">The path of the EnigmaContainer on disk</param>
        /// <param name="password">The password used to encrypt the contents of the Enigma Container</param>
        /// <param name="access">The access mode, if Open, an exception is throw if the file doesn't exist, 
        /// if Create, an exception is thrown if container exists. If CreateOrOpen, no exception thrown related to accessing</param>
        public CryptoContainer(string path, string password, ContainerAccess access)
        {
            Password = password;
            Path = path;
            Files = new List<ContainerObject>();

            bool open = access == ContainerAccess.Open || (access == ContainerAccess.CreateOrOpen && File.Exists(path));

            if (open)
            {
                ReadHeaderFile();

            }

            else
            {
                //Create the zipFile:
                WriteHeaderFile();
            }

        }
        #endregion

        #region Private Methods

        //Open a stream to write the header data information. Use when the header has been updated
        private void WriteHeaderFile()
        {
            var writer = new XmlSerializer(typeof(List<ContainerObject>));
            
            using var fsContainer = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Update);

            var entry=zaContainer.GetEntry(".header");
            if (entry == null) entry = zaContainer.CreateEntry(".header");
            writer.Serialize(entry.Open(), Files);
            zaContainer.Dispose();
            fsContainer.Close();
        }

        //Open a stream to read header information. Use should be restricted to initialization
        private void ReadHeaderFile()
        {
            using var fsContainer = new FileStream(Path, FileMode.Open, FileAccess.Read);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Read);
            var entry = zaContainer.GetEntry(".header");

            var files = new List<ContainerObject>();
            var reader = new XmlSerializer(typeof(List<ContainerObject>));

            files=(List<ContainerObject>)reader.Deserialize(entry.Open());
            Files.AddRange(files);
        }

        private async Task AddAsync(Stream stream, string name)
        {
            var obj = new ContainerObject(name);
           
            //Open zipFile:
            using var fsContainer = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Update);

            ZipArchiveEntry zaeEdit;
            //Rewrite if Exists
            if (Files.Exists(x => x.PlainName == name))
            {
                obj = Files.Find(x => x.PlainName == name);
                //rewrite:
                zaeEdit = zaContainer.GetEntry(obj.EntryName);
            }
            else
            {
                Files.Add(obj);
                zaeEdit = zaContainer.CreateEntry(obj.EntryName);
            }
            using var s=zaeEdit.Open();
            using var efAdd = new CryptoFile(s, Password);
            await efAdd.WriteAsync(stream);

            zaContainer.Dispose();
            fsContainer.Close();

            WriteHeaderFile();
        }

        private void GetStream(Stream stream, string name)
        {
            
            var efGet = new CryptoFile(Password);
            var coGet = Files.Find((x) => x.PlainName == name);

            if(coGet==null)
            {
                throw new NullReferenceException("File does not exist in the container");
            }

            var eName = coGet.EntryName;

            //Open zipFile:
            using var fsContainer = new FileStream(Path, FileMode.Open, FileAccess.Read);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Read, true);

            var zaeGet = zaContainer.GetEntry(eName);
            using var dsGet = zaeGet.Open();
            using var msGet = new MemoryStream();
            dsGet.CopyTo(msGet);
            efGet.Load(msGet);
            efGet.Decrypt(stream);
            zaContainer.Dispose();
            fsContainer.Close();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Extracts the contents of the container to the specified path
        /// </summary>
        /// <param name="path">The path where the contents of the container will be extracted to, if the file exists, it will be skipped</param>
        public void ExtractContainer(string path, bool overwrite=false)
        {
            foreach(var file in Files)
            {
                if (file.PlainName == ".header") continue;
                if (File.Exists(path))
                {
                    if (!overwrite) continue;
                    else File.Delete(path);
                }
                var extracDir = IOUtility.CombinePath(path, file.PlainName);
                Get(extracDir, file.PlainName);
            }
        }

        /// <summary>
        /// Returns the EnigmaFile representing the required object. It's recommended to use Get method instead unless you know what you're doing
        /// </summary>
        /// <param name="key">The name of the resident</param>
        /// <returns>EnigmaFile</returns>
        public CryptoFile this[string key]
        {
            get
            {
                var ef = new CryptoFile(Password);
                using var msGet = new MemoryStream();
                Get(msGet, key);
                ef.SetData(msGet);
                return ef;
            }
        }

        /// <summary>
        /// Adds an entry to the container from the specified path, where the entry name becomes the name of the file. 
        /// If the file already exists, it is overwritten.
        /// </summary>
        /// <param name="srcPath">The path of the file to be added to the container</param>
        public void Add(string srcPath)
        {
            try
            {
                var fInfo = new FileInfo(srcPath);
                using var msAdd = new MemoryStream(File.ReadAllBytes(srcPath));
                Add(msAdd, fInfo.Name);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Adds an entry to the container from the specified stream, if the file exists, it is overwritten.
        /// </summary>
        /// <param name="stream">The stream representing the data to be wirtten to the entry</param>
        /// <param name="name">The name of the entry for purposes of retrieving and extracting</param>
        public void Add(Stream stream, string name)
        {
            //Checking for valid name
            if (name == ".header") throw new ArgumentException("Invalid name for file");

            AddStream(stream, name);
        }

        /// <summary>
        /// Adds an XML serializable C# object to the container. Use to directly store and retieve class objects for easy operation.
        /// </summary>
        /// <typeparam name="T">The type of object to be serialized</typeparam>
        /// <param name="srcObj">The class object to be serialized and saved</param>
        /// <param name="name">The name of the object that will be used for retrieval purposes</param>
        public void AddObject<T>(T srcObj, string name)
        {
            using var msAdd = new MemoryStream();
            var xsAdd = new XmlSerializer(typeof(T));
            xsAdd.Serialize(msAdd, srcObj);
            msAdd.Position = 0;
            Add(msAdd, name);
 
        }

        /// <summary>
        /// Returns an object that was previously saved into the container.
        /// </summary>
        /// <typeparam name="T">The type of the object to be retrieved</typeparam>
        /// <param name="obj">The object on which the deserialized data is applied</param>
        /// <param name="name">The name used to store the object in the container</param>
        public void Get<T>(out T obj, string name)
        {
            using var msGet = new MemoryStream();
            Get(msGet, name);
            msGet.Position = 0;
            var xrGet = new XmlSerializer(typeof(T));
            obj=(T)xrGet.Deserialize(msGet);
        }

        /// <summary>
        /// Copies the contents of an entry to the specified stream
        /// </summary>
        /// <param name="stream">The stream to which the data will be copied</param>
        /// <param name="name">The name of the entry to be copied from</param>
        public void Get( Stream stream, string name)
        {
            //Checking for valid name
            if (name == ".header") throw new ArgumentException("Invalid name for file");

            GetStream(stream, name);
        }

        /// <summary>
        /// Copies the contents of an entry to the specified file path. If the file exists, it will be overwritten
        /// </summary>
        /// <param name="path">The path to the file where the data will be written</param>
        /// <param name="name">The name of the entry to be copied</param>
        public void Get(string path, string name)
        {
            using var msGet = new MemoryStream();
            Get(msGet, name);
            File.WriteAllBytes(path, msGet.ToArray());
        }

        /// <summary>
        /// Deletes a file in the container, returning true if deletion was successful, otherwise false
        /// </summary>
        /// <param name="name">The name of the file to be deleted</param>
        /// <returns></returns>
        public bool Delete(string name)
        {
            var coGet = Files.Find(x => x.PlainName == name);
            if (coGet == null) return false;
            //Open zipFile:
            using var fsContainer = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.Inheritable);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Update, true);
            var zaeGet = zaContainer.GetEntry(coGet.EntryName);
            if (coGet == null) return false;
            zaeGet.Delete();
            Files.Remove(coGet);
            zaContainer.Dispose();
            fsContainer.Close();
            WriteHeaderFile();
            return true;
        }

        /// <summary>
        /// Writes all the text in the string to a file in the container, if the file exists, it is overwritten
        /// </summary>
        /// <param name="text">The text to be written to the file</param>
        /// <param name="name">The name to give the file entry</param>
        public void WriteAllText(string text, string name)
        {
            var baText = Encoding.UTF8.GetBytes(text);
            WriteAllBytes(baText, name);
        }

        /// <summary>
        /// Returns the contents of an item in the container as UTF8 string
        /// </summary>
        /// <param name="name">The name of the item whose contents are to be read</param>
        public string ReadAllText(string name)
        {
            return Encoding.UTF8.GetString(ReadAllBytes(name));
        }

        public void WriteAllBytes(byte[] bytes, string name)
        {
            using var msFile = new MemoryStream();
            msFile.Write(bytes, 0, bytes.Length);

            msFile.Position = 0;
            Add(msFile, name);
        }

        public byte[] ReadAllBytes(string name)
        {
            using var msFile = new MemoryStream();
            Get(msFile, name);
            return msFile.ToArray();
        }

        public bool Exists(string name)
        {
            return Files.Exists(x => x.PlainName == name);
        }

        public void Dispose()
        {
            Files = null;
        }

        #endregion
    }
    /// <summary>
    /// Methods used to access an EnigmaContainer
    /// </summary>
    public enum ContainerAccess { Create, Open, CreateOrOpen};

    public class ContainerObject
    {

        public string PlainName { get; set; }
        
        public string EntryName { get; set; }
        
        public string ExtractedDirectory { get; set; }
        
        public string Hash { get; set; }

        private ContainerObject() { }

        public ContainerObject(string name)
        {
            PlainName = name;
            EntryName = Horus.GenerateID();
        }
    }

}
