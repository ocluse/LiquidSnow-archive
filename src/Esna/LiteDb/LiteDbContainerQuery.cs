using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    internal class LiteDbContainerQuery<TModel, TStorage> : IContainerQuery<TModel, TStorage>
    {
        private readonly Func<TStorage, TModel> _convertToModel;
        private ILiteQueryable<TStorage> _query;
        private IQueryable<TStorage> _enumerable;
        internal LiteDbContainerQuery(Func<TStorage, TModel> convertToModel, ILiteQueryable<TStorage> query)
        {
            _convertToModel = convertToModel;
            _query = query;
        }

        public Task<List<TModel>> ExecuteAsync()
        {
            List<TModel> result = new List<TModel>();

            if (_enumerable == null)
            {
                _enumerable = _query.ToEnumerable().AsQueryable();
            }

            foreach (var item in _enumerable)
            {
                result.Add(_convertToModel(item));
            }

            return Task.FromResult(result);
        }

        public IContainerQuery<TModel, TStorage> OrderBy<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate)
        {
            if (_enumerable != null)
            {
                _enumerable = _enumerable.OrderBy(predicate);
            }
            else
            {
                _query = _query.OrderBy(predicate);
            }
            return this;
        }

        public IContainerQuery<TModel, TStorage> OrderByDescending<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate)
        {

            if (_enumerable != null)
            {
                _enumerable = _enumerable.OrderByDescending(predicate);
            }
            else
            {
                _query = _query.OrderByDescending(predicate);
            }

            return this;
        }

        public IContainerQuery<TModel, TStorage> Skip(int count)
        {
            if (_enumerable != null)
            {
                _enumerable=_enumerable.Skip(count);
            }
            else
            {
                _enumerable = _query.Skip(count).ToEnumerable().AsQueryable();
            }
            
            return this;
        }

        public IContainerQuery<TModel, TStorage> Take(int count)
        {
            if (_enumerable != null)
            {
                _enumerable = _enumerable.Take(count);
            }
            else
            {
                _enumerable = _query.Limit(count).ToEnumerable().AsQueryable();
            }
            return this;
        }

        public IContainerQuery<TModel, TStorage> Where(Expression<Func<TStorage, bool>> predicate)
        {
            if (_enumerable != null)
            {
                _enumerable = _enumerable.Where(predicate);
            }
            else
            {
                _query = _query.Where(predicate);
            }

            return this;
        }
    }
}
