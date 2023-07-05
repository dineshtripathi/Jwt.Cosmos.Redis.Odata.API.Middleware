using System.Linq.Expressions;
using System.Text.Json;
using StackExchange.Redis;
using ValidUsers.API.Repository.Core.DTO;

namespace ValidUsers.API.Repository.Core.Repository;

/// <summary>
/// The redis user repository.
/// </summary>

public class RedisRepository<T> : IGenericRepository<T> where T : class
{
    private readonly IDatabase _database;


    /// <summary>
    /// Initializes a new instance of the <see /> class.
    /// </summary>
    /// <param name="connectionMultiplexer">The connection multiplexer.</param>
    /// <param name="databaseIndex">The database index.</param>
    public RedisRepository(IConnectionMultiplexer connectionMultiplexer, int databaseIndex)
    {
        _database = connectionMultiplexer.GetDatabase(databaseIndex);
    }

    /// <summary>
    /// Gets the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task<T?> GetItemAsync(string id, CancellationToken cancellationToken)
    {
        var data = await _database.StringGetAsync(id);
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<T>(data);
    }

    /// <summary>
    /// Gets the items async.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Cannot perform complex queries on Redis.");
    }

    /// <summary>
    /// Adds the item async.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task<T> AddItemAsync(T item, string id, CancellationToken cancellationToken)
    {
        if (item is ValidUser user)
        {
            var data = JsonSerializer.Serialize(item);
            await _database.StringSetAsync(id, data);
        }
        else
        {
            throw new InvalidOperationException("Item must be of type ValidUser");
        }
        return item;

    }

    /// <summary>
    /// Updates the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task UpdateItemAsync(string id, T item, CancellationToken cancellationToken)
    {
        if (item is ValidUser user && user.userId == id)
        {
            var data = JsonSerializer.Serialize(item);
            await _database.StringSetAsync(id, data);
        }
        else
        {
            throw new InvalidOperationException("Item must be of type ValidUser and ID must match");
        }
    }

    /// <summary>
    /// Deletes the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task DeleteItemAsync(string id, CancellationToken cancellationToken)
    {
        await _database.KeyDeleteAsync(id);
    }
}
