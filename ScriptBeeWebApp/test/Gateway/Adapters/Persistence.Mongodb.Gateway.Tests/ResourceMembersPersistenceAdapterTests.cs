using MongoDB.Driver;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class ResourceMembersPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
{
    private readonly ResourceMembersPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbResourceMember> _mongoCollection;

    public ResourceMembersPersistenceAdapterIntegrationTests(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbResourceMember>("ResourceMembers");
        _adapter = new ResourceMembersPersistenceAdapter(
            new MongoRepository<MongodbResourceMember>(_mongoCollection)
        );
    }

    [Fact]
    public async Task GetRole_ForUserMember()
    {
        var projectId = ProjectId.FromValue("project-user-member");
        var userId = new UserId("user-id");
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "user",
                MemberId = userId.Value,
                Role = "owner",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetRole(
            userId,
            [],
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(new UserRole("owner"));
    }

    [Fact]
    public async Task GetRole_ForGroupMember()
    {
        var projectId = ProjectId.FromValue("project-group-member");
        var userId = new UserId("user-id");
        var groups = new List<UserGroup> { new("admins"), new("reviewers") };
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "group",
                MemberId = "admins",
                Role = "editor",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetRole(
            userId,
            groups,
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(new UserRole("editor"));
    }

    [Fact]
    public async Task GivenUserWithNoMatchingAccess_GetRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-no-access");
        var userId = new UserId("user-with-no-access");
        var groups = new List<UserGroup> { new("admins") };
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "user",
                MemberId = "someone-else",
                Role = "owner",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "group",
                MemberId = "reviewers",
                Role = "viewer",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetRole(
            userId,
            groups,
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GivenProjectDoesNotMatch_GetRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-id");
        var userId = new UserId("user-id");
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "project",
                ResourceId = "other-project-id",
                MemberType = "user",
                MemberId = userId.Value,
                Role = "admin",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetRole(
            userId,
            [],
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GivenResourceTypeDoesNotMatch_GetRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-resource-type");
        var userId = new UserId("user-id");
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "dataset",
                ResourceId = projectId.Value,
                MemberType = "user",
                MemberId = userId.Value,
                Role = "owner",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetRole(
            userId,
            [],
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GivenGroupMemberExistsButGroupIsNotInList_GetRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-group-mismatch");
        var userId = new UserId("user-id");
        var groups = new List<UserGroup> { new("developers") };
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "group",
                MemberId = "admins",
                Role = "editor",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetRole(
            userId,
            groups,
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }

    [Fact]
    public async Task SetRoleForUser()
    {
        var projectId = ProjectId.FromValue("project-to-added-for-user");
        var userId = new UserId("user-id-added-to-project");

        await _adapter.SetRoleForUser(
            userId,
            projectId,
            new UserRole("role-added"),
            TestContext.Current.CancellationToken
        );

        var savedProject = await _mongoCollection
            .Find(p => p.ResourceId == projectId.Value)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        savedProject.ResourceType.ShouldBe("project");
        savedProject.MemberType.ShouldBe("user");
        savedProject.MemberId.ShouldBe(userId.Value);
        savedProject.AssignedAt.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}
