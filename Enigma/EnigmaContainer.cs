using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Xml.Serialization;
using SysPath = System.IO.Path;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Thismaker.Enigma
{
    /// <summary>
    /// An Enigma Container is a self-managing collection for storing, accessing and updating files. 
    /// Files within are encrypted using a common password
    /// </summary>
    [DataContract]
    public class EnigmaContainer
    {
        #region Properties
        [DataMember]
        private List<ContainerObject> Files { get; set; }

        /// <summary>
        /// The path is the path(including the file name) to the Enigma Container file. 
        /// Set during initialization
        /// </summary>
        [XmlIgnore] public string Path { get; private set; }

        //Password set only once!
        private readonly string Password;
        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new EnigmaContainer object to interact with an existing or create a container file
        /// </summary>
        /// <param name="path">The path of the EnigmaContainer on disk</param>
        /// <param name="password">The password used to encrypt the contents of the Enigma Container</param>
        /// <param name="access">The access mode, if Open, an exception is throw if the file doesn't exist, 
        /// if Create, an exception is thrown if container exists. If CreateOrOpen, no exception thrown related to accessing</param>
        public EnigmaContainer(string path, string password, ContainerAccess access)
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
                using var zipToOpen = new FileStream(Path, FileMode.Create);
                using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);

                archive.Dispose();
                zipToOpen.Dispose();
                //Create the header file:
                WriteHeaderFile();

            }

        }
        #endregion

        #region Private Methods
        //Open a stream to write the header data information. Use when the header has been updated
        private void WriteHeaderFile()
        {
            using var fsContainer = new FileStream(Path, FileMode.Open);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Update);

            //Create or open the header file:
            var header = zaContainer.GetEntry(".header");
            if (header == null) header = zaContainer.CreateEntry(".header");

            //Write to the entry stream:
            using var swHeader = new StreamWriter(header.Open());
            var xmlsCreate = new XmlSerializer(typeof(EnigmaContainer));
            xmlsCreate.Serialize(swHeader, this);

        }

        //Open a stream to read header information. Use should be restricted to initialization
        private void ReadHeaderFile()
        {
            using var fsContainer = new FileStream(Path, FileMode.Open);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Read);

            //Open the header file
            var header = zaContainer.GetEntry(".header");

            //Read the header file:
            var reader = new XmlSerializer(typeof(EnigmaContainer));
            using var srHeader = new StreamReader(header.Open());
            var myContainer = reader.Deserialize(srHeader) as EnigmaContainer;

            //Apply the aquired changes:
            Files = myContainer.Files;
        }
        #endregion

        #region Public Methods

        public void ExtractContainer(string path, ContainerExtractionMode mode=ContainerExtractionMode.Complete)
        {

            using var zipToOpen = new FileStream(Path, FileMode.Open);
            using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
            foreach (var file in Files)
            {
                var entry = archive.GetEntry(file.EntryName);
                try
                {
                    var extractedDir = SysPath.Combine(path, file.PlainName);
                    entry.ExtractToFile(SysPath.Combine(extractedDir));
                    file.ExtractedDirectory = extractedDir;

                    //If mode is complete, further decrypt the EnigmaFile:
                    if (mode == ContainerExtractionMode.Complete)
                    {
                        var destination = SysPath.Combine(extractedDir, file.PlainName);
                        var efExtract = new EnigmaFile(extractedDir, Password);
                        efExtract.Decrypt(destination);
                    }
                }
                catch { throw; }

            }
        }

        /// <summary>
        /// Returns the EnigmaFile representing the required object. It's recommended to use Get method instead unless you know what you're doing
        /// </summary>
        /// <param name="key">The name of the resident</param>
        /// <returns>EnigmaFile</returns>
        public EnigmaFile this[string key]
        {
            get
            {
                var ef = new EnigmaFile(Password);
                return ef;
            }
        }

        /// <summary>
        /// Adds an entry to the container from the specified path, where the entry name becomes the name of the file
        /// </summary>
        /// <param name="srcPath">The path of the file to be added to the container</param>
        public void Add(string srcPath)
        {
            var fInfo = new FileInfo(srcPath);
            using var msAdd = new MemoryStream(File.ReadAllBytes(srcPath));
            Add(msAdd, fInfo.Name);
        }

        /// <summary>
        /// Adds an entry to the container from the specified stream.
        /// </summary>
        /// <param name="stream">The stream representing the data to be wirtten to the entry</param>
        /// <param name="name">The name of the entry for purposes of retrieving and extracting</param>
        public void Add(Stream stream, string name)
        {
            var obj = new ContainerObject(name);

            var efAdd = new EnigmaFile(Password);
            efAdd.SetData(stream);

            //Open the zipFile:
            using var fsContainer = new FileStream(Path, FileMode.Open);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Update);

            var zaeAdd = zaContainer.CreateEntry(obj.EntryName);
            efAdd.Save(zaeAdd.Open());

            zaContainer.Dispose();
            fsContainer.Dispose();

            Files.Add(obj);
            WriteHeaderFile();

        }

        /// <summary>
        /// Adds an XML serializable C# object to the container. Use to directly store and retieve class objects for easy operation.
        /// </summary>
        /// <typeparam name="T">The type of object to be serialized</typeparam>
        /// <param name="srcObj">The class object to be serialized and saved</param>
        /// <param name="name">The name of the object that will be used for retrieval purposes</param>
        public void AddObject<T>(T srcObj, string name)
        {
            var obj = new ContainerObject(name);

            //Create the enigma file:
            var efAdd = new EnigmaFile(Password);
            efAdd.Serialize(srcObj, false);

            //Open the zipFile:
            using var fsContainer = new FileStream(Path, FileMode.Open);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Update);

            var zaeAdd = zaContainer.CreateEntry(obj.EntryName);
            efAdd.Save(zaeAdd.Open());

            //Close streams:
            zaContainer.Dispose();
            fsContainer.Dispose();

            //Add the CO and write header:
            Files.Add(obj);
            WriteHeaderFile();
        }

        /// <summary>
        /// Returns an object that was previously saved into the container.
        /// </summary>
        /// <typeparam name="T">The type of the object to be retrieved</typeparam>
        /// <param name="obj">The object on which the deserialized data is applied</param>
        /// <param name="name">The name used to store the object in the container</param>
        public void Get<T>(out T obj, string name)
        {
            var efGet = new EnigmaFile(Password);

            //Open the zipFile:
            using var fsContainer = new FileStream(Path, FileMode.Open);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Read);
            var coGet = Files.Find((x) => x.PlainName == name);
            var zaeGet = zaContainer.GetEntry(coGet.EntryName);

            efGet.Load(zaeGet.Open());

            //Close streams:
            zaContainer.Dispose();
            fsContainer.Dispose();


            efGet.Deserialize(out obj);
        }

        /// <summary>
        /// Copies the contents of an entry to the specified stream
        /// </summary>
        /// <param name="stream">The stream to which the data will be copied</param>
        /// <param name="name">The name of the entry to be copied from</param>
        public void Get( Stream stream, string name)
        {
            var efGet = new EnigmaFile(Password);
            using var fsContainer = new FileStream(Path, FileMode.Open);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Read);
            var coGet = Files.Find((x) => x.PlainName == name);
            var zaeGet = zaContainer.GetEntry(coGet.EntryName);
            efGet.Load(zaeGet.Open());
            efGet.Decrypt(stream);
