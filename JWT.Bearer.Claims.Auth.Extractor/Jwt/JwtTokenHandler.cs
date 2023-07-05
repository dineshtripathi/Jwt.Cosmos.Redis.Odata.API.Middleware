using JWT.Bearer.Claims.Auth.Extractor.Claims;
using JWT.Bearer.Claims.Auth.Extractor.Services;

namespace JWT.Bearer.Claims.Auth.Extractor.Jwt;
/// <summary>
/// The jwt token handler.
/// </summary>

public class JwtTokenHandler : IJwtTokenHandler
{
    private readonly JwtClaimsExtractor _claimsExtractor;
    private readonly JwtParser _jwtParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenHandler"/> class.
    /// </summary>
    /// <param name="jwtParser">The jwt parser.</param>
    /// <param name="claimsExtractor">The claims extractor.</param>
    public JwtTokenHandler(JwtParser jwtParser, JwtClaimsExtractor claimsExtractor)
    {
        _jwtParser = jwtParser;
        _claimsExtractor = claimsExtractor;
    }

    /// <summary>
    /// Handles the token async.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="client">The client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    public async Task<CustomUser> HandleTokenAsync(string token, DataCenterValidUserEndpointClient client,
        CancellationToken cancellationToken)
    {
        var jwt = _jwtParser.Parse(token);
        return await _claimsExtractor.ExtractAsync(jwt, client, cancellationToken);
    }
}