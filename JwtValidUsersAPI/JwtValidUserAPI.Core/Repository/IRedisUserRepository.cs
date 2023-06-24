using JwtValidUserAPI.Core.DTO;

namespace JwtValidUserAPI.Core.Repository;
/// <summary>
/// The redis user repository.
/// </summary>

public interface IRedisUserRepository
{
    /// <summary>
    /// Gets the async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    Task<ValidUser> GetAsync(string id, CancellationToken cancellationToken);
    /// <summary>
    /// Sets the async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    Task SetAsync(string id, ValidUser user,CancellationToken cancellationToken);
}