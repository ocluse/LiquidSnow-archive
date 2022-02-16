using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thismaker.Horus.IO;

namespace Thismaker.Esna
{
    internal class HorusStorageProvider<T> : StorageProviderBase<T>
    {
        private readonly string _path;
        private readonly string _key;

        public HorusStorageProvider(string path, string key)
        {
            _path = path;
            _key = key;
        }

        private ICryptoContainer GetParentContainer()
        {
            using var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            return IOBuilder.CreateContainer(_key, fs);
        }

        private async Task<ICryptoContainer> GetPartitionContainer(string partitonKey)
        {

            using var parent = GetParentContainer();
            using var ms = new MemoryStream();

            await parent.GetAsync(partitonKey, ms);
            return IOBuilder.CreateContainer(_key, ms);
        }

        public override async Task UpsertAsync(T item, string id, string partitionKey)
        {
            using var container = await GetPartitionContainer(partitionKey);
            await container.AddAsync(id, item, true);
        }

        public override async Task<T> ReadAsync(string id, string partitionKey)
        {
            using var container = await GetPartitionContainer(partitionKey);
            return await container.GetAsync<T>(id);
        }

        public override async Task DeleteAsync(string id, string partitionKey)
        {
            using var container = await GetPartitionContainer(partitionKey);
            container.Delete(id);
        }

        public override async Task<bool> ExistsAsync(string id, string partitionKey)
        {
            using var container = await GetPartitionContainer(partitionKey);
            return container.Exists(id);
        }

        public override Task CreatePartitionAsync(string partitionKey)
        {
            using var parent = GetParentContainer();
            return parent.AddBytesAsync(partitionKey, new byte[0]);
        }

        public override Task DeletePartitionAsync(string partitionKey)
        {
            using var parent = GetParentContainer();
            parent.Delete(partitionKey);
            return Task.CompletedTask;
        }

        public override Task<IEnumerable<string>> EnumeratePartitionKeys()
        {
            using var parent = GetParentContainer();
            return Task.FromResult(parent.EnumerateItems().AsEnumerable());
        }

        public override Task<IEnumerable<string>> EnumerateItemIds(string partitionKey)
        {
            var task = GetPartitionContainer(partitionKey);
            task.Wait();
            using var container = task.Result;
            return Task.FromResult(container.EnumerateItems().AsEnumerable());
        }

        public override async Task<Dictionary<string, string>> LoadDirectoryStructureAsync()
        {
            using var parent = GetParentContainer();
            if (parent.Exists(Extensions.DIR_STRUCT_ID))
            {
                return await parent.GetAsync<Dictionary<string,string>>(Extensions.DIR_STRUCT_ID);
            }

            return new Dictionary<string, string>();
        }

        public override async Task SaveDirectoryStructureAsync(Dictionary<string, string> directory)
        {
            using var parent = GetParentContainer();
            await parent.AddAsync(_key, directory, true);
        }
    }
}