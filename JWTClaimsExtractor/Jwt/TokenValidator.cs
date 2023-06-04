using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTClaimsExtractor.Jwt;

public class TokenValidator : ITokenValidator
{
    private readonly JwtTokenConfiguration _tokenConfigOptions;

    public TokenValidator(IOptions<JwtTokenConfiguration> tokenConfigOptions)
    {
        _tokenConfigOptions = tokenConfigOptions.Value;
    }
    public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ValidateToken(token, validationParameters, out _);
    }

    public void ValidateToken(string token)
    {
        var validationParameters = new TokenValidationParameters
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

        ValidateToken(token, validationParameters);
    }
}