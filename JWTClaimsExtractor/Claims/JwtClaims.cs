namespace JWTClaimsExtractor.Claims;

public class JwtClaims
{
    public string CorrelationId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Sub { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public string OrgRole { get; set; } = string.Empty;
    public string ExtensionAgreedTermsAndConditionsVersion { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsTpiConsultancy { get; set; }
    public string OrgName { get; set; } = string.Empty;
}