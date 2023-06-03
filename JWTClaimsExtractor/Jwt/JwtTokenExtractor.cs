using Microsoft.AspNetCore.Http;

namespace JWTClaimsExtractor.Jwt;

public class JwtTokenExtractor : IJwtTokenExtractor
{
    public bool TryExtractToken(HttpContext context, out string token)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].ToString();
        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //context.Response.WriteAsync("Missing or incorrect authorization header");
            //token = null;
            //return false;

            throw new Exception("Missing or incorrect authorization header");


        }

        // Extract the token from the Authorization header
        token = authorizationHeader["Bearer ".Length..].Trim();
        return true;
    }
}