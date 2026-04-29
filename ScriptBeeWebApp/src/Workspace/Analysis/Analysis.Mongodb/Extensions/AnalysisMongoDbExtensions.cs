using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Analysis.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Extensions;

namespace ScriptBee.Analysis.Mongodb.Extensions;

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
            .AddSingleton<IGetRunningAnalyses, AnalysisPersistenceAdapter>()
            .AddSingleton<ICreateAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IUpdateAnalysis, AnalysisPersistenceAdapter>()
            .AddSingleton<IDeleteAnalysis, AnalysisPersistenceAdapter>();
    }
}
