using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    internal abstract class StorageProviderBase<T> : IStorageProvider<T>
    {
        public abstract Task DeleteAsync(string id, string partitionKey);
        public abstract Task<bool> ExistsAsync(string id, string partitionKey);
        public abstract Task<T> ReadAsync(string id, string partitionKey);
        public abstract Task UpsertAsync(T item, string id, string partitionKey);
        public abstract Task<Dictionary<string, string>> LoadDirectoryStructureAsync();
        public abstract Task SaveDirectoryStructureAsync(Dictionary<string, string> directory);
        public abstract Task CreatePartitionAsync(string partitionKey);
        public abstract Task DeletePartitionAsync(string partitionKey);

        public async IAsyncEnumerable<T> EnumerateItemsAsync()
        {
            var dirStruct = await LoadDirectoryStructureAsync();

            foreach(var item in dirStruct)
            {
                yield return await ReadAsync(item.Key, item.Value);
            }
        }

        public async Task<List<T>> GetAllItemsAsync()
        {
            List<T> items = new List<T>();
            await foreach (var item in EnumerateItemsAsync())
            {
                items.Add(item);
            }
            return items;
        }

        
    }
}
