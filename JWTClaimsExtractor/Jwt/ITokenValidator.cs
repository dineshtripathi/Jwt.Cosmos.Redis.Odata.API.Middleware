using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace JWTClaimsExtractor.Jwt;

public interface ITokenValidator
{
    ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters);
    void ValidateToken(string token);
}