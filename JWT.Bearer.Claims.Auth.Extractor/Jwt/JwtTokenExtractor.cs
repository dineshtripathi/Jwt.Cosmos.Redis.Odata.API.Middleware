using JWTClaimsExtractor.Constant;
using Microsoft.AspNetCore.Http;

namespace JWTClaimsExtractor.Jwt;
/// <summary>
/// The jwt token extractor.
/// </summary>

public class JwtTokenExtractor : IJwtTokenExtractor
{
    /// <summary>
    /// Tries the extract token.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="token">The token.</param>
    /// <returns>A bool.</returns>
    public bool TryExtractToken(HttpContext context, out string? token)
    {
        var authorizationHeader = context.Request.Headers[AuthorizationHeaderConstant.Authorization].ToString();
        if (!authorizationHeader.StartsWith($"{AuthorizationHeaderConstant.Bearer} ",
                StringComparison.OrdinalIgnoreCase))
        {
            token = null;
            return false;
        }

        token = authorizationHeader[$"{AuthorizationHeaderConstant.Bearer} ".Length..].Trim();
        return true;
    }
}