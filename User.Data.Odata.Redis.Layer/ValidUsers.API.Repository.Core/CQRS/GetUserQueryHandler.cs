using JwtValidUserAPI.Repository.Core.DTO;
using JwtValidUserAPI.Repository.Core.Repository;
using MediatR;

namespace JwtValidUserAPI.Repository.Core.CQRS;

/// <summary>
/// The get user query handler.
/// </summary>

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ValidUser?>
{
    private readonly IGenericRepository<ValidUser?> _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserQueryHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    public GetUserQueryHandler(IGenericRepository<ValidUser?> userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles the.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    public async Task<ValidUser?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetItemAsync(request.Id, cancellationToken);
    }
}