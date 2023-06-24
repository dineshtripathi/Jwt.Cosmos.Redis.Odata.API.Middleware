using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace JWTClaimsExtractor.Services;
/// <summary>
/// The no op token validator.
/// </summary>

public class NoOpTokenValidator : ISecurityTokenValidator
{
    /// <summary>
    /// Cans the read token.
    /// </summary>
    /// <param name="securityToken">The security token.</param>
    /// <returns>A bool.</returns>
    public bool CanReadToken(string securityToken)
    {
        return true; // All tokens are considered valid
    }

    /// <summary>
    /// Validates the token.
    /// </summary>
    /// <param name="securityToken">The security token.</param>
    /// <param name="validationParameters">The validation parameters.</param>
    /// <param name="validatedToken">The validated token.</param>
    /// <returns>A ClaimsPrincipal.</returns>
    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken? validatedToken)
    {
        validatedToken = null;
        var identity = new ClaimsIdentity();
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Gets a value indicating whether can validate token.
    /// </summary>
    public bool CanValidateToken => true;

    /// <summary>
    /// Gets or sets the maximum token size in bytes.
    /// </summary>
    public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
}