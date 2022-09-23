using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    /// <summary>
    /// A class with utility methods for operating on a LiteDb Collection
    /// </summary>
    /// <remarks>
    /// Due to the way LiteDb is structured, the partition key is always ignored when storing data, 
    /// i.e no partitions will be created. However, the model will still be converted 
    /// to <typeparamref name="TStorage"/> to ensure compatibility with other implementers.
    /// </remarks>
    public class LiteDbContainerHandler<TModel, TStorage> : IContainerHandler<TModel, TStorage>
    {
        private readonly ILiteCollection<TStorage> _collection;
        private readonly ContainerSettings<TModel, TStorage> _containerSettings;
        
        /// <summary>
        /// Creates a new instance to handle the provided collection and container settings.
        /// </summary>
        /// <param name="collection">The underlying LiteDb collection</param>
        /// <param name="containerSettings">The settings for the container</param>
        public LiteDbContainerHandler(ILiteCollection<TStorage> collection, ContainerSettings<TModel, TStorage> containerSettings)
        {
            _collection = collection;
            _containerSettings = containerSettings;
        }

        ///<inheritdoc/>
        public Task CreateAsync(TModel item, ConvertArgs args = null)
        {
            TStorage storage = _containerSettings.ConvertToStorage(item, args);
            _collection.Insert(storage);
            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public Task<bool> DeleteAsync(string id, object partitionKey = null)
        {
            return Task.FromResult(_collection.Delete(id));
        }

        ///<inheritdoc/>
        public Task<List<TModel>> FindAllAsync(Expression<Func<TStorage, bool>> predicate, int maxItems = 100)
        {
            List<TModel> result = new List<TModel>();
            var query = _collection.Query().Where(predicate).Limit(maxItems);
            
            foreach(var item in query.ToEnumerable())
            {
                result.Add(_containerSettings.ConvertToModel(item));
            }

            return Task.FromResult(result);
        }

        ///<inheritdoc/>
        public Task<TModel> FindAsync(Expression<Func<TStorage, bool>> predicate)
        {
            var item = _collection.FindOne(predicate);
            if (item == null)
            {
                throw new ResourceNotFoundException();
            }
            else
            {
                return Task.FromResult(_containerSettings.ConvertToModel(item));
            }
        }

        ///<inheritdoc/>
        public IContainerQuery<TModel, TStorage> Query()
        {
            var query = _collection.Query();
            return new LiteDbContainerQuery<TModel, TStorage>(_containerSettings.ConvertToModel, query);
        }

        ///<inheritdoc/>
        public Task<TModel> ReadAsync(string id, object partitionKey = null)
        {
            TStorage item = _collection.FindById(id);

            if (item == null)
            {
                throw new ResourceNotFoundException();
            }
            else
            {
                return Task.FromResult(_containerSettings.ConvertToModel(item));
            }
        }

        ///<inheritdoc/>
        public Task UpdateAsync(TModel item, ConvertArgs args = null)
        {
            TStorage storage = _containerSettings.ConvertToStorage(item, args);
            _collection.Update(storage);
            return Task.CompletedTask;
        }
    }
}
