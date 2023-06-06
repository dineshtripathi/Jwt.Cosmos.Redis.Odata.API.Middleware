using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using JWTClaimsExtractor.Jwt;
using JWTClaimsExtractor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTClaimsExtractor;

public static class JwtClaimsExtractorServices
{
    public static IServiceCollection AddJwtClaimsExtractorServices(this IServiceCollection services,
        IConfiguration configuration)
    {
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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtConfig = services.BuildServiceProvider().GetRequiredService<IOptions<JwtTokenConfiguration>>().Value;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = false,
                    ValidateAudience = false,
                    ValidateLifetime = false, 
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.IssuerSigningKey))

                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // Log the error message or view it in debug mode
                        Console.WriteLine(context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var token = context.SecurityToken as JwtSecurityToken;
                        if (token == null)
                        {
                            context.Fail("Received token is not a valid JWT");
                            return;
                        }
                        var jwtTokenHandler = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenHandler>();
                        var authorizedAccountEndpointClient = context.HttpContext.RequestServices.GetRequiredService<AuthorizedAccountEndpointClient>();
                        var tokenString = token.RawData;


                        var user = await jwtTokenHandler.HandleTokenAsync(tokenString, authorizedAccountEndpointClient,
                            context.HttpContext.RequestAborted);

                        // Add all claims to the identity
                        // Add all claims to the identity
                        var identity = (ClaimsIdentity)context.Principal.Identity;
                        var userProperties = user.GetType().GetProperties();

                        foreach (var property in userProperties)
                        {
                            // Skip the AdditionalClaims and AuthorizedAccounts properties here as we will handle them separately
                            if (property.Name != nameof(CustomUser.AdditionalClaims) )//&& property.Name != nameof(CustomUser.AuthorizedAccounts))
                            {
                                var claimType = property.Name;
                                var claimValue = property.GetValue(user)?.ToString() ?? string.Empty;
                                identity.AddClaim(new Claim(claimType, claimValue));
                            }
                        }

                        // Add additional claims
                        if (user.AdditionalClaims != null)
                        {
                            foreach (var claim in user.AdditionalClaims)
                            {
                                var claimValue = claim.Value != null ? JsonSerializer.Serialize(claim.Value) : string.Empty;

                                identity.AddClaim(new Claim(claim.Key, claimValue));
                            }
                        }

                        // Add AuthorizedAccounts as a claim
                        //if (user.AuthorizedAccounts != null)
                        //{
                        //    var authorizedAccountsClaim = new Claim("AuthorizedAccounts", JsonSerializer.Serialize(user.AuthorizedAccounts));
                        //    identity.AddClaim(authorizedAccountsClaim);
                        //}

                        var authorisedUserClaim = new Claim("AuthorisedUser", JsonSerializer.Serialize(user));
                        identity.AddClaim(authorisedUserClaim);

                        // Update the HttpContext user with the modified identity
                        context.HttpContext.User = new ClaimsPrincipal(identity);


                    }
                };
            });
        return services;
    }
}