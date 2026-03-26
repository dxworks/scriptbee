using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Analysis;
using ScriptBee.Persistence.Mongodb.Entity;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class AnalysisMongoDbExtensions
{
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
}
