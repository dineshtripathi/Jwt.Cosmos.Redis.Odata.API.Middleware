using JwtValidUserAPI.Core.DTO;

namespace JwtValidUserAPI.Core.Repository;
/// <summary>
/// The cosmos user repository.
/// </summary>

public interface ICosmosUserRepository
{
    /// <summary>
    /// Gets the by id async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    Task<ValidUser> GetByIdAsync(string id,CancellationToken cancellationToken);

    /// <summary>
    /// Creates the async.
    /// </summary>
    /// <param name="validUser">The valid user.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task.</returns>
    Task CreateAsync(ValidUser validUser,CancellationToken cancellationToken);
}