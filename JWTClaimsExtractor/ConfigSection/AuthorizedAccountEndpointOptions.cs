namespace JWTClaimsExtractor.ConfigSection;

public class AuthorizedAccountEndpointOptions
{
    public string BaseUri { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string EndpointRoute { get; set; } = string.Empty;
    public string CustomClaimName { get; set; }=string.Empty;
}