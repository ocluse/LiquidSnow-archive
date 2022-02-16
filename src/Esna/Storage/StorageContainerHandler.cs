using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Thismaker.Horus.IO;

namespace Thismaker.Esna
{
    /// <summary>
    /// Contains utility methods for storing, managing and retrieveing data stored via a <see cref="IStorageProvider{T}"/>
    /// </summary>
    /// <typeparam name="TModel">The "in use" model of the data</typeparam>
    /// <typeparam name="TStorage">The container storage model of the data</typeparam>
    public class StorageContainerHandler<TModel, TStorage> : IContainerHandler<TModel, TStorage>
    {
        private readonly IStorageProvider<TStorage> _storageProvider;
        private readonly ContainerSettings<TModel, TStorage> _containerSettings;
        private Dictionary<string, string> _dirStruct;
        #region Instansiation
        /// <summary>
        /// Creates an instance using the storage provider and settings provided.
        /// </summary>
        /// <param name="storageProvider">The storage provider to use</param>
        /// <param name="containerSettings">The container settings to use</param>
        public StorageContainerHandler(IStorageProvider<TStorage> storageProvider, ContainerSettings<TModel, TStorage> containerSettings)
        {
            _storageProvider = storageProvider;
            _containerSettings = containerSettings;
            _dirStruct = _storageProvider.LoadDirectoryStructureAsync().Result;
        }

        ///<inheritdoc cref="CreateFileSystemHandler(string, ContainerSettings{TModel, TStorage}, ISerializer)"/>
        public static StorageContainerHandler<TModel, TStorage> CreateFileSystemHandler(string path, ContainerSettings<TModel, TStorage> containerSettings)
        {
            var serializer = InternalSerializer.Instance;
            return CreateFileSystemHandler(path, containerSettings, serializer);
        }

        /// <summary>
        /// Creates a <see cref="StorageContainerHandler{TModel, TStorage}"/> that stores it database in a directory structure.
        /// </summary>
        /// <param name="path">The path to an exisitng directory where the database will be stored</param>
        /// <param name="containerSettings">The settings for the container</param>
        /// <param name="serializer">The serializer to use when serializing data</param>
        /// <returns>A <see cref="StorageContainerHandler{TModel, TStorage}"/> that stores its data in a directory structure.</returns>
        /// <exception cref="DirectoryNotFoundException">When the directory does not exist</exception>
        public static StorageContainerHandler<TModel, TStorage> CreateFileSystemHandler(string path, ContainerSettings<TModel, TStorage> containerSettings, ISerializer serializer)
        {
            var provider = new FileSystemStorageProvider<TStorage>(path, serializer);
            return new StorageContainerHandler<TModel, TStorage>(provider, containerSettings);
        }

        ///<inheritdoc cref="CreateCryptoContainerHandler(string, string, ContainerSettings{TModel, TStorage}, ISerializer)"/>
        public static StorageContainerHandler<TModel, TStorage> CreateCryptoContainerHandler(string path, string key, ContainerSettings<TModel, TStorage> containerSettings)
        {
            var serializer = InternalSerializer.Instance;
            return CreateCryptoContainerHandler(path, key, containerSettings, serializer);
        }

        /// <summary>
        /// Creates a <see cref="StorageContainerHandler{TModel, TStorage}"/> that stores it's database securely in a <see cref="ICryptoContainer"/>
        /// </summary>
        /// <remarks>
        /// When a serializer is provided, it will replace the <see cref="IOSettings.Serializer"/>.
        /// This serializer is used by all <see cref="ICryptoContainer"/> instances. Otherwise, the set/default serializer will be used.
        /// </remarks>
        /// <param name="path">The path to a <see cref="ICryptoContainer"/>. If the file does not exist, it will be created</param>
        /// <param name="key">Used to encrypt the database</param>
        /// <param name="containerSettings">The settings to use</param>
        /// <param name="serializer">The serializer to use</param>
        /// <returns>A <see cref="StorageContainerHandler{TModel, TStorage}"/> that stores its data securely in a <see cref="ICryptoContainer"/></returns>
        public static StorageContainerHandler<TModel,TStorage> CreateCryptoContainerHandler(string path, string key, ContainerSettings<TModel,TStorage> containerSettings, ISerializer serializer)
        {
            var provider = new HorusStorageProvider<TStorage>(path, key);
            IOSettings.Serializer = serializer;
            return new StorageContainerHandler<TModel, TStorage>(provider, containerSettings);
        }
        #endregion

