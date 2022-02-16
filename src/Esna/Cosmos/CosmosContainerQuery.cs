using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    internal class CosmosContainerQuery<TModel, TStorage> : IContainerQuery<TModel, TStorage>
    {
        private IQueryable<TStorage> _query;
        private readonly Func<TStorage, TModel> _convertToModel;

        internal CosmosContainerQuery(Func<TStorage, TModel> convertToModel, IQueryable<TStorage> query)
        {
            _convertToModel = convertToModel;
            _query = query;
        }

        public async Task<List<TModel>> ExecuteAsync()
        {
            List<TModel> result = new List<TModel>();

            using (FeedIterator<TStorage> feed = _query.ToFeedIterator())
            {
                while (feed.HasMoreResults)
                {
                    foreach (TStorage item in await feed.ReadNextAsync())
                    {
                        TModel model = _convertToModel(item);
                        result.Add(model);
                    }
                }
            }

            return result;
        }

        public IContainerQuery<TModel, TStorage> Where(Expression<Func<TStorage, bool>> predicate)
        {
            _query = _query.Where(predicate);
            return this;
        }

        public IContainerQuery<TModel, TStorage> OrderBy<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate)
        {
            _query = _query.OrderBy(predicate);
            return this;
        }

        public IContainerQuery<TModel, TStorage> OrderByDescending<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate)
        {
            _query = _query.OrderByDescending(predicate);
            return this;
        }

        public IContainerQuery<TModel, TStorage> Skip(int count)
        {
            _query = _query.Skip(count);
            return this;
        }

        public IContainerQuery<TModel, TStorage> Take(int count)
        {
            _query = _query.Take(count);
            return this;
        }
    }
}