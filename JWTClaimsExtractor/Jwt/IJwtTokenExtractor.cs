using Microsoft.AspNetCore.Http;

namespace JWTClaimsExtractor.Jwt;

public interface IJwtTokenExtractor
{
    bool TryExtractToken(HttpContext context, out string token);
}