        #region Private Methods

        private string GetPartitionKey(string id, object partitionKey)
        {
            if(partitionKey == null)
            {
                return id;
            }

            return partitionKey.ToString();
        }

        private async Task EnsureLoadedDirectoryStructure()
        {
            if (_dirStruct == null)
            {
                _dirStruct = await _storageProvider.LoadDirectoryStructureAsync();
            }
        }
        
        private void EnsureAllowed(string id, string pk)
        {
            if (id ==  Extensions.DIR_STRUCT_ID||pk==Extensions.DIR_STRUCT_ID)
            {
                throw new InvalidOperationException($"{Extensions.DIR_STRUCT_ID} cannot be used as an ID or Partition Key");
            }
        }

        private async Task EnsureExistsAsync(string id, object partitionKey)
        {
            string pk = GetPartitionKey(id, partitionKey);

            EnsureAllowed(id, pk);

            await EnsureLoadedDirectoryStructure();
            
            if(_dirStruct.TryGetValue(id, out string pkTest))
            {
                if(pkTest != pk)
                {
                    throw new ResourceNotFoundException();
                }
            }
            else
            {
                throw new ResourceNotFoundException();
            }
        }

        #endregion

        #region IContainerHandler
        ///<inheritdoc/>
        public async Task CreateAsync(TModel item, ConvertArgs args = null)
        {
            await EnsureLoadedDirectoryStructure();
            
            string id = _containerSettings.GetId(item);

            if (_dirStruct.ContainsKey(id))
            {
                throw new ResourceConflictException();
            }

            TStorage storage = _containerSettings.ConvertToStorage(item, args);

            object partitionKey = _containerSettings.GetPartitionKey(storage);

            string pk = GetPartitionKey(id, partitionKey);

            EnsureAllowed(id, pk);

            if (!_dirStruct.Values.Contains(pk))
            {
                await _storageProvider.CreatePartitionAsync(pk);
            }

            await _storageProvider.UpsertAsync(storage, id, pk);

            _dirStruct.Add(id, pk);

            await _storageProvider.SaveDirectoryStructureAsync(_dirStruct);
        }

        ///<inheritdoc/>
        public async Task<TModel> ReadAsync(string id, object partitionKey = null)
        {
            await EnsureExistsAsync(id, partitionKey);
            string pk = GetPartitionKey(id, partitionKey);
            TStorage storage = await _storageProvider.ReadAsync(id, pk);
            return _containerSettings.ConvertToModel(storage);
        }

        ///<inheritdoc/>
        public async Task UpdateAsync(TModel item, ConvertArgs args = null)
        {
            TStorage storage = _containerSettings.ConvertToStorage(item, args);
            
            string id = _containerSettings.GetId(item);
            object partitionKey = _containerSettings.GetPartitionKey(storage);

            await EnsureExistsAsync(id, partitionKey);

            await _storageProvider.UpsertAsync(storage, id, GetPartitionKey(id, partitionKey));
        }

        ///<inheritdoc/>
        public async Task<bool> DeleteAsync(string id, object partitionKey = null)
        {
            try
            {
                string pk = GetPartitionKey(id, partitionKey);
                await EnsureExistsAsync(id, partitionKey);

                await _storageProvider.DeleteAsync(id, pk);
                _dirStruct.Remove(id);
                await _storageProvider.SaveDirectoryStructureAsync(_dirStruct);

                if (!_dirStruct.Values.Contains(pk))
                {
                    await _storageProvider.DeletePartitionAsync(pk);
                }

                return true;
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
            
        }

        ///<inheritdoc/>
        public IContainerQuery<TModel, TStorage> Query()
        {
            return new StorageContainerQuery<TModel, TStorage>(_containerSettings, _storageProvider);
        }

        ///<inheritdoc/>
        public async Task<TModel> FindAsync(Expression<Func<TStorage, bool>> predicate)
        {
            var item = (await _storageProvider.GetAllItemsAsync()).AsQueryable().First(predicate);
            return _containerSettings.ConvertToModel(item);
        }

        ///<inheritdoc/>
        public async Task<List<TModel>> FindAllAsync(Expression<Func<TStorage, bool>> predicate, int maxItems = 100)
        {
            List<TModel> result = new List<TModel>();

            var items = (await _storageProvider.GetAllItemsAsync()).AsQueryable().Where(predicate);

            foreach(var item in items)
            {
                result.Add(_containerSettings.ConvertToModel(item));
            }

            return result;
        }
        #endregion
    }
}
