using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class MongoDbAbstractionsExtensions
{
    extension(IServiceCollection services)
    {
        public IMongoDatabase AddMongodbDatabase(string? connectionString)
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

        public IServiceCollection AddMongoCollection<T>(
            IMongoDatabase mongoDatabase,
            string collectionName
        )
            where T : IDocument
        {
            return services
                .AddSingleton<IMongoCollection<T>>(_ =>
                    mongoDatabase.GetCollection<T>(collectionName)
                )
                .AddSingleton<IMongoRepository<T>, MongoRepository<T>>();
        }
    }
}
