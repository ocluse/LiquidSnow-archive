using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thismaker.Esna
{

    /// <summary>
    /// A class with utility methods for operating on Cosmos Db Containers.
    /// </summary>
    /// <remarks>
    /// The model type and storage type are there for conveniece where for example,
    /// some properties may need to be added before storage in cosmos containers.
    /// </remarks>
    /// <typeparam name="TModel">The "in use" model of the data</typeparam>
    /// <typeparam name="TStorage">The container storage model of the data</typeparam>
    public class CosmosContainerHandler<TModel, TStorage> : IContainerHandler<TModel, TStorage>
    {
        private readonly Container _inner;
        private readonly ContainerSettings<TModel, TStorage> _containerSettings;
        /// <summary>
        /// Creates a new instance of the <see cref="CosmosContainerHandler{TModel, TStorage}"/>
        /// </summary>
        /// <remarks>
        /// If no converts are provided, the storage form and model form are assumed to be the same.
        /// </remarks>
        /// <param name="innerContainer">The underlying CosmosDb container</param>
        /// <param name="containerSettings">The settings for the container</param>
        public CosmosContainerHandler(Container innerContainer, ContainerSettings<TModel, TStorage> containerSettings)
        {
            _inner = innerContainer;
            _containerSettings = containerSettings;
        }

        private PartitionKey GetPartitionKey(string id, object partitionKey)
        {
            if (partitionKey == null)
            {
                partitionKey = id;
            }

            //TODO:  this function could certainly be made better
            if (partitionKey is string str)
            {
                return new PartitionKey(str);
            }
            else if (partitionKey is int i)
            {
                return new PartitionKey(i);
            }
            else if (partitionKey is bool b)
            {
                return new PartitionKey(b);
            }
            else if (partitionKey is double d)
            {
                return new PartitionKey(d);
            }

            throw new InvalidOperationException($"The type of partition key passed to the method is not supported");
        }

        /// <inheritdoc/>
        public virtual async Task CreateAsync(TModel item, ConvertArgs args = null)
        {
            try
            {
                TStorage storage = _containerSettings.ConvertToStorage(item, args);
                _ = await _inner.CreateItemAsync(storage);
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw new ResourceConflictException();
            }
        }

        /// <inheritdoc/>
        public virtual async Task<TModel> ReadAsync(string id, object partitionKey = null)
        {
            try
            {
                ItemResponse<TStorage> response = await _inner.ReadItemAsync<TStorage>(id, GetPartitionKey(id, partitionKey));
                return _containerSettings.ConvertToModel(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ResourceNotFoundException();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(TModel item, ConvertArgs args = null)
        {
            TStorage storage = _containerSettings.ConvertToStorage(item, args);

            string id = _containerSettings.GetId(item);

            try
            {
                _ = await _inner.ReplaceItemAsync(storage, id);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ResourceNotFoundException();
            }
        }

        /// <summary>
        /// Patch an item.
        /// </summary>
        /// <remarks>
        /// The item itself is not patched, but instead what is stored in Azure Db. The item is only used for obtaining a PK and ID.
        /// </remarks>
        /// <param name="item">The item to patch</param>
        /// <param name="patchOperations">The patch operations to perform on the item.</param>
        /// <param name="args">The args to pass to the converter.</param>
        public async Task PatchAsync(TModel item, List<PatchOperation> patchOperations, ConvertArgs args = null)
        {
            string id=_containerSettings.GetId(item);
            object pk = _containerSettings.GetPartitionKey(_containerSettings.ConvertToStorage(item, args));

            await _inner.PatchItemAsync<TStorage>(id, GetPartitionKey(id, pk), patchOperations);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> DeleteAsync(string id, object partitionKey = null)
        {
            try
            {
                _ = await _inner.DeleteItemAsync<TStorage>(id, GetPartitionKey(id, partitionKey));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public virtual IContainerQuery<TModel, TStorage> Query()
        {
            IQueryable<TStorage> query = _inner.GetItemLinqQueryable<TStorage>();
            CosmosContainerQuery<TModel, TStorage> result = new CosmosContainerQuery<TModel, TStorage>(_containerSettings.ConvertToModel, query);
            return result;
        }

        /// <inheritdoc/>
        public async Task<TModel> FindAsync(Expression<Func<TStorage, bool>> predicate)
        {
            using FeedIterator<TStorage> feed = _inner
               .GetItemLinqQueryable<TStorage>()
               .Where(predicate)
               .ToFeedIterator();

            while (feed.HasMoreResults)
            {
                foreach (TStorage item in await feed.ReadNextAsync())
                {
                    return _containerSettings.ConvertToModel(item);
                }
            }

            throw new ResourceNotFoundException();
        }

        /// <inheritdoc/>
        public async Task<List<TModel>> FindAllAsync(Expression<Func<TStorage, bool>> predicate, int maxItems = 100)
        {
            List<TModel> result = new List<TModel>();

            using FeedIterator<TStorage> feed = _inner
                .GetItemLinqQueryable<TStorage>()
                .Where(predicate)
                .ToFeedIterator();

            while (feed.HasMoreResults)
            {
                foreach (TStorage item in await feed.ReadNextAsync())
                {
                    TModel model = _containerSettings.ConvertToModel(item);
                    result.Add(model);
                    if (result.Count >= maxItems && maxItems != -1)
                    {
                        return result;
                    }
                }
            }

            return result;
        }
    }
}