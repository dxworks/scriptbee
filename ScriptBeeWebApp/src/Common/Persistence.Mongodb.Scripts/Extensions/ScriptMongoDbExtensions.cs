using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity.Script;
using ScriptBee.Ports.Scripts;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class ScriptMongoDbExtensions
{
    public static IServiceCollection AddScriptAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddMongoCollection<MongodbScript>(mongoDatabase, "Scripts")
            .AddSingleton<ICreateScript, ScriptPersistenceAdapter>()
            .AddSingleton<IGetScript, ScriptPersistenceAdapter>();
    }
}

