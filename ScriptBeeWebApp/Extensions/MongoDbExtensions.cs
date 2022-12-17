using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBeeWebApp.Exceptions.Configuration;

namespace ScriptBeeWebApp.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidMongoConfigurationException("MongoDB connection string is not set");
        }

        var mongoUrl = new MongoUrl(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        services.AddSingleton(_ => mongoDatabase);
        return services;
    }
}
