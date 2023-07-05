using Microsoft.AspNetCore.Http;

namespace JWT.Bearer.Claims.Auth.Extractor.Jwt;

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