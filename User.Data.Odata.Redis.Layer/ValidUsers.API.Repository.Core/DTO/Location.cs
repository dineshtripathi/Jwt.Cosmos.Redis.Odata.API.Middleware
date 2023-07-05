using Newtonsoft.Json;

namespace ValidUsers.API.Repository.Core.DTO;

/// <summary>
/// The location.
/// </summary>
public class Location:IEntity
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

    public string id { get; }
}