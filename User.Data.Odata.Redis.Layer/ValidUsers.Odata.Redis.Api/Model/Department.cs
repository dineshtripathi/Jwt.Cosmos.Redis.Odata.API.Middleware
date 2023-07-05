using Newtonsoft.Json;

namespace ValidUsers.Odata.Redis.Api.Model;

/// <summary>
/// The department.
/// </summary>
public class Department
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
}