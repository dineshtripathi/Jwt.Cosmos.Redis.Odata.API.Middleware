using JWTClaimsExtractor.Jwt;
using JWTClaimsExtractor.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Abstractions;

namespace JWTClaimsExtractor.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtTokenExtractor _jwtTokenExtractor;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly string  claimName;
    public JwtMiddleware(RequestDelegate next, IOptions<AuthorizedAccountEndpointOptions> options, IJwtTokenExtractor jwtTokenExtractor, IJwtTokenHandler jwtTokenHandler,ILogger<JwtMiddleware> logger)
    {
        _next = next;
       
        _jwtTokenExtractor = jwtTokenExtractor;
        _jwtTokenHandler = jwtTokenHandler;
        claimName = string.IsNullOrWhiteSpace(options?.Value?.CustomClaimName) ? "SseUser" : options.Value.CustomClaimName;
    }

    public async Task Invoke(HttpContext context, AuthorizedAccountEndpointClient authorizedAccountEndpointClient)
    {

        // Extract the token from the Authorization header
        if (!_jwtTokenExtractor.TryExtractToken(context, out var token))
        {
            // Handle the error - the request is not correctly authenticated
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var user = await _jwtTokenHandler.HandleTokenAsync(token, authorizedAccountEndpointClient, context.RequestAborted);
        SetUserInContext(context, claimName, user);

        await _next(context);
    }

    private static void SetUserInContext(HttpContext context, string claimName, CustomUser user)
    {
        var claims = new List<Claim>
        {
            new(claimName, JsonSerializer.Serialize(user))
        };

        var identity = new ClaimsIdentity(claims);
        context.User = new ClaimsPrincipal(identity);
        context.Items[claimName] = new ClaimsPrincipal(identity);
    }

}
