using Newtonsoft.Json;

namespace JwtValidUsersOdataRedis.Api.Model;

/// <summary>
/// The location.
/// </summary>
public class Location
{
    /// <summary>
    /// Gets or sets the location id.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public required string LocationId { get; set; }

    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string? LocationName { get; set; }
}