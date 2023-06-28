using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTClaimsExtractor.Jwt;
/// <summary>
/// The token validator.
/// </summary>

public class TokenValidator : ITokenValidator
{
    private readonly JwtTokenConfiguration _tokenConfigOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidator"/> class.
    /// </summary>
    /// <param name="tokenConfigOptions">The token config options.</param>
    public TokenValidator(IOptions<JwtTokenConfiguration> tokenConfigOptions)
    {
        _tokenConfigOptions = tokenConfigOptions.Value;
    }

    /// <summary>
    /// Validates the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="validationParameters">The validation parameters.</param>
    /// <returns>A ClaimsPrincipal.</returns>
    public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ValidateToken(token, validationParameters, out _);
    }

    /// <summary>
    /// Gets the validation parameters.
    /// </summary>
    /// <returns>A TokenValidationParameters.</returns>
    public TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = _tokenConfigOptions.ValidateIssuer,
            ValidateAudience = _tokenConfigOptions.ValidateAudience,
            ValidateLifetime = _tokenConfigOptions.ValidateLifetime,
            ValidateIssuerSigningKey = _tokenConfigOptions.ValidateIssuerSigningKey,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = _tokenConfigOptions.ValidIssuer,
            ValidAudience = _tokenConfigOptions.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfigOptions.IssuerSigningKey))
        };
    }

    /// <summary>
    /// Validates the token.
    /// </summary>
    /// <param name="token">The token.</param>
    public void ValidateToken(string token)
    {
        ValidateToken(token, GetValidationParameters());
    }
}
