using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Horus.IO;

namespace Thismaker.Esna
{
    internal class AzureBlobStorageProvider<T> : StorageProviderBase<T>
    {
        class Database<T1>
        {
            public Dictionary<string, string> DirectoryStructure { get; set; }
            public Dictionary<string, Partition<T1>> Partitions { get; set; }
        }
        
        class Partition<T1>
        {
            public Dictionary<string, T1> Items { get; set; }
        }
        
        private readonly BlobClient _blob;
        private readonly ISerializer _serializer;
        private Database<T> _database;


        public AzureBlobStorageProvider(BlobClient blob, ISerializer serializer)
        {
            _serializer = serializer;
            _blob = blob;
        }

        private void EnsureLoadedDirectoryStructure()
        {
            if(_database == null)
            {
                throw new InvalidOperationException($"You must call {nameof(LoadDirectoryStructureAsync)} before attempting to operate on the database");
            }
        }

        private async Task SaveDatabaseAsync()
        {
            EnsureLoadedDirectoryStructure();

            using var ms = new MemoryStream();
            await _serializer.SerializeAsync(_database, ms);
            ms.Position = 0;
            await _blob.UploadAsync(ms, true);
        }

        public override Task CreatePartitionAsync(string partitionKey)
        {
            return Task.CompletedTask;
        }

        public override Task DeletePartitionAsync(string partitionKey)
        {
            return Task.CompletedTask;
        }

        public override async Task UpsertAsync(T item, string id, string partitionKey)
        {
            EnsureLoadedDirectoryStructure();

            if(!_database.Partitions.TryGetValue(partitionKey, out Partition<T> partition))
            {
                partition = new Partition<T>()
                {
                    Items = new Dictionary<string, T>()
                };

                _database.Partitions.Add(partitionKey, partition);
            }

            if(partition.Items.TryAdd(id, item))
            {
                _database.DirectoryStructure.Add(id, partitionKey);
            }
            else
            {
                partition.Items[id] = item;
            }
            await SaveDatabaseAsync();
        }

        public override Task<T> ReadAsync(string id, string partitionKey)
        {
            EnsureLoadedDirectoryStructure();

            var item = _database.Partitions[partitionKey].Items[id];
            return Task.FromResult(item);
        }

        public override async Task DeleteAsync(string id, string partitionKey)
        {
            EnsureLoadedDirectoryStructure();
            
            var partition = _database.Partitions[partitionKey];

            partition.Items.Remove(id);
            _database.DirectoryStructure.Remove(id);

            if (partition.Items.Count == 0)
            {
                _database.Partitions.Remove(partitionKey);
            }

            await SaveDatabaseAsync();
        }

        public async override Task<Dictionary<string, string>> LoadDirectoryStructureAsync()
        {
            if(await _blob.ExistsAsync())
            {
                using var ms = new MemoryStream();
                await _blob.DownloadToAsync(ms);
                ms.Position = 0;
                _database = await _serializer.DeserializeAsync<Database<T>>(ms);
            }
            else
            {
                _database = new Database<T>()
                {
                    DirectoryStructure = new Dictionary<string, string>(),
                    Partitions = new Dictionary<string, Partition<T>>()
                };
            }
            return new Dictionary<string, string>(_database.DirectoryStructure);
        }
        
        public override Task SaveDirectoryStructureAsync(Dictionary<string, string> directory)
        {
            return Task.CompletedTask;
        } 
    }
}
