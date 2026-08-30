using ScriptBee.Analysis.Mongodb.Extensions;
using ScriptBee.Artifacts.Mongodb.Extensions;
using ScriptBee.Persistence.Mongodb;
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
            .AddResourceMembersAdapters(mongoDatabase)
            .AddUserManagementAdapters(mongoDatabase)
            .AddProjectTokensAdapters(mongoDatabase)
            .AddAnalysisAdapters(mongoDatabase)
            .AddScriptAdapters(mongoDatabase);
    }

    public static async Task CreateMongodbIndexes(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        using var scope = serviceProvider.CreateScope();
        var indexCreators = scope.ServiceProvider.GetServices<IIndexCreator>();

        foreach (var indexCreator in indexCreators)
        {
            await indexCreator.Create(cancellationToken);
        }
    }
}
