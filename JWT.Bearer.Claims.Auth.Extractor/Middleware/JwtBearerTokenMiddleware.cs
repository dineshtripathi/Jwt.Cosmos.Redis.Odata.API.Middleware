using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Json;
using JWT.Bearer.Claims.Auth.Extractor.Claims;
using JWT.Bearer.Claims.Auth.Extractor.ConfigSection;
using JWT.Bearer.Claims.Auth.Extractor.Jwt;
using JWT.Bearer.Claims.Auth.Extractor.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JWT.Bearer.Claims.Auth.Extractor.Middleware;

/// <summary>
///     The jwt bearer token middleware.
/// </summary>
public class JwtBearerTokenMiddleware
{
    private readonly AppSettings _appSettingsOptions;
    private readonly string _claimName;
    private readonly ITokenValidator _jwTokenValidator;
    private readonly IJwtTokenExtractor _jwtTokenExtractor;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JwtBearerTokenMiddleware" /> class.
    /// </summary>
    /// <param name="next">The next.</param>
    /// <param name="authorizedEndpointOptions">The authorized endpoint options.</param>
    /// <param name="appSettingsOptions">The app settings options.</param>
    /// <param name="jwtTokenExtractor">The jwt token extractor.</param>
    /// <param name="jwtTokenHandler">The jwt token handler.</param>
    /// <param name="jwTokenValidator">The jw token validator.</param>
    /// <param name="logger">The logger.</param>
    public JwtBearerTokenMiddleware(RequestDelegate next,
        IOptions<ValidUserEndpointOptions> authorizedEndpointOptions,
        IOptions<AppSettings> appSettingsOptions, IJwtTokenExtractor jwtTokenExtractor,
        IJwtTokenHandler jwtTokenHandler, ITokenValidator jwTokenValidator, ILogger<JwtBearerTokenMiddleware> logger)
    {
        _next = next;
        _appSettingsOptions = appSettingsOptions.Value;
        _jwtTokenExtractor = jwtTokenExtractor;
        _jwtTokenHandler = jwtTokenHandler;
        _jwTokenValidator = jwTokenValidator;
        _claimName = string.IsNullOrWhiteSpace(authorizedEndpointOptions?.Value?.CustomClaimName)
            ? "ClaimsUser"
            : authorizedEndpointOptions.Value.CustomClaimName;
    }

    /// <summary>
    ///     Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="authorizedAccountEndpointClient">The authorized account endpoint client.</param>
    /// <returns>A Task.</returns>
    public async Task Invoke(HttpContext context, DataCenterValidUserEndpointClient authorizedAccountEndpointClient)
    {
        // Extract the token from the Authorization header
        if (!_jwtTokenExtractor.TryExtractToken(context, out var token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {error = "Missing or incorrect authorization header"}));
            return;
        }

        try
        {
            if (token != null)
            {
                if (_appSettingsOptions.ValidatedJwtToken)
                    _jwTokenValidator.ValidateToken(token);

                var user = await _jwtTokenHandler.HandleTokenAsync(token, authorizedAccountEndpointClient,
                    context.RequestAborted);
                AddUserClaimToContext(context, _claimName, user);
            }

            await _next(context);
        }
        catch (Exception exception)
        {
            throw new AuthenticationException("Invalid Token passed", exception);
        }
    }

    /// <summary>
    ///     Adds the user claim to context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="claimName">The claim name.</param>
    /// <param name="user">The user.</param>
    internal static void AddUserClaimToContext(HttpContext context, string claimName, CustomUser user)
    {
        var claims = new List<Claim>
        {
            new(claimName, JsonSerializer.Serialize(user))
        };

        if (context.User.Identity is ClaimsIdentity {IsAuthenticated: true} identity)
            identity.AddClaims(claims);
    }
}