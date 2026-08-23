using MongoDB.Driver;
using ScriptBee.Domain.Model.User;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class UserManagementPersistenceAdapterTests : IClassFixture<MongoDbFixture>
{
    private readonly UserManagementPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbUser> _mongoCollection;

    public UserManagementPersistenceAdapterTests(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbUser>("Users");
        _adapter = new UserManagementPersistenceAdapter(
            new MongoRepository<MongodbUser>(_mongoCollection)
        );
    }

    [Fact]
    public async Task GetOrAddUser_WhenUserDoesNotExist_ShouldCreateUser()
    {
        var result = await _adapter.GetOrAddUser(
            "external-user-id",
            "external-user-name",
            TestContext.Current.CancellationToken
        );

        result.Value.ShouldNotBeNullOrEmpty();
        var savedUser = await _mongoCollection
            .Find(u => u.ExternalId == "external-user-id")
            .FirstOrDefaultAsync(TestContext.Current.CancellationToken);

        savedUser.ShouldNotBeNull();
        savedUser.ExternalId.ShouldBe("external-user-id");
        savedUser.Name.ShouldBe("external-user-name");
        savedUser.CreatedAt.ShouldNotBe(default(DateTimeOffset));
        result.ShouldBe(new UserId(savedUser.Id));
    }

    [Fact]
    public async Task GetOrAddUser_WhenUserAlreadyExists_ShouldReturnExistingUserId()
    {
        var createdAt = DateTimeOffset.UtcNow.AddDays(-1);
        await _mongoCollection.InsertOneAsync(
            new MongodbUser
            {
                ExternalId = "existing-external-id",
                Name = "existing-user-name",
                CreatedAt = createdAt,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetOrAddUser(
            "existing-external-id",
            "new-user-name",
            TestContext.Current.CancellationToken
        );

        Assert.NotEmpty(result.Value);
        var userCount = await _mongoCollection.CountDocumentsAsync(
            u => u.ExternalId == "existing-external-id",
            cancellationToken: TestContext.Current.CancellationToken
        );
        userCount.ShouldBe(1);
        var savedUser = await _mongoCollection
            .Find(u => u.ExternalId == "existing-external-id")
            .FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        savedUser.Name.ShouldBe("existing-user-name");
    }
}
