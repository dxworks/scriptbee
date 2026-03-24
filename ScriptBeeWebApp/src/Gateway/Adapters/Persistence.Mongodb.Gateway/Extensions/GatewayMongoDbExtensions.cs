using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Entity.Script;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Ports.Scripts;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class GatewayMongoDbExtensions
{
    public static IServiceCollection AddProjectAdapters(
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

    public static IServiceCollection AddProjectInstancesAdapters(
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

    public static IServiceCollection AddScriptAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbScript>(mongoDatabase, "Scripts")
            .AddSingleton<ICreateScript, ScriptPersistenceAdapter>()
            .AddSingleton<IGetScript, ScriptPersistenceAdapter>();
    }
}
