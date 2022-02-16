using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Thismaker.Aba.Server.CosmosDb
{
    public class LocalContainerHandler<TModel, TStorage> : IContainerHandler<TModel, TStorage>
    {
        private readonly string _containerPath;
        private readonly PropertyInfo _idProperty;
        private readonly Dictionary<string, Dictionary<string, TModel>> _storages;

        public LocalContainerHandler(string containerName, string basePath, string idPropName="Id")
        {
            _containerPath = Path.Combine(basePath, containerName);
            _storages = new Dictionary<string, Dictionary<string, TModel>>();
            _ = Directory.CreateDirectory(_containerPath);
        }

        public async Task CreateAsync(TModel item, string partitionKey)
        {
            await EnsurePartitionLoaded(partitionKey);

            if (!_storages.TryGetValue(partitionKey, out Dictionary<string, TModel> storage))
            {
                storage = new Dictionary<string, TModel>();
                _storages.Add(partitionKey, storage);
            }

            if (storage.TryAdd(item.Id, item))
            {
                string path = Path.Combine(_containerPath, partitionKey);
                File.WriteAllText(path, JsonConvert.SerializeObject(storage));
            }
            else
            {
                throw new InvalidOperationException("Id already exists");
            }
        }

        public async Task<TModel> ReadAsync(string id, string partitionKey)
        {
            await EnsurePartitionLoaded(partitionKey);

            if (_storages.TryGetValue(partitionKey, out Dictionary<string, TModel> storage))
            {
                if (storage.TryGetValue(id, out TModel item))
                {
                    return item;
                }
            }

            throw new ResourceNotFoundException();
        }

        public async Task ReplaceAsync(TModel item, string id, string partitionKey)
        {
            await EnsurePartitionLoaded(partitionKey);

            if (_storages.TryGetValue(partitionKey, out Dictionary<string, TModel> storage))
            {
                try
                {
                    storage[id] = item;

                    string path = PathOf(partitionKey);

                    File.WriteAllText(path, JsonConvert.SerializeObject(storage));

                    //using FileStream fs = File.Create(path);
                    //await JsonSerializer.SerializeAsync(fs, storage);

                }
                catch (KeyNotFoundException)
                {
                    throw new ResourceNotFoundException();
                }
            }
            else
            {
                throw new ResourceNotFoundException();
            }
        }

        public async Task UpsertAsync(TModel item, string partitionKey)
        {
            try
            {
                await ReplaceAsync(item, item.Id, partitionKey);
            }
            catch (ResourceNotFoundException)
            {
                await CreateAsync(item, partitionKey);
            }
        }

        public async Task DeleteAsync(string id, string partitionKey)
        {
            await EnsurePartitionLoaded(partitionKey);

            if (_storages.TryGetValue(partitionKey, out Dictionary<string, TModel> storage))
            {
                if (storage.Remove(id))
                {
                    string path = PathOf(partitionKey);

                    if (storage.Count == 0)
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        File.WriteAllText(path, JsonConvert.SerializeObject(storage));
                        //using FileStream fs = File.Create(path);
                        //await JsonSerializer.SerializeAsync(fs, storage);
                    }
                }
                else
                {
                    throw new ResourceNotFoundException();
                }
            }
            else
            {
                throw new ResourceNotFoundException();
            }
        }

        public async Task<List<TModel>> QueryFirmResourceAsync(string partitionKey, DateTime fromDate, int page)
        {
            await EnsurePartitionLoaded(partitionKey);

            List<TModel> result = new();

            if (_storages.TryGetValue(partitionKey, out Dictionary<string, TModel> storage))
            {
                IEnumerable<KeyValuePair<string, TModel>> query = storage.OrderBy(x => x.Value.LastUpdated)
                    .Where(x => x.Value.LastUpdated > fromDate)
                    .Skip(page * 100);

                foreach (KeyValuePair<string, TModel> x in query)
                {
                    result.Add(x.Value);

                    if (result.Count == 100)
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        private string PathOf(string partitonKey)
        {
            return Path.Combine(_containerPath, partitonKey);
        }

        private Task EnsurePartitionLoaded(string partitionKey)
        {
            if (_storages.ContainsKey(partitionKey))
            {
                return Task.CompletedTask;
            }

            string path = PathOf(partitionKey);

            if (File.Exists(path))
            {
                Dictionary<string, TModel> storage = JsonConvert.DeserializeObject<Dictionary<string, TModel>>(File.ReadAllText(path));

                //using FileStream fs = File.OpenRead(path);

                //Dictionary<string, T> storage = await JsonSerializer.DeserializeAsync<Dictionary<string, T>>(fs);

                _storages.Add(partitionKey, storage);
            }

            return Task.CompletedTask;
        }
    }
}
}
