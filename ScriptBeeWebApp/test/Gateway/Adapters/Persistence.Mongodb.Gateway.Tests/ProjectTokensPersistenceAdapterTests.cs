using MongoDB.Driver;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class ProjectTokensPersistenceAdapterTests : IClassFixture<MongoDbFixture>
{
    private readonly ProjectTokensPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbProjectToken> _mongoCollection;

    public ProjectTokensPersistenceAdapterTests(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbProjectToken>("ProjectTokens");
        _adapter = new ProjectTokensPersistenceAdapter(
            new MongoRepository<MongodbProjectToken>(_mongoCollection)
        );
    }

    [Fact]
    public async Task CreateToken_ShouldPersistToken()
    {
        var projectId = ProjectId.FromValue("project-create-token");
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        var token = await _adapter.CreateToken(
            projectId,
            "hashed-token",
            "test-token",
            new UserRole("editor"),
            expiresAt,
            TestContext.Current.CancellationToken
        );

        token.ProjectId.ShouldBe(projectId);
        token.TokenHash.ShouldBe("hashed-token");
        token.Description.ShouldBe("test-token");
        token.Role.ShouldBe(new UserRole("editor"));
        token.ExpiresAt.ShouldBe(expiresAt, TimeSpan.FromSeconds(5));
        token.Id.Value.ShouldNotBeNullOrWhiteSpace();

        var savedToken = await _mongoCollection
            .Find(t => t.ProjectId == projectId.Value)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        savedToken.ShouldNotBeNull();
        savedToken.ProjectId.ShouldBe(projectId.Value);
        savedToken.TokenHash.ShouldBe("hashed-token");
        savedToken.Description.ShouldBe("test-token");
        savedToken.Role.ShouldBe("editor");
    }

    [Fact]
    public async Task GetAllForProjectId_ShouldReturnOnlyMatchingProjectTokens()
    {
        var projectId = ProjectId.FromValue("project-get-all-tokens");
        var otherProjectId = ProjectId.FromValue("other-project");

        await _mongoCollection.InsertManyAsync(
            [
                new MongodbProjectToken
                {
                    ProjectId = projectId.Value,
                    TokenHash = "hash-one",
                    Description = "first token",
                    Role = "owner",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
                },
                new MongodbProjectToken
                {
                    ProjectId = projectId.Value,
                    TokenHash = "hash-two",
                    Description = "second token",
                    Role = "viewer",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExpiresAt = DateTimeOffset.UtcNow.AddDays(2),
                },
                new MongodbProjectToken
                {
                    ProjectId = otherProjectId.Value,
                    TokenHash = "hash-other",
                    Description = "other project",
                    Role = "editor",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExpiresAt = DateTimeOffset.UtcNow.AddDays(3),
                },
            ],
            cancellationToken: TestContext.Current.CancellationToken
        );

        var tokens = await _adapter.GetAllForProjectId(
            projectId,
            TestContext.Current.CancellationToken
        );

        tokens.Count.ShouldBe(2);
        tokens.ShouldContain(token =>
            token.ProjectId == projectId
            && token.TokenHash == "hash-one"
            && token.Description == "first token"
            && token.Role == new UserRole("owner")
        );
        tokens.ShouldContain(token =>
            token.ProjectId == projectId
            && token.TokenHash == "hash-two"
            && token.Description == "second token"
            && token.Role == new UserRole("viewer")
        );
    }

    [Fact]
    public async Task DeleteToken_ShouldRemoveMatchingProjectToken()
    {
        var projectId = ProjectId.FromValue("project-delete-token");
        var otherProjectId = ProjectId.FromValue("other-project-delete");
        var tokenId = new ProjectTokenId("507f1f77bcf86cd799439011");
        var otherTokenId = new ProjectTokenId("507f1f77bcf86cd799439012");
        var otherProjectTokenId = new ProjectTokenId("507f1f77bcf86cd799439013");

        await _mongoCollection.InsertManyAsync(
            [
                new MongodbProjectToken
                {
                    Id = tokenId.Value,
                    ProjectId = projectId.Value,
                    TokenHash = "hash-delete",
                    Description = "delete me",
                    Role = "editor",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExpiresAt = DateTimeOffset.UtcNow.AddDays(2),
                },
                new MongodbProjectToken
                {
                    Id = otherTokenId.Value,
                    ProjectId = projectId.Value,
                    TokenHash = "hash-keep",
                    Description = "keep me",
                    Role = "viewer",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExpiresAt = DateTimeOffset.UtcNow.AddDays(3),
                },
                new MongodbProjectToken
                {
                    Id = otherProjectTokenId.Value,
                    ProjectId = otherProjectId.Value,
                    TokenHash = "hash-other-project",
                    Description = "other project",
                    Role = "owner",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ExpiresAt = DateTimeOffset.UtcNow.AddDays(4),
                },
            ],
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.DeleteToken(projectId, tokenId, TestContext.Current.CancellationToken);

        var remaining = await _mongoCollection
            .Find(t => t.ProjectId == projectId.Value)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

        remaining.Count.ShouldBe(1);
        remaining[0].Id.ShouldBe(otherTokenId.Value);
        remaining[0].TokenHash.ShouldBe("hash-keep");

        var differentProjectToken = await _mongoCollection
            .Find(t => t.ProjectId == otherProjectId.Value)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        differentProjectToken.ShouldNotBeNull();
        differentProjectToken.TokenHash.ShouldBe("hash-other-project");
    }
}
