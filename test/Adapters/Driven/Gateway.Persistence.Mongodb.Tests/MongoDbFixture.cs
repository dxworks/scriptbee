using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace ScriptBee.Gateway.Persistence.Mongodb.Tests;

public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container = new MongoDbBuilder()
        .WithImage("mongo:8.0.4")
        .WithPortBinding(1337, 27017)
        .WithCleanUp(true)
        .Build();

    private IMongoDatabase Database { get; set; } = null!;

    public IMongoCollection<T> GetCollection<T>(string name) => Database.GetCollection<T>(name);

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var client = new MongoClient(_container.GetConnectionString());
        Database = client.GetDatabase("TestDatabase");
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
    }
}
