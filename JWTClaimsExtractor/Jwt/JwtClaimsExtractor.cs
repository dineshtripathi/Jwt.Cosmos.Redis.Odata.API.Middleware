using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.Services;

namespace JWTClaimsExtractor.Jwt;
/// <summary>
/// The jwt claims extractor.
/// </summary>

public class JwtClaimsExtractor
{
    /// <summary>
    /// Extracts the async.
    /// </summary>
    /// <param name="jwt">The jwt.</param>
    /// <param name="orgService">The org service.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    public async Task<CustomUser> ExtractAsync(JwtSecurityToken? jwt, DataCenterValidUserEndpointClient orgService, CancellationToken cancellationToken)
    {
        if (jwt == null)
            throw new ArgumentNullException(nameof(jwt));

        var claims = jwt.Claims.ToList();

        if (!claims.Any())
            throw new Exception("No claims found in the JWT");

        var additionalClaims = await BuildAdditionalClaims(claims, orgService, cancellationToken);

        return new CustomUser(additionalClaims);
    }

    /// <summary>
    /// Builds the additional claims.
    /// </summary>
    /// <param name="claims">The claims.</param>
    /// <param name="orgService">The org service.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    private static async Task<Dictionary<string, object>> BuildAdditionalClaims(IEnumerable<Claim>? claims, DataCenterValidUserEndpointClient orgService, CancellationToken cancellationToken)
    {
        var additionalClaims = new Dictionary<string, object>();
        if (claims == null) 
            return additionalClaims;
        Debug.Assert(claims != null, nameof(claims) + " != null");
        var datacenterId = GetClaimValue(claims, "datacenterId");
        var accounts = await orgService.GetDataCentreValidUser(datacenterId, cancellationToken) ?? new List<object>();

        additionalClaims["ValidUsers"] = accounts;
       

        foreach (var claim in claims)
        {
            additionalClaims[claim.Type] = claim.Value;
        }
        return additionalClaims;
    }
    /// <summary>
    /// Gets the identity from user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="schemeName">The scheme name.</param>
    /// <returns>A ClaimsIdentity.</returns>
    public ClaimsIdentity GetIdentityFromUser(CustomUser user, string schemeName)
    {
        var identity = new ClaimsIdentity(schemeName);

        foreach (var claim in user.JwtCustomClaims)
        {
            var claimValue = JsonSerializer.Serialize(claim.Value);
            identity.AddClaim(new Claim(claim.Key, claimValue));
        }

        var authorizedUserClaim = new Claim("AuthorizedUser", JsonSerializer.Serialize(user));
        identity.AddClaim(authorizedUserClaim);

        return identity;
    }
    /// <summary>
    /// Gets the claim value.
    /// </summary>
    /// <param name="claims">The claims.</param>
    /// <param name="claimType">The claim type.</param>
    /// <returns>A string? .</returns>
    private static string? GetClaimValue(IEnumerable<Claim>? claims, string claimType) => claims?.FirstOrDefault(c => c.Type == claimType)?.Value;
}
