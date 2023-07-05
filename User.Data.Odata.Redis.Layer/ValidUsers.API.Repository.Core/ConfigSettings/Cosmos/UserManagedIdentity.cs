namespace ValidUsers.API.Repository.Core.ConfigSettings.Cosmos;
/// <summary>
/// The user managed identity.
/// </summary>

public class UserManagedIdentity
{
    /// <summary>
    /// Gets or sets a value indicating whether user managed identity is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the client id of the user managed identity.
    /// </summary>
    public string? ClientId { get; set; }
}