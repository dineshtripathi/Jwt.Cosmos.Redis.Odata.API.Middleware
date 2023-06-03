using JWTClaimsExtractor.Middleware;
using Microsoft.AspNetCore.Builder;

namespace JWTClaimsExtractor;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
}
