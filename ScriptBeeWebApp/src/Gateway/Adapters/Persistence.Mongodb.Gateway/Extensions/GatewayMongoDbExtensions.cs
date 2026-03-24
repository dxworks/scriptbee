using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class GatewayMongoDbExtensions
{
    public static IServiceCollection AddGatewayProjectAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbProjectModel>(mongoDatabase, "Projects")
            .AddSingleton<ICreateProject, ProjectPersistenceAdapter>()
            .AddSingleton<IDeleteProject, ProjectPersistenceAdapter>()
            .AddSingleton<IGetAllProjects, ProjectPersistenceAdapter>()
            .AddSingleton<IGetProject, ProjectPersistenceAdapter>()
            .AddSingleton<IUpdateProject, ProjectPersistenceAdapter>();
    }

    public static IServiceCollection AddGatewayProjectInstancesAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbProjectInstance>(mongoDatabase, "Instances")
            .AddSingleton<ICreateProjectInstance, ProjectInstancesPersistenceAdapter>()
            .AddSingleton<IGetAllProjectInstances, ProjectInstancesPersistenceAdapter>()
            .AddSingleton<IGetProjectInstance, ProjectInstancesPersistenceAdapter>();
    }
}
