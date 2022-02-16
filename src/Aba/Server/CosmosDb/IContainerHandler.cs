using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Server.CosmosDb
{
    public interface IContainerHandler<TModel, TStorage>
    {
        Task CreateAsync(TModel item, string partitionKey);

        Task<TModel> ReadAsync(string id, string partitionKey);

        Task ReplaceAsync(TModel item, string id, string partitionKey);

        Task UpsertAsync(TModel item, string partitionKey);

        Task DeleteAsync(string id, string partitionKey);
    }
}
