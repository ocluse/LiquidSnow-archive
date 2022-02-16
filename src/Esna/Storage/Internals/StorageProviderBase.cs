using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    internal abstract class StorageProviderBase<T> : IStorageProvider<T>
    {
        public abstract Task DeleteAsync(string id, string partitionKey);
        public abstract Task<IEnumerable<string>> EnumerateItemIds(string partitionKey);
        public abstract Task<IEnumerable<string>> EnumeratePartitionKeys();
        public abstract Task<bool> ExistsAsync(string id, string partitionKey);
        public abstract Task<T> ReadAsync(string id, string partitionKey);
        public abstract Task UpsertAsync(T item, string id, string partitionKey);
        public abstract Task<Dictionary<string, string>> LoadDirectoryStructureAsync();
        public abstract Task SaveDirectoryStructureAsync(Dictionary<string, string> directory);
        public abstract Task CreatePartitionAsync(string partitionKey);
        public abstract Task DeletePartitionAsync(string partitionKey);

        public async IAsyncEnumerable<T> EnumerateItemsAsync()
        {
            foreach (var partition in await EnumeratePartitionKeys())
            {
                foreach (var itemId in await EnumerateItemIds(partition))
                {
                    yield return await ReadAsync(itemId, partition);
                }
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
