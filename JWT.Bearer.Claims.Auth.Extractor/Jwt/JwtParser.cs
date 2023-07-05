using System.IdentityModel.Tokens.Jwt;

namespace JWT.Bearer.Claims.Auth.Extractor.Jwt;
/// <summary>
/// The jwt parser.
/// </summary>

public class JwtParser
{
    /// <summary>
    /// Parses the.
    /// </summary>
    /// <param name="jwt">The jwt.</param>
    /// <returns>A JwtSecurityToken? .</returns>
    public JwtSecurityToken? Parse(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwt);
        return jsonToken as JwtSecurityToken;
    }
}