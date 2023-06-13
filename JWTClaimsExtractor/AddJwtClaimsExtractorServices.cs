using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using JWTClaimsExtractor.Jwt;
using JWTClaimsExtractor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JWTClaimsExtractor;

public static class JwtClaimsExtractorServices
{
    public static IServiceCollection AddJwtClaimsExtractorServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var authorizedEndpointOptions = new AuthorizedAccountEndpointOptions();
        configuration.GetSection("AuthorizedAccountEndpoint").Bind(authorizedEndpointOptions);

        var jwtTokenConfiguration = new JwtTokenConfiguration();
        configuration.GetSection("JwtTokenConfiguration").Bind(jwtTokenConfiguration);

        services.Configure<AuthorizedAccountEndpointOptions>(configuration.GetSection("AuthorizedAccountEndpoint"));
        services.Configure<JwtTokenConfiguration>(configuration.GetSection("JwtTokenConfiguration"));
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddTransient<AuthorizedAccountEndpointClient>();
        services.AddTransient<JwtClaims>();
        services.AddTransient<JwtClaimsExtractor>();
        services.AddTransient<JwtParser>();
        services.AddTransient<IJwtTokenExtractor, JwtTokenExtractor>();
        services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
        services.AddTransient<ITokenValidator, TokenValidator>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        //  services.AddTransient<ISecurityTokenValidator, NoOpTokenValidator>();

        services.AddLogging(builder =>
        {
            builder.AddConsole(); // Use the console logging provider
        });

        // Configure the desired logging levels
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddFilter("Microsoft.AspNetCore.Authentication.JwtBearer", LogLevel.Debug)
                .AddConsole(); // Use the console logging provider
        });
        var tokenValidationEnabled = bool.Parse(configuration["ValidatedJwtToken"]);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtTokenConfiguration.ValidateIssuer,
                    ValidateIssuerSigningKey = jwtTokenConfiguration.ValidateIssuerSigningKey,
                    ValidateAudience = jwtTokenConfiguration.ValidateAudience,
                    ValidateLifetime = jwtTokenConfiguration.ValidateLifetime,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfiguration.IssuerSigningKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jwtClaimsExtractor =
                            context.HttpContext.RequestServices.GetRequiredService<JwtClaimsExtractor>();
                        var authorizedAccountEndpointClient = context.HttpContext.RequestServices
                            .GetRequiredService<AuthorizedAccountEndpointClient>();

                        if (context.SecurityToken is not JwtSecurityToken token)
                        {
                            context.Fail("Received token is not a valid JWT");
                            return;
                        }


                        CustomUser customUser;
                        if (tokenValidationEnabled)
                        {
                            var jwtTokenHandler =
                                context.HttpContext.RequestServices.GetRequiredService<IJwtTokenHandler>();
                            customUser = await jwtTokenHandler.HandleTokenAsync(token.RawData,
                                authorizedAccountEndpointClient, context.HttpContext.RequestAborted);
                        }
                        else
                        {
                            customUser = await jwtClaimsExtractor.ExtractAsync(token, authorizedAccountEndpointClient,
                                context.HttpContext.RequestAborted);
                        }

                        var identity = jwtClaimsExtractor.GetIdentityFromUser(customUser, context.Scheme.Name);
                        context.Principal = new ClaimsPrincipal(identity);
                        context.Success();
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger("JwtClaimsExtractorServices");
                        logger.LogError(context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });


        return services;
    }
}