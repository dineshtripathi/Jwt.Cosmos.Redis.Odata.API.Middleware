using JwtValidUserAPI.Repository.Core.CQRS;
using JwtValidUserAPI.Repository.Core.Repository;

namespace JwtValidUserAPI.Repository.Core.Registration;
using Autofac;
using MediatR;
/// <summary>
/// The application module.
/// </summary>

public class ApplicationModule : Module
{
    /// <summary>
    /// Loads the.
    /// </summary>
    /// <param name="builder">The builder.</param>
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(CosmosRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(RedisRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(GetUserQueryHandler).Assembly)
            .Where(t => t.IsClosedTypeOf(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces();
    }
}

