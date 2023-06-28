namespace JwtValidUserAPI.Repository.Core.ConfigSettings.Redis;
/// <summary>
/// The redis settings.
/// </summary>

public class RedisSettings
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public required string ConnectionString { get; set; }
    /// <summary>
    /// Gets or sets the databases.
    /// </summary>
    public required Dictionary<string, int> Databases { get; set; }
}