;
        }

        /// <summary>
        /// Copies the contents of an entry to the specified file path. If the file exists, it will be overwritten
        /// </summary>
        /// <param name="path">The path to the file where the data will be written</param>
        /// <param name="name">The name of the entry to be copied</param>
        public void Get(string path, string name)
        {
            var efEx = new EnigmaFile(Password);
            using var fsContainer = new FileStream(Path, FileMode.Open);
            using var zaContainer = new ZipArchive(fsContainer, ZipArchiveMode.Read);
            var coGet = Files.Find((x) => x.PlainName == name);
            var zaeGet = zaContainer.GetEntry(coGet.EntryName);
            efEx.Load(zaeGet.Open());
            using var msData = new MemoryStream();
            efEx.Decrypt(msData);
            File.WriteAllBytes(path, msData.ToArray());
        }

        #endregion
    }
    /// <summary>
    /// Methods used to access an EnigmaContainer
    /// </summary>
    public enum ContainerAccess { Create, Open, CreateOrOpen};

    public enum ContainerExtractionMode { Complete, Parent}

    [DataContract]
    class ContainerObject
    {
        [DataMember] public string PlainName { get; set; }
        [DataMember] public string EntryName { get; set; }
        [DataMember] public string ExtractedDirectory { get; set; }
        [DataMember] public string Hash { get; set; }

        public ContainerObject(string name)
        {
            PlainName = name;
            EntryName = Enigma.GenerateID();
        }
    }

}
