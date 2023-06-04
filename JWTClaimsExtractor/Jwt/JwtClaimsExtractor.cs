using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Services;

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

        return new CustomUser
        (
            accounts,
            ParseGuidClaim(claims, CustomClaimTypes.CorrelationId),
            GetClaimValue(claims, CustomClaimTypes.Email),
            ParseGuidClaim(claims, CustomClaimTypes.Sub),
            GetClaimValue(claims, CustomClaimTypes.GivenName),
            GetClaimValue(claims, CustomClaimTypes.FamilyName),
            ParseGuidClaim(claims, CustomClaimTypes.OrgId),
            GetClaimValue(claims, CustomClaimTypes.OrgRole),
            GetClaimValue(claims, CustomClaimTypes.Name),
            ParseBoolClaim(claims, CustomClaimTypes.IsTpiConsultancy),
            GetClaimValue(claims, CustomClaimTypes.OrgName)
        );
    }

    private static string? GetClaimValue(List<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    private static Guid ParseGuidClaim(List<Claim> claims, string claimType)
    {
        var claimValue = GetClaimValue(claims, claimType);
        if (Guid.TryParse(claimValue, out var guidValue))
            return guidValue;

        throw new Exception($"Claim {claimType} could not be parsed to a Guid.");
    }

    private static bool ParseBoolClaim(List<Claim> claims, string claimType)
    {
        var claimValue = GetClaimValue(claims, claimType);
        if (bool.TryParse(claimValue, out var boolValue))
            return boolValue;

        throw new Exception($"Claim {claimType} could not be parsed to a Boolean.");
    }
}