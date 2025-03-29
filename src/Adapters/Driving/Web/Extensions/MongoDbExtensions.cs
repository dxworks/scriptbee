using static ScriptBee.Persistence.Mongodb.Extensions.MongoDbExtensions;

namespace ScriptBee.Web.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        string? connectionString
    )
    {
        var mongoDatabase = services.AddMongodbDatabase(connectionString);

        return services
            .AddProjectAdapters(mongoDatabase)
            .AddProjectInstancesAdapters(mongoDatabase)
            .AddScriptAdapters(mongoDatabase)
            .AddAnalysisAdapters(mongoDatabase);
    }
}
