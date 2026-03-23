using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Entity.Analysis;
using ScriptBee.Persistence.Mongodb.Entity.Script;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.Ports.Project.Structure;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class MongoDbExtensions
{
    public static IMongoDatabase AddMongodbDatabase(
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

        services
            .AddSingleton<IMongoClient>(mongoClient)
            .AddSingleton(mongoDatabase)
            .AddSingleton<IFileModelService, FileModelService>();
        return mongoDatabase;
    }

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

    public static IServiceCollection AddAnalysisAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbAnalysisInfo>(mongoDatabase, "Analysis")
            .AddSingleton<IGetAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IGetAllAnalyses, AnalysisPersistenceAdapter>()
            .AddSingleton<ICreateAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IUpdateAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IDeleteAnalysis, AnalysisPersistenceAdapter>();
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
