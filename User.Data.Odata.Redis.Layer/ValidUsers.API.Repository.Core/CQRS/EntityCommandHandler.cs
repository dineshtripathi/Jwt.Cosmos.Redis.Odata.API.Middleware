using MediatR;
using ValidUsers.API.Repository.Core.DTO;
using ValidUsers.API.Repository.Core.Repository;

namespace ValidUsers.API.Repository.Core.CQRS;
/// <summary>
/// The create user command handler.
/// </summary>

public class EntityCommandHandler<TEntity> : IRequestHandler<CreateEntityCommand<TEntity>, TEntity>
    where TEntity : class, IEntity
{
    private readonly IGenericRepository<TEntity> _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityCommandHandler{TEntity}"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public EntityCommandHandler(IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    public async Task<TEntity> Handle(CreateEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        return await _repository.AddItemAsync(request.Entity, request.Entity.id, cancellationToken);
    }
}
