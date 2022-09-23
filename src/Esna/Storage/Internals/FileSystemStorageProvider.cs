using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Thismaker.Core.Utilities;
using Thismaker.Horus.IO;

namespace Thismaker.Esna
{
    internal class FileSystemStorageProvider<T> : StorageProviderBase<T>
    {
        private readonly ISerializer _serializer;
        private readonly string _path;

        public FileSystemStorageProvider(string path, ISerializer serializer)
        {
            _serializer = serializer;
            _path = path;
        }

        private string ItemPath(string id, string partitionKey)
        {
            return IOUtility.CombinePath(PartitionKeyPath(partitionKey), id);
        }

        public string PartitionKeyPath(string partitionKey)
        {
            return IOUtility.CombinePath(_path, partitionKey);
        }

        public override async Task UpsertAsync(T item, string id, string partitionKey)
        {
            string path = ItemPath(id, partitionKey);

            using FileStream fs= File.Create(path);

            await _serializer.SerializeAsync(item, fs);
        }

        public override async Task<T> ReadAsync(string id, string partitionKey)
        {
            string path = ItemPath(id, partitionKey);
            using FileStream fs=File.OpenRead(path);
            return await _serializer.DeserializeAsync<T>(fs);
        }

        public override Task DeleteAsync(string id, string partitionKey)
        {
            string path = ItemPath(id, partitionKey);
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return Task.CompletedTask;
        }

        public override Task CreatePartitionAsync(string partitionKey)
        {
            string path = PartitionKeyPath(partitionKey);
            Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public override Task DeletePartitionAsync(string partitionKey)
        {
            string path=PartitionKeyPath(partitionKey);
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public override async Task<Dictionary<string, string>> LoadDirectoryStructureAsync()
        {
            string path = IOUtility.CombinePath(_path, Extensions.DIR_STRUCT_ID);

            if (File.Exists(path))
            {
                using var fs = File.OpenRead(path);
                return await _serializer.DeserializeAsync<Dictionary<string, string>>(fs);

            }
            return new Dictionary<string, string>();
        }

        public override async Task SaveDirectoryStructureAsync(Dictionary<string, string> directory)
        {
            string path = IOUtility.CombinePath(_path, Extensions.DIR_STRUCT_ID);

            using var fs = File.Create(path);

            await _serializer.SerializeAsync(directory, fs);
        }
    }
}