using MediatR;
using ValidUsers.API.Repository.Core.DTO;

namespace ValidUsers.API.Repository.Core.CQRS;

/// <summary>
/// The get user query.
/// </summary>

public class GetUserQuery : IRequest<ValidUser?>
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserQuery"/> class.
    /// </summary>
    /// <param name="id">The id.</param>
    public GetUserQuery(string id)
    {
        Id = id;
    }
}