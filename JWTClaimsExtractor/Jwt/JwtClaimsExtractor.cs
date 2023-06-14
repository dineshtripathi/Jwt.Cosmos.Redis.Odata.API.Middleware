using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Services;

namespace JWTClaimsExtractor.Jwt;

public class JwtClaimsExtractor
{
    public async Task<CustomUser> ExtractAsync(JwtSecurityToken? jwt, AuthorizedAccountEndpointClient orgService, CancellationToken cancellationToken)
    {
        if (jwt == null)
            throw new ArgumentNullException(nameof(jwt));

        var claims = jwt.Claims.ToList();

        if (!claims.Any())
            throw new Exception("No claims found in the JWT");

        var additionalClaims = await BuildAdditionalClaims(claims, orgService, cancellationToken);

        return new CustomUser(additionalClaims);
    }

    private static async Task<Dictionary<string, object>> BuildAdditionalClaims(IEnumerable<Claim> claims, AuthorizedAccountEndpointClient orgService, CancellationToken cancellationToken)
    {
        var additionalClaims = new Dictionary<string, object>();
        var orgId = GetClaimValue(claims, CustomClaimTypes.OrgId);
        var accounts = await orgService.GetAuthorizedAccounts(orgId, cancellationToken) ?? new List<AuthorizedAccount>();

        additionalClaims["AuthorizedUserAccounts"] = accounts;

        foreach (var claim in claims)
        {
            additionalClaims[claim.Type] = claim.Value;
        }

        return additionalClaims;
    }
    public ClaimsIdentity GetIdentityFromUser(CustomUser user, string schemeName)
    {
        var identity = new ClaimsIdentity(schemeName);

        foreach (var claim in user.JwtCustomClaims)
        {
            var claimValue = claim.Value != null ? JsonSerializer.Serialize(claim.Value) : string.Empty;
            identity.AddClaim(new Claim(claim.Key, claimValue));
        }

        var authorisedUserClaim = new Claim("AuthorisedUser", JsonSerializer.Serialize(user));
        identity.AddClaim(authorisedUserClaim);

        return identity;
    }
    private static string? GetClaimValue(IEnumerable<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

}
