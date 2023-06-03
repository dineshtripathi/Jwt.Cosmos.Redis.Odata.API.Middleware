using System.IdentityModel.Tokens.Jwt;

namespace JWTClaimsExtractor.Jwt;

public class JwtParser
{
    public JwtSecurityToken? Parse(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwt);
        return jsonToken as JwtSecurityToken;
    }
}