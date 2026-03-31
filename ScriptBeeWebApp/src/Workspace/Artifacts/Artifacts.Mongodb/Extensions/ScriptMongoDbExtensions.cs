using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Persistence.Mongodb.Extensions;

namespace ScriptBee.Artifacts.Mongodb.Extensions;

public static class ScriptMongoDbExtensions
{
    public static IServiceCollection AddScriptAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbScript>(mongoDatabase, "Scripts")
            .AddSingleton<IFileModelService, FileModelService>()
            .AddSingleton<ICreateScript, ScriptsPersistenceAdapter>()
            .AddSingleton<IGetScripts, ScriptsPersistenceAdapter>();
    }
}
