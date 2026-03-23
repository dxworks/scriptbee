using static ScriptBee.Persistence.Mongodb.Extensions.MongoDbExtensions;

namespace ScriptBee.Analysis.Web.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        string? connectionString
    )
    {
        var mongoDatabase = services.AddMongodbDatabase(connectionString);

        return services.AddAnalysisAdapters(mongoDatabase).AddScriptAdapters(mongoDatabase);
    }
}
