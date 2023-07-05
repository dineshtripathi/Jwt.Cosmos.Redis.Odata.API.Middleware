using System.Linq.Expressions;

namespace ValidUsers.API.Repository.Core.Repository;
/// <summary>
/// The repository.
/// </summary>

public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Gets the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    Task<T?> GetItemAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the items async.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Adds the item async.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    Task<T> AddItemAsync(T item, string id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    Task UpdateItemAsync(string id, T item, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    Task DeleteItemAsync(string id, CancellationToken cancellationToken);
}