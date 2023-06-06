using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Services;
using Newtonsoft.Json;

namespace JWTClaimsExtractor.Jwt;

public class JwtClaimsExtractor
{
    public async Task<CustomUser> ExtractAsync(JwtSecurityToken? jwt, AuthorizedAccountEndpointClient orgService,
        CancellationToken cancellationToken)
    {
        if (jwt == null)
            throw new ArgumentNullException(nameof(jwt));

        var claims = jwt.Claims.ToList();

        if (!claims.Any())
            throw new Exception("No claims found in the JWT");

        var orgId = GetClaimValue(claims, CustomClaimTypes.OrgId);
        var accounts = await orgService.GetAuthorizedAccounts(orgId, cancellationToken) ??
                       new List<AuthorizedAccount>();
        var additionalClaims = new Dictionary<string, string>();
        additionalClaims["AuthorizedUserAccounts"]=JsonConvert.SerializeObject(accounts);
        foreach (var claim in claims)
        {
           // if (claim.Type != CustomClaimTypes.OrgId && claim.Type != CustomClaimTypes.CorrelationId && claim.Type != CustomClaimTypes.Email && claim.Type != CustomClaimTypes.Sub && claim.Type != CustomClaimTypes.GivenName && claim.Type != CustomClaimTypes.FamilyName && claim.Type != CustomClaimTypes.OrgRole && claim.Type != CustomClaimTypes.Name && claim.Type != CustomClaimTypes.IsTpiConsultancy && claim.Type != CustomClaimTypes.OrgName)
            {
                additionalClaims[claim.Type] = claim.Value;
            }
        }

        return new CustomUser
        (
            accounts,
            //ParseGuidClaim(claims, CustomClaimTypes.CorrelationId),
            //GetClaimValue(claims, CustomClaimTypes.Email),
            //ParseGuidClaim(claims, CustomClaimTypes.Sub),
            //GetClaimValue(claims, CustomClaimTypes.GivenName),
            //GetClaimValue(claims, CustomClaimTypes.FamilyName),
            //ParseGuidClaim(claims, CustomClaimTypes.OrgId),
            //GetClaimValue(claims, CustomClaimTypes.OrgRole),
            //GetClaimValue(claims, CustomClaimTypes.Name),
            //ParseBoolClaim(claims, CustomClaimTypes.IsTpiConsultancy),
            //GetClaimValue(claims, CustomClaimTypes.OrgName),
            additionalClaims
        );
    }

    private static string? GetClaimValue(IEnumerable<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    private static Guid ParseGuidClaim(IEnumerable<Claim> claims, string claimType)
    {
        var claimValue = GetClaimValue(claims, claimType);
        if (Guid.TryParse(claimValue, out var guidValue))
            return guidValue;

        throw new Exception($"Claim {claimType} could not be parsed to a Guid.");
    }

    private static bool ParseBoolClaim(IEnumerable<Claim> claims, string claimType)
    {
        var claimValue = GetClaimValue(claims, claimType);
        if (bool.TryParse(claimValue, out var boolValue))
            return boolValue;

        throw new Exception($"Claim {claimType} could not be parsed to a Boolean.");
    }
}