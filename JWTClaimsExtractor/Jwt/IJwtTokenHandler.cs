using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Services;

namespace JWTClaimsExtractor.Jwt;

public interface IJwtTokenHandler
{
    /// <summary>
    /// Handles the token async.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="client">The client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    Task<CustomUser> HandleTokenAsync(string token, DataCenterValidUserEndpointClient client,
        CancellationToken cancellationToken);
}