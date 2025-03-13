using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Entity.Script;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Ports.Project.Structure;

namespace ScriptBee.Web.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        string? connectionString
    )
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidMongoConfigurationException();
        }

        var mongoUrl = new MongoUrl(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        return services.AddSingleton<IMongoClient>(mongoClient).AddAdapters(mongoDatabase);
    }

    private static IServiceCollection AddAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddProjectAdapters(mongoDatabase)
            .AddProjectInstancesAdapters(mongoDatabase)
            .AddScriptAdapters(mongoDatabase);
    }

    private static IServiceCollection AddProjectAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbProjectModel>(mongoDatabase, "Projects")
            .AddSingleton<ICreateProject, ProjectPersistenceAdapter>()
            .AddSingleton<IDeleteProject, ProjectPersistenceAdapter>()
            .AddSingleton<IGetAllProjects, ProjectPersistenceAdapter>()
            .AddSingleton<IGetProject, ProjectPersistenceAdapter>();
    }

    private static IServiceCollection AddProjectInstancesAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbProjectInstance>(mongoDatabase, "Instances")
            .AddSingleton<ICreateProjectInstance, ProjectInstancesPersistenceAdapter>()
            .AddSingleton<IGetAllProjectInstances, ProjectInstancesPersistenceAdapter>();
    }

    private static IServiceCollection AddScriptAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbScript>(mongoDatabase, "Scripts")
            .AddSingleton<ICreateScript, ScriptPersistenceAdapter>();
    }

    private static IServiceCollection AddMongoCollection<T>(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase,
        string collectionName
    )
        where T : IDocument
    {
        return services
            .AddSingleton<IMongoCollection<T>>(_ => mongoDatabase.GetCollection<T>(collectionName))
            .AddSingleton<IMongoRepository<T>, MongoRepository<T>>();
    }
}
