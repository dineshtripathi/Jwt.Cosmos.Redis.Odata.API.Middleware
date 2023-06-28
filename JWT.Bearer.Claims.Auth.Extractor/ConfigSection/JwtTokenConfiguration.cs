namespace JWTClaimsExtractor.ConfigSection;
/// <summary>
/// The jwt token configuration.
/// </summary>

public class JwtTokenConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether validate issuer.
    /// </summary>
    public bool ValidateIssuer { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether validate audience.
    /// </summary>
    public bool ValidateAudience { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether validate lifetime.
    /// </summary>
    public bool ValidateLifetime { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether validate issuer signing key.
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; }
    /// <summary>
    /// Gets or sets the valid issuer.
    /// </summary>
    public string ValidIssuer { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the valid audience.
    /// </summary>
    public string ValidAudience { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the issuer signing key.
    /// </summary>
    public string IssuerSigningKey { get; set; } = string.Empty;
}