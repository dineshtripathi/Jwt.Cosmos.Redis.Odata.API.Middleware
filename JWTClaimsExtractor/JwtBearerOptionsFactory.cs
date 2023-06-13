using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Jwt;
using JWTClaimsExtractor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;

namespace JWTClaimsExtractor;

public class JwtBearerOptionsFactory
{
    private readonly ITokenValidator _tokenValidator;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly AuthorizedAccountEndpointClient _authorizedAccountEndpointClient;
    private readonly JwtClaimsExtractor _jwtClaimsExtractor;

    public JwtBearerOptionsFactory(
        ITokenValidator tokenValidator,
        ILoggerFactory loggerFactory,
        IJwtTokenHandler jwtTokenHandler,
        AuthorizedAccountEndpointClient authorizedAccountEndpointClient,
        JwtClaimsExtractor jwtClaimsExtractor)
    {
        _tokenValidator = tokenValidator;
        _loggerFactory = loggerFactory;
        _jwtTokenHandler = jwtTokenHandler;
        _authorizedAccountEndpointClient = authorizedAccountEndpointClient;
        _jwtClaimsExtractor = jwtClaimsExtractor;
    }

    public JwtBearerOptions Create(bool tokenValidationEnabled)
    {
        var options = new JwtBearerOptions
        {
            TokenValidationParameters = _tokenValidator.GetValidationParameters(),
            Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = _loggerFactory.CreateLogger("JwtClaimsExtractorServices");
                    logger.LogError(context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    if (context.SecurityToken is not JwtSecurityToken token)
                    {
                        context.Fail("Received token is not a valid JWT");
                        return;
                    }
                    CustomUser customUser;
                    if (tokenValidationEnabled)
                    {
                        customUser = await _jwtTokenHandler.HandleTokenAsync(
                            token.RawData,
                            _authorizedAccountEndpointClient,
                            context.HttpContext.RequestAborted);
                    }
                    else
                    {
                        customUser = await _jwtClaimsExtractor.ExtractAsync(
                            token,
                            _authorizedAccountEndpointClient,
                            context.HttpContext.RequestAborted);
                    }

                    var identity = _jwtClaimsExtractor.GetIdentityFromUser(customUser, context.Scheme.Name);
                    context.Principal = new ClaimsPrincipal(identity);
                    context.Success();
                }
            }
        };

        return options;
    }
}