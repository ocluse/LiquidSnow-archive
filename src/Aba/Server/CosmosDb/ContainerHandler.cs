using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Thismaker.Aba.Server.CosmosDb
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
    public class ContainerHandler<TModel, TStorage>
    {
        private readonly Container _inner;
        private readonly PropertyInfo _idProperty;
        private readonly Func<TModel, TStorage> _convertToStorage;
        private readonly Func<TStorage, TModel> _convertToModel;

        /// <summary>
        /// Creates a new instance of the <see cref="ContainerHandler{TModel, TStorage}"/>
        /// </summary>
        /// <remarks>
        /// If no converts are provided, the storage form and model form are assumed to be the same.
        /// </remarks>
        /// <param name="innerContainer">The underlying CosmosDb container</param>
        /// <param name="convertToStorage">The function to be called when coverting the data to its storage form</param>
        /// <param name="covnertToModel">The function to be called when converting the data to its "in use" form </param>
        /// <param name="idPropName">The name of the ID property, e.g Id, used when extracting an ID value from the data</param>
        public ContainerHandler(Container innerContainer, Func<TModel, TStorage> convertToStorage=null, Func<TStorage, TModel> covnertToModel=null, string idPropName = "Id")
        {
            _inner = innerContainer;
            _idProperty = typeof(TStorage).GetProperty(idPropName);
            _convertToModel = covnertToModel;
            _convertToStorage = convertToStorage;
        }

        private PartitionKey GetPartitionKey(string id, object partitionKey)
        {
            if(partitionKey == null)
            {
                partitionKey = id;
            }

            //TODO:  this function could certainly be made better
            if (partitionKey is string str)
            {
                return new PartitionKey(str);
            }
            else if(partitionKey is int i)
            {
                return new PartitionKey(i);
            }
            else if(partitionKey is bool b)
            {
                return new PartitionKey(b);
            }
            else if(partitionKey is double d)
            {
                return new PartitionKey(d);
            }

            throw new InvalidOperationException($"The type of partition key passed to the method is not supported");
        }

        private TStorage ConvertToStorage(TModel model)
        {
            if(_convertToStorage == null)
            {
                return (TStorage)(object)model;
            }
            else
            {
                return ConvertToStorage(model);
            }
        }

        private TModel ConvertToModel(TStorage storage)
        {
            if(_convertToModel == null)
            {
                return (TModel)(object)storage;
            }
            else
            {
                return ConvertToModel(storage);
            }
        }

        /// <summary>
        /// Creates a new item in the container, by first converting it to storage class.
        /// </summary>
        /// <param name="item">The item to be added to the container</param>
        public virtual async Task CreateAsync(TModel item)
        {   
            TStorage storage = ConvertToStorage(item);
            _ = await _inner.CreateItemAsync(storage);
        }

        /// <summary>
        /// Reads an item from the container and converts it from storage form.
        /// </summary>
        /// <param name="id">The ID of the the item to be read</param>
        /// <param name="partitionKey">The partition key of the item. If null, the Id will be used as partition key instead</param>
        /// <returns>The item read from the container</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the item is not found in the containner</exception>
        public virtual async Task<TModel> ReadAsync(string id, object partitionKey = null)
        {
            try
            {
                ItemResponse<TStorage> response = await _inner.ReadItemAsync<TStorage>(id, GetPartitionKey(id, partitionKey));
                return ConvertToModel(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ResourceNotFoundException();
            }
        }

        /// <summary>
        /// Updates an item in the container.
        /// </summary>
        /// <param name="item">The item to be updated in the container.</param>
        /// <exception cref="ResourceNotFoundException">Thrown when the item is not found in the container.</exception>
        public async Task UpdateAsync(TModel item)
        {
            TStorage storage = ConvertToStorage(item);

            string id = (string)_idProperty.GetValue(storage);

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
        /// Deletes an item with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the item to delete</param>
        /// <param name="partitonKey">The partition key of the item to delete, if none is provided, the ID will be used as partition key instead</param>
        /// <returns>True if the item was found and successfully deleted, false if it was not found</returns>
        public virtual async Task<bool> DeleteAsync(string id, object partitonKey = null)
        {
            try
            {
                _ = await _inner.DeleteItemAsync<TStorage>(id, GetPartitionKey(id, partitonKey));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a <see cref="ContainerQuery{TModel, TStorage}"/> to use to run queries on the container
        /// </summary>
        /// <returns>A <see cref="ContainerQuery{TModel, TStorage}"/> that can be used to run a query on the container</returns>
        public virtual ContainerQuery<TModel, TStorage> Query()
        {
            IQueryable<TStorage> query = _inner.GetItemLinqQueryable<TStorage>();

            ContainerQuery<TModel, TStorage> result = new ContainerQuery<TModel, TStorage>(ConvertToModel, query);
            return result;
        }

        /// <summary>
        /// Finds and returns the first item that matches the provided exrepssion
        /// </summary>
        /// <param name="predicate">The expression to use in findint the item</param>
        /// <returns>The first item matching the expression, otherwise throws a <see cref="ResourceNotFoundException"/> when there are no matches</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the item is not found</exception>
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
                    return _convertToModel(item);
                }
            }

            throw new ResourceNotFoundException();
        }

        /// <summary>
        /// Finds all the items matching the provided expression
        /// </summary>
        /// <param name="predicate">The expression to use in searching for the items</param>
        /// <param name="maxItems">The maximum number of items to return from the search, if -1, ALL items matching the expression will be returned</param>
        /// <returns>A list containing the found items. The list is empty if no items are found</returns>
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
                    TModel model = ConvertToModel(item);
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