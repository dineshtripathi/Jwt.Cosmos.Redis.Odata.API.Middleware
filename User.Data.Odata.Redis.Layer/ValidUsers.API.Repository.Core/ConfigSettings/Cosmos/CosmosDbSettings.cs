namespace ValidUsers.API.Repository.Core.ConfigSettings.Cosmos;
/// <summary>
/// The cosmos db settings options.
/// </summary>
public class CosmosDbSettings
{
    /// <summary>
    /// Gets or sets the cosmos db endpoint.
    /// </summary>
    public required string CosmosDbEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the connection key.
    /// </summary>
    public required string ConnectionKey { get; set; }

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public required string DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the containers.
    /// </summary>
    public required List<Container> Containers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether system managed identity is enabled.
    /// </summary>
    public bool SystemManagedIdentityEnabled { get; set; }

    /// <summary>
    /// Gets or sets the user managed identity settings.
    /// </summary>
    public UserManagedIdentity? IfUserManagedIdentity { get; set; }
}