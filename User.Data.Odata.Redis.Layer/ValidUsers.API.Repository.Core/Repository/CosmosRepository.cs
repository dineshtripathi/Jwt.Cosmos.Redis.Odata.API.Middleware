using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;

namespace ValidUsers.API.Repository.Core.Repository;

/// <summary>
/// The cosmos repository.
/// </summary>

public class CosmosRepository<T> : IGenericRepository<T> where T : class
{
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the <see /> class.
    /// </summary>
    /// <param name="dbClient">The db client.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="containerName">The container name.</param>
    public CosmosRepository(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        this._container = dbClient.GetDatabase(databaseName).GetContainer(containerName);
    }

    /// <summary>
    /// Gets the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task<T?> GetItemAsync(string id,CancellationToken cancellationToken)
    {
            {
                ItemResponse<T?> response = await _container.ReadItemAsync<T?>(id, new PartitionKey(id), cancellationToken: cancellationToken);
                return response.Resource;
            }
    }

    /// <summary>
    /// Gets the items async.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate,CancellationToken cancellationToken)
    {
        var query = this._container.GetItemQueryIterator<T>(new QueryDefinition(
            $"SELECT * FROM c WHERE {predicate}"));

        var results = new List<T>
        {
            Capacity = 0
        };
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync(cancellationToken);

            results.AddRange(response.ToList());
        }

        return results;
    }

    /// <summary>
    /// Adds the item async.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task<T> AddItemAsync(T item, string id, CancellationToken cancellationToken)
    {
        var response = await _container.CreateItemAsync<T>(item, new PartitionKey(id), cancellationToken: cancellationToken);
        return response.Resource;
    }


    /// <summary>
    /// Updates the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task UpdateItemAsync(string id, T item,CancellationToken cancellationToken)
    {
        await _container.UpsertItemAsync<T?>(item, new PartitionKey(id), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes the item async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    public async Task DeleteItemAsync(string id,CancellationToken cancellationToken)
    {
        await _container.DeleteItemAsync<T?>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}