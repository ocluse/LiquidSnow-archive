using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading;
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
            var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            return IOBuilder.CreateContainer(_key, fs);
        }

        public override async Task UpsertAsync(T item, string id, string partitionKey)
        {
            using var parent = GetParentContainer();
            await parent.AddAsync($"{partitionKey}/{id}", item, true);
        }

        public override async Task<T> ReadAsync(string id, string partitionKey)
        {
            using var parent = GetParentContainer();
            return await parent.GetAsync<T>($"{partitionKey}/{id}");
        }

        public override Task DeleteAsync(string id, string partitionKey)
        {
            using var parent = GetParentContainer();
            parent.Delete($"{partitionKey}/{id}");
            return Task.CompletedTask;
        }

        public override Task<bool> ExistsAsync(string id, string partitionKey)
        {
            using var parent = GetParentContainer();
            return Task.FromResult(parent.Exists($"{partitionKey}/{id}"));
        }

        public override Task CreatePartitionAsync(string partitionKey)
        {
            return Task.CompletedTask;
        }

        public override Task DeletePartitionAsync(string partitionKey)
        {
            return Task.CompletedTask;
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
            await parent.AddAsync(Extensions.DIR_STRUCT_ID, directory, true);
        }
    }
}