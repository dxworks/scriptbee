using ScriptBee.Artifacts;
using ScriptBee.Artifacts.Mongodb;
using ScriptBee.Artifacts.Mongodb.Extensions;
using ScriptBee.Persistence.Mongodb.Extensions;

namespace ScriptBee.Analysis.Web.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        string? connectionString
    )
    {
        var mongoDatabase = services.AddMongodbDatabase(connectionString);

        return services
            .AddSingleton<IFileModelService, FileModelService>()
            .AddAnalysisAdapters(mongoDatabase)
            .AddScriptAdapters(mongoDatabase);
    }
}
