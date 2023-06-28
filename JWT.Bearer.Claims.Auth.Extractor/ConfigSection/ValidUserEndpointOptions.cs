namespace JWTClaimsExtractor.ConfigSection;
/// <summary>
/// The authorized account endpoint options.
/// </summary>

public class ValidUserEndpointOptions
{
    /// <summary>
    /// Gets or sets the base uri.
    /// </summary>
    public string ApiGatewayEndpoint { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the api key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the endpoint route.
    /// </summary>
    public string EndpointRoute { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the custom claim name.
    /// </summary>
    public string CustomClaimName { get; set; }=string.Empty;
}