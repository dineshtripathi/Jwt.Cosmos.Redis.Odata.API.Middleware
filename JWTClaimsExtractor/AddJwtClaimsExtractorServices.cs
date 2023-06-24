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
/// <summary>
/// The jwt claims extractor services.
/// </summary>

public static class JwtClaimsExtractorServices
{
    /// <summary>
    /// Adds the jwt claims extractor services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An IServiceCollection.</returns>
    public static IServiceCollection AddJwtClaimsExtractorServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var validUserEndpointOptions = new ValidUserEndpointOptions();
        configuration.GetSection("ValidUserEndpoint").Bind(validUserEndpointOptions);

        var jwtTokenConfiguration = new JwtTokenConfiguration();
        configuration.GetSection("JwtTokenConfiguration").Bind(jwtTokenConfiguration);

        services.Configure<ValidUserEndpointOptions>(configuration.GetSection("ValidUserEndpoint"));
        services.Configure<JwtTokenConfiguration>(configuration.GetSection("JwtTokenConfiguration"));
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddTransient<DataCenterValidUserEndpointClient>();
     //   services.AddTransient<JwtClaims>();
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
        var tokenValidationEnabled = bool.Parse(configuration["ValidatedJwtToken"] ?? string.Empty);
        
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
                if (tokenValidationEnabled)
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var jwtClaimsExtractor =
                                context.HttpContext.RequestServices.GetRequiredService<JwtClaimsExtractor>();
                            var authorizedAccountEndpointClient = context.HttpContext.RequestServices
                                .GetRequiredService<DataCenterValidUserEndpointClient>();

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
                        },
                        OnMessageReceived = context =>
                        {
                            var jwtTokenHandler = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenHandler>();
                            var authorizedAccountEndpointClient = context.HttpContext.RequestServices.GetRequiredService<DataCenterValidUserEndpointClient>();
                            var tokenValidator = context.HttpContext.RequestServices.GetRequiredService<ITokenValidator>();

                            var token = context.Token;

                            try
                            {
                                if (token != null)
                                {
                                    tokenValidator.ValidateToken(token);
                                    var customUser = jwtTokenHandler.HandleTokenAsync(token,
                                        authorizedAccountEndpointClient, context.HttpContext.RequestAborted).Result;

                                    var jwtClaimsExtractor = context.HttpContext.RequestServices
                                        .GetRequiredService<JwtClaimsExtractor>();
                                    var identity =
                                        jwtClaimsExtractor.GetIdentityFromUser(customUser, context.Scheme.Name);

                                    context.Principal = new ClaimsPrincipal(identity);
                                }

                                context.Success();
                            }
                            catch (Exception)
                            {
                                // handle validation failure
                            }

                            return Task.CompletedTask;
                        }
                    };
                }
                else
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var jwtTokenHandler = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenHandler>();
                            var authorizedAccountEndpointClient = context.HttpContext.RequestServices.GetRequiredService<DataCenterValidUserEndpointClient>();
                            var tokenValidator = context.HttpContext.RequestServices.GetRequiredService<ITokenValidator>();

                            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                            if (!string.IsNullOrEmpty(authorizationHeader))
                            {
                                context.Token = authorizationHeader.Replace("Bearer ", string.Empty);
                            }

                            if (context.Token != null)
                            {
                                tokenValidator.ValidateToken(context.Token);
                                var customUser = jwtTokenHandler.HandleTokenAsync(context.Token,
                                    authorizedAccountEndpointClient, context.HttpContext.RequestAborted).Result;

                                var jwtClaimsExtractor = context.HttpContext.RequestServices
                                    .GetRequiredService<JwtClaimsExtractor>();
                                var identity = jwtClaimsExtractor.GetIdentityFromUser(customUser, context.Scheme.Name);

                                context.Principal = new ClaimsPrincipal(identity);
                            }

                            context.Success();
                            return Task.CompletedTask;
                        }
                    };
                }
                
            });


        return services;
    }
}