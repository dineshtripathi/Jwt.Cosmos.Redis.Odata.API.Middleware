using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Services;

namespace JWTClaimsExtractor.Jwt;

public interface IJwtTokenHandler
{
    Task<CustomUser> HandleTokenAsync(string token, AuthorizedAccountEndpointClient client,
        CancellationToken cancellationToken);
}