using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb;
using ScriptBee.Persistence.Mongodb.Entity.Analysis;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Analysis;

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
        return AddProjectAdapters(services, mongoDatabase);
    }

    private static IServiceCollection AddProjectAdapters(
        IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services.AddAnalysisAdapters(mongoDatabase);
    }

    private static IServiceCollection AddAnalysisAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddSingleton<IMongoCollection<MongodbAnalysisInfo>>(_ =>
                mongoDatabase.GetCollection<MongodbAnalysisInfo>("Analysis")
            )
            .AddSingleton<
                IMongoRepository<MongodbAnalysisInfo>,
                MongoRepository<MongodbAnalysisInfo>
            >()
            .AddSingleton<IGetAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IGetAllAnalyses, AnalysisPersistenceAdapter>()
            .AddSingleton<ICreateAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IUpdateAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IDeleteAnalysis, AnalysisPersistenceAdapter>();
    }
}
