using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Bearer.Claims.Auth.Extractor.Jwt;

public interface ITokenValidator
{
    /// <summary>
    /// Validates the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="validationParameters">The validation parameters.</param>
    /// <returns>A ClaimsPrincipal.</returns>
    ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters);
    /// <summary>
    /// Validates the token.
    /// </summary>
    /// <param name="token">The token.</param>
    void ValidateToken(string token);
    /// <summary>
    /// Gets the validation parameters.
    /// </summary>
    /// <returns>A TokenValidationParameters.</returns>
    TokenValidationParameters GetValidationParameters();
}