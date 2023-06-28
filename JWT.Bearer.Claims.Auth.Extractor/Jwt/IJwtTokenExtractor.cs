using Microsoft.AspNetCore.Http;

namespace JWTClaimsExtractor.Jwt;

public interface IJwtTokenExtractor
{
    /// <summary>
    /// Tries the extract token.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="token">The token.</param>
    /// <returns>A bool.</returns>
    bool TryExtractToken(HttpContext context, out string? token);
}