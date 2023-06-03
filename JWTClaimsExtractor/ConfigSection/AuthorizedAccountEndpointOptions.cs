namespace JWTClaimsExtractor.ConfigSection;

public class AuthorizedAccountEndpointOptions
{
    public string BaseUri { get; set; }
    public string ApiKey { get; set; }
    public string? EndpointRoute { get; set; }
    public string CustomClaimName { get; set; }
}
