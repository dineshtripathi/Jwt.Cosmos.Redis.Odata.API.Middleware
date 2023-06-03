using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using JWTClaimsExtractor.Jwt;
using JWTClaimsExtractor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JWTClaimsExtractor;

public static class JwtClaimsExtractorServices
{
    public static IServiceCollection AddJwtClaimsExtractorServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthorizedAccountEndpointOptions>(configuration.GetSection("AuthorizedAccountEndpoint"));
        services.AddTransient<AuthorizedAccountEndpointClient>();
        services.AddTransient<JwtClaims>();
        services.AddTransient<JwtClaimsExtractor>();
        services.AddTransient<JwtParser>();
        services.AddTransient<IJwtTokenExtractor, JwtTokenExtractor>();
        services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();

        return services;
    }
}