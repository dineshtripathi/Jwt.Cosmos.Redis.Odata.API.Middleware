namespace JWTClaimsExtractor.Claims;

public record CustomUser(
    IEnumerable<AuthorizedAccount>? AuthorizedAccounts,
    Guid? CorrelationId = default,
    string? Email = default,
    Guid? Sub = default,
    string? GivenName = default,
    string? FamilyName = default,
    Guid OrgId = default,
    string? OrgRole = default,
    string? Name = default,
    bool? IsTpiConsultancy = default,
    string? OrgName = default);