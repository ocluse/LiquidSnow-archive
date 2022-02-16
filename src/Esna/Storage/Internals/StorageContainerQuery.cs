using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    internal class StorageContainerQuery<TModel, TStorage> : IContainerQuery<TModel, TStorage>
    {
        abstract class QueuedQuery
        {
            public abstract IQueryable<TStorage> Execute(IQueryable<TStorage> e);
        }
        class OrderByQuery<TKeySelector> : QueuedQuery
        {
            public Expression<Func<TStorage, TKeySelector>> Predicate { get; set; }

            public override IQueryable<TStorage> Execute(IQueryable<TStorage> e)
            {
                return e.OrderBy(Predicate);
            }
        }
        class OrderByDescendingQuery<TKeySelector> : QueuedQuery
        {
            public Expression<Func<TStorage, TKeySelector>> Predicate { get; set; }

            public override IQueryable<TStorage> Execute(IQueryable<TStorage> e)
            {
                return e.OrderByDescending(Predicate);
            }
        }
        class SkipQuery : QueuedQuery
        {
            public int Count { get; set; }

            public override IQueryable<TStorage> Execute(IQueryable<TStorage> e)
            {
                return e.Skip(Count);
            }
        }
        class TakeQuery : QueuedQuery
        {
            public int Count { get; set; }

            public override IQueryable<TStorage> Execute(IQueryable<TStorage> e)
            {
                return e.Take(Count);
            }
        }
        class WhereQuery : QueuedQuery
        {
            public Expression<Func<TStorage, bool>> Predicate { get; set; }

            public override IQueryable<TStorage> Execute(IQueryable<TStorage> e)
            {
                return e.Where(Predicate);
            }
        }

        private readonly Queue<QueuedQuery> _queuedQueries;
        private readonly ContainerSettings<TModel, TStorage> _containerSettings;
        private readonly IStorageProvider<TStorage> _storageProvider;

        public StorageContainerQuery(ContainerSettings<TModel, TStorage> containerSettings, IStorageProvider<TStorage> storageProvider)
        {
            _queuedQueries = new Queue<QueuedQuery>();
            _containerSettings = containerSettings;
            _storageProvider = storageProvider;
        }

        public async Task<List<TModel>> ExecuteAsync()
        {
            List<TStorage> items = await _storageProvider.GetAllItemsAsync();

            foreach(var item in _queuedQueries)
            {
                item.Execute(items.AsQueryable());
            }

            List<TModel> result = new List<TModel>();

            foreach(var item in items)
            {
                result.Add(_containerSettings.ConvertToModel(item));
            }

            return result;
        }

        public IContainerQuery<TModel, TStorage> OrderBy<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate)
        {
            _queuedQueries.Enqueue(new OrderByQuery<TKeySelector>() { Predicate = predicate });
            return this;
        }

        public IContainerQuery<TModel, TStorage> OrderByDescending<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate)
        {
            _queuedQueries.Enqueue(new OrderByDescendingQuery<TKeySelector>() { Predicate = predicate });
            return this;
        }

        public IContainerQuery<TModel, TStorage> Skip(int count)
        {
            _queuedQueries.Enqueue(new SkipQuery() { Count = count });
            return this;
        }

        public IContainerQuery<TModel, TStorage> Take(int count)
        {
            _queuedQueries.Enqueue(new TakeQuery() { Count = count });
            return this;
        }

        public IContainerQuery<TModel, TStorage> Where(Expression<Func<TStorage, bool>> storage)
        {
            _queuedQueries.Enqueue(new WhereQuery() { Predicate = storage });
            return this;
        }
    }
}
