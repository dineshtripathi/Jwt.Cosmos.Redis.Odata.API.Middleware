using Newtonsoft.Json;

namespace JwtValidUserAPI.Repository.Core.DTO;

/// <summary>
/// The department.
/// </summary>
public class Department:IEntity
{
    /// <summary>
    /// Gets or sets the department id.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the department name.
    /// </summary>
    public required string DepartmentName { get; set; }

    public string Id { get; }
}