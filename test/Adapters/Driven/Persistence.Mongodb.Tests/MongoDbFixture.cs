using System.Net;
using System.Net.Sockets;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container;

    public IMongoDatabase Database { get; set; } = null!;

    public IMongoCollection<T> GetCollection<T>(string name) => Database.GetCollection<T>(name);

    public MongoDbFixture()
    {
        var freePort = GetAvailablePort();

        _container = new MongoDbBuilder()
            .WithImage("mongo:8.0.4")
            .WithPortBinding(freePort, 27017)
            .WithCleanUp(true)
            .Build();
    }

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

    private static int GetAvailablePort()
    {
        using var socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        socket.Listen(1);
        var endPoint = (IPEndPoint)socket.LocalEndPoint!;
        return endPoint.Port;
    }
}
