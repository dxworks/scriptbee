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
    public async Task GetResourceRoleForUserMember()
    {
        var projectId = ProjectId.FromValue("project-user-member");
        var userId = new UserId("user-id");
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                Id = "user-member-id",
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "user",
                MemberId = userId.Value,
                Role = "owner",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetResourceRole(
            userId,
            [],
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(new UserRole("owner"));
    }

    [Fact]
    public async Task GetResourceRoleForGroupMember()
    {
        var projectId = ProjectId.FromValue("project-group-member");
        var userId = new UserId("user-id");
        var groups = new List<UserGroup> { new("admins"), new("reviewers") };
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                Id = "group-member-id",
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "group",
                MemberId = "admins",
                Role = "editor",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetResourceRole(
            userId,
            groups,
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(new UserRole("editor"));
    }

    [Fact]
    public async Task GivenUserWithNoMatchingAccess_GetResourceRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-no-access");
        var userId = new UserId("user-with-no-access");
        var groups = new List<UserGroup> { new("admins") };
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                Id = "different-user-member-id",
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "user",
                MemberId = "someone-else",
                Role = "owner",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                Id = "different-group-member-id",
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "group",
                MemberId = "reviewers",
                Role = "viewer",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetResourceRole(
            userId,
            groups,
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GivenProjectDoesNotMatch_GetResourceRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-id");
        var userId = new UserId("user-id");
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                Id = "other-project-member-id",
                ResourceType = "project",
                ResourceId = "other-project-id",
                MemberType = "user",
                MemberId = userId.Value,
                Role = "admin",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetResourceRole(
            userId,
            [],
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GivenResourceTypeDoesNotMatch_GetResourceRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-resource-type");
        var userId = new UserId("user-id");
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                Id = "other-resource-type-member-id",
                ResourceType = "dataset",
                ResourceId = projectId.Value,
                MemberType = "user",
                MemberId = userId.Value,
                Role = "owner",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetResourceRole(
            userId,
            [],
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GivenGroupMemberExistsButGroupIsNotInList_GetResourceRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("project-group-mismatch");
        var userId = new UserId("user-id");
        var groups = new List<UserGroup> { new("developers") };
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                Id = "group-member-mismatch-id",
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "group",
                MemberId = "admins",
                Role = "editor",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetResourceRole(
            userId,
            groups,
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeNull();
    }
}
