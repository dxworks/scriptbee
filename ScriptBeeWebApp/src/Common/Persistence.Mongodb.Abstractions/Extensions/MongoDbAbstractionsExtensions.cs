using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class MongoDbAbstractionsExtensions
{
    public static IMongoDatabase AddMongodbDatabase(
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

        services.AddSingleton<IMongoClient>(mongoClient).AddSingleton(mongoDatabase);

        return mongoDatabase;
    }

    public static IServiceCollection AddMongoCollection<T>(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase,
        string collectionName
    )
        where T : IDocument
    {
        return services
            .AddSingleton<IMongoCollection<T>>(_ => mongoDatabase.GetCollection<T>(collectionName))
            .AddSingleton<IMongoRepository<T>, MongoRepository<T>>();
    }
}
