using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    /// <summary>
    /// An interface that provides handles used by a <see cref="StorageContainerHandler{TModel, TStorage}"/>
    /// to store and manage database records.
    /// </summary>
    /// <typeparam name="T">The type stored in the database</typeparam>
    public interface IStorageProvider<T>
    {
        /// <summary>
        /// Updates an item in the database, or if the item is not found, creates the item.
        /// </summary>
        /// <param name="item">The item to update or create</param>
        /// <param name="id">The ID of the item</param>
        /// <param name="partitionKey">The PartitionKey of the item</param>
        Task UpsertAsync(T item, string id, string partitionKey);

        /// <summary>
        /// Retrieves an item from the database.
        /// </summary>
        /// <param name="id">The ID of the item to read</param>
        /// <param name="partitionKey">The partition key of the item</param>
        Task<T> ReadAsync(string id, string partitionKey);
        
        /// <summary>
        /// Deletes an item from the database
        /// </summary>
        /// <param name="id">The ID of the item to delete</param>
        /// <param name="partitionKey">The PartitionKey of the item to delete</param>
        Task DeleteAsync(string id, string partitionKey);

        /// <summary>
        /// Creates a partition for the provided key
        /// </summary>
        /// <param name="partitionKey">The partition to create</param>
        Task CreatePartitionAsync(string partitionKey);

        /// <summary>
        /// Deletes a partition of the provided key.
        /// </summary>
        /// <param name="partitionKey">The partition to delete</param>
        Task DeletePartitionAsync(string partitionKey);

        /// <summary>
        /// Returns a dictionary that defines all item Ids with their partition keys
        /// </summary>
        Task<Dictionary<string, string>> LoadDirectoryStructureAsync();

        /// <summary>
        /// Save the dictionary that defines all item Ids with their partition Keys
        /// </summary>
        /// <returns></returns>
        Task SaveDirectoryStructureAsync(Dictionary<string, string> directory);

        /// <summary>
        /// Returns an awaitable enumerator that enumerates all the items in the database
        /// </summary>
        IAsyncEnumerable<T> EnumerateItemsAsync();
        
        /// <summary>
        /// Returns all the items in the database
        /// </summary>
        Task<List<T>> GetAllItemsAsync();
    }
}