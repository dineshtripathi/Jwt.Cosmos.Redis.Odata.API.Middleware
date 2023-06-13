using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace JWTClaimsExtractor.Services;

public class NoOpTokenValidator : ISecurityTokenValidator
{
    public bool CanReadToken(string securityToken)
    {
        return true; // All tokens are considered valid
    }

    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        validatedToken = null;
        var identity = new ClaimsIdentity();
        return new ClaimsPrincipal(identity);
    }

    public bool CanValidateToken => true;

    public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
}