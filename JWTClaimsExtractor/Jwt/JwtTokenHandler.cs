using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Services;

namespace JWTClaimsExtractor.Jwt;

public class JwtTokenHandler : IJwtTokenHandler
{
    private readonly JwtParser _jwtParser;
    private readonly JwtClaimsExtractor _claimsExtractor;

    public JwtTokenHandler(JwtParser jwtParser, JwtClaimsExtractor claimsExtractor)
    {
        _jwtParser = jwtParser;
        _claimsExtractor = claimsExtractor;
    }

    public async Task<CustomUser> HandleTokenAsync(string token, AuthorizedAccountEndpointClient client, CancellationToken cancellationToken)
    {
        var jwt = _jwtParser.Parse(token);
        return await _claimsExtractor.ExtractAsync(jwt, client, cancellationToken);
    }
}