using JWTClaimsExtractor.Middleware;
using Microsoft.AspNetCore.Builder;

namespace JWTClaimsExtractor;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseJwtTokenClaimsMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtBearerTokenMiddleware>();
    }

    public static IApplicationBuilder UseJwtExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtExceptionMiddleware>();
    }
}