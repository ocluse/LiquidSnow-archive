using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    /// <summary>
    /// Contains utility methods for running queries on <see cref="IContainerHandler{TModel, TStorage}"/>
    /// </summary>
    /// <typeparam name="TModel">The in use state of the data</typeparam>
    /// <typeparam name="TStorage">The storage state of the data</typeparam>
    public interface IContainerQuery<TModel, TStorage>
    {
        /// <summary>
        /// Executes the query and returns all items matching the query.
        /// </summary>
        /// <returns></returns>
        Task<List<TModel>> ExecuteAsync();

        /// <summary>
        /// A query that returns all items that match the provided expression
        /// </summary>
        IContainerQuery<TModel, TStorage> Where(Expression<Func<TStorage, bool>> storage);
        
        /// <summary>
        /// A query that orders the items from the previous list by the provided key
        /// </summary>
        IContainerQuery<TModel, TStorage> OrderBy<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate);
        
        /// <summary>
        /// A query that orders the items from the previous list in a descending order by the provided key.
        /// </summary>
        /// <typeparam name="TKeySelector"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IContainerQuery<TModel, TStorage> OrderByDescending<TKeySelector>(Expression<Func<TStorage, TKeySelector>> predicate);
        
        /// <summary>
        /// A query that skips the provided number of items from the previus list.
        /// </summary>
        IContainerQuery<TModel, TStorage> Skip(int count);
        
        /// <summary>
        /// A query that takes the provided number of items from the previous list.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IContainerQuery<TModel, TStorage> Take(int count);

    }
}
