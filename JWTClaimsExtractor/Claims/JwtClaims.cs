namespace JWTClaimsExtractor.Claims;

public class JwtClaims
{
    public string CorrelationId { get; set; }
    public string Email { get; set; }
    public string Sub { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string OrgId { get; set; }
    public string OrgRole { get; set; }
    public string ExtensionAgreedTermsAndConditionsVersion { get; set; }
    public string Name { get; set; }
    public bool IsTpiConsultancy { get; set; }
    public string OrgName { get; set; }
}