using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thismaker.Esna
{
    /// <summary>
    /// Manage and store a database.
    /// </summary>
    /// <typeparam name="TModel">The "in use" form of the data</typeparam>
    /// <typeparam name="TStorage">The stored form of the data</typeparam>
    public interface IContainerHandler<TModel, TStorage>
    {
        /// <summary>
        /// Creates a new item in the container, by first converting it to storage class.
        /// </summary>
        /// <param name="item">The item to be added to the container</param>
        /// <param name="args">The arguments passed to the model-storage converter</param>
        ///<exception cref="ResourceConflictException">When the ID already exists in the database</exception>
        Task CreateAsync(TModel item, ConvertArgs args = null);

        /// <summary>
        /// Reads an item from the container and converts it from storage form.
        /// </summary>
        /// <param name="id">The ID of the the item to be read.</param>
        /// <param name="partitionKey">The partition key of the item. If null, the Id will be used as partition key instead.</param>
        /// <returns>The item read from the container.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the item is not found in the container.</exception>
        Task<TModel> ReadAsync(string id, object partitionKey = null);

        /// <summary>
        /// Updates an item in the container.
        /// </summary>
        /// <param name="item">The item to be updated in the container.</param>
        /// <param name="args">The arguments passed to the model-storage converter</param>
        /// <exception cref="ResourceNotFoundException">Thrown when the item is not found in the container.</exception>
        Task UpdateAsync(TModel item, ConvertArgs args = null);

        /// <summary>
        /// Deletes an item with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the item to delete</param>
        /// <param name="partitionKey">The partition key of the item to delete, if none is provided, the ID will be used as partition key instead</param>
        /// <returns>True if the item was found and successfully deleted, false if it was not found</returns>
        Task<bool> DeleteAsync(string id, object partitionKey = null);
        
        /// <summary>
        /// Creates a <see cref="IContainerQuery{TModel, TStorage}"/> that can be used to perform queries on the database
        /// </summary>
        IContainerQuery<TModel, TStorage> Query();

        /// <summary>
        /// Finds and returns the first item that matches the provided exrepssion
        /// </summary>
        /// <param name="predicate">The expression to use in findint the item</param>
        /// <returns>The first item matching the expression, otherwise throws a <see cref="ResourceNotFoundException"/> when there are no matches</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the item is not found</exception>
        Task<TModel> FindAsync(Expression<Func<TStorage, bool>> predicate);

        /// <summary>
        /// Finds all the items matching the provided expression
        /// </summary>
        /// <param name="predicate">The expression to use in searching for the items</param>
        /// <param name="maxItems">The maximum number of items to return from the search, if -1, ALL items matching the expression will be returned</param>
        /// <returns>A list containing the found items. The list is empty if no items are found</returns>
        Task<List<TModel>> FindAllAsync(Expression<Func<TStorage, bool>> predicate, int maxItems = 100);
    }
}