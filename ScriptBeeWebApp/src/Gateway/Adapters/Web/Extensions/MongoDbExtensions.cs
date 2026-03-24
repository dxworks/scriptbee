using ScriptBee.Persistence.Mongodb.Extensions;

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
            .AddScriptAdapters(mongoDatabase);
    }
}
