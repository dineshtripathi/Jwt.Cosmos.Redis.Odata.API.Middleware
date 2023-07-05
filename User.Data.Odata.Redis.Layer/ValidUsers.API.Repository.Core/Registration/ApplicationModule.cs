using Autofac;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Configuration.Internal;
using ValidUsers.API.Repository.Core.CQRS;
using ValidUsers.API.Repository.Core.Repository;

namespace ValidUsers.API.Repository.Core.Registration;

/// <summary>
/// The application module.
/// </summary>

public class ApplicationModule : Module
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationModule"/> class.
    /// </summary>
    /// <param name="">The .</param>
    /// <param name="configuration"></param>
    public ApplicationModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }
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

