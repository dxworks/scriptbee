using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb;
using ScriptBee.Persistence.Mongodb.Entity.Analysis;
using ScriptBee.Persistence.Mongodb.Entity.Script;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Project.Structure;

namespace ScriptBee.Analysis.Web.Extensions;

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
        return services.AddAnalysisAdapters(mongoDatabase).AddScriptAdapters(mongoDatabase);
    }

    private static IServiceCollection AddAnalysisAdapters(
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

    private static IServiceCollection AddScriptAdapters(
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
