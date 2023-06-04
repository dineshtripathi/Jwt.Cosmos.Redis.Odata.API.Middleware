using JWTClaimsExtractor.Constant;
using Microsoft.AspNetCore.Http;

namespace JWTClaimsExtractor.Jwt;

public class JwtTokenExtractor : IJwtTokenExtractor
{
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