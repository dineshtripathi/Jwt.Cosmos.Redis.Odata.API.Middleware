namespace JwtValidUserAPI.Repository.Core.ConfigSettings.Cosmos;

/// <summary>
/// The container.
/// </summary>

public class Container
{
    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public required string User { get; set; }
    /// <summary>
    /// Gets or sets the department.
    /// </summary>
    public required string Department { get; set; }
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public required string Location { get; set; }
}