using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JWT.Bearer.Claims.Auth.Extractor.Claims;
using JWT.Bearer.Claims.Auth.Extractor.Jwt;
using JWT.Bearer.Claims.Auth.Extractor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;

namespace JWT.Bearer.Claims.Auth.Extractor;
/// <summary>
/// The jwt bearer options factory.
/// </summary>

public class JwtBearerOptionsFactory
{
    private readonly ITokenValidator _tokenValidator;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly DataCenterValidUserEndpointClient _authorizedAccountEndpointClient;
    private readonly JwtClaimsExtractor _jwtClaimsExtractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtBearerOptionsFactory"/> class.
    /// </summary>
    /// <param name="tokenValidator">The token validator.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="jwtTokenHandler">The jwt token handler.</param>
    /// <param name="authorizedAccountEndpointClient">The authorized account endpoint client.</param>
    /// <param name="jwtClaimsExtractor">The jwt claims extractor.</param>
    public JwtBearerOptionsFactory(
        ITokenValidator tokenValidator,
        ILoggerFactory loggerFactory,
        IJwtTokenHandler jwtTokenHandler,
        DataCenterValidUserEndpointClient authorizedAccountEndpointClient,
        JwtClaimsExtractor jwtClaimsExtractor)
    {
        _tokenValidator = tokenValidator;
        _loggerFactory = loggerFactory;
        _jwtTokenHandler = jwtTokenHandler;
        _authorizedAccountEndpointClient = authorizedAccountEndpointClient;
        _jwtClaimsExtractor = jwtClaimsExtractor;
    }

    /// <summary>
    /// Creates the.
    /// </summary>
    /// <param name="tokenValidationEnabled">If true, token validation enabled.</param>
    /// <returns>A JwtBearerOptions.</returns>
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