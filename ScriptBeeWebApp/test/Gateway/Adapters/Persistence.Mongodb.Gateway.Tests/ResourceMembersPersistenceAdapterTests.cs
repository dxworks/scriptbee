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

    [Fact]
    public async Task SetRoleForUser_WhenUserAlreadyExists_ShouldUpdateRole()
    {
        var projectId = ProjectId.FromValue("project-upsert-user");
        var userId = new UserId("user-id-upsert");

        await _adapter.SetRoleForUser(
            userId,
            projectId,
            new UserRole("initial-role"),
            TestContext.Current.CancellationToken
        );

        await _adapter.SetRoleForUser(
            userId,
            projectId,
            new UserRole("updated-role"),
            TestContext.Current.CancellationToken
        );

        var count = await _mongoCollection.CountDocumentsAsync(
            m => m.ResourceId == projectId.Value && m.MemberId == userId.Value,
            cancellationToken: TestContext.Current.CancellationToken
        );
        count.ShouldBe(1);

        var saved = await _mongoCollection
            .Find(m => m.ResourceId == projectId.Value && m.MemberId == userId.Value)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        saved.Role.ShouldBe("updated-role");
    }

    [Fact]
    public async Task SetRoleForMember_ShouldUpsertGroupRole()
    {
        var projectId = ProjectId.FromValue("project-group-upsert");
        const string groupId = "dev-team";

        await _adapter.SetRoleForMember(
            groupId,
            "group",
            projectId,
            new UserRole("viewer"),
            TestContext.Current.CancellationToken
        );

        await _adapter.SetRoleForMember(
            groupId,
            "group",
            projectId,
            new UserRole("editor"),
            TestContext.Current.CancellationToken
        );

        var count = await _mongoCollection.CountDocumentsAsync(
            m => m.ResourceId == projectId.Value && m.MemberId == groupId,
            cancellationToken: TestContext.Current.CancellationToken
        );
        count.ShouldBe(1);

        var saved = await _mongoCollection
            .Find(m => m.ResourceId == projectId.Value && m.MemberId == groupId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        saved.Role.ShouldBe("editor");
        saved.MemberType.ShouldBe("group");
    }

    [Fact]
    public async Task GetProjectMembers_ShouldReturnAllMembersForProject()
    {
        var projectId = ProjectId.FromValue("project-get-members");

        await _mongoCollection.InsertManyAsync(
            [
                new MongodbResourceMember
                {
                    ResourceType = "project",
                    ResourceId = projectId.Value,
                    MemberType = "user",
                    MemberId = "user-a",
                    Role = "owner",
                    AssignedAt = DateTimeOffset.UtcNow,
                },
                new MongodbResourceMember
                {
                    ResourceType = "project",
                    ResourceId = projectId.Value,
                    MemberType = "group",
                    MemberId = "team-b",
                    Role = "viewer",
                    AssignedAt = DateTimeOffset.UtcNow,
                },
                new MongodbResourceMember
                {
                    ResourceType = "project",
                    ResourceId = "other-project",
                    MemberType = "user",
                    MemberId = "user-c",
                    Role = "editor",
                    AssignedAt = DateTimeOffset.UtcNow,
                },
            ],
            cancellationToken: TestContext.Current.CancellationToken
        );

        var members = await _adapter.GetProjectMembers(
            projectId,
            TestContext.Current.CancellationToken
        );

        members.Count.ShouldBe(2);
        members.ShouldContain(m =>
            m.MemberId == "user-a" && m.MemberType == "user" && m.Role == new UserRole("owner")
        );
        members.ShouldContain(m =>
            m.MemberId == "team-b" && m.MemberType == "group" && m.Role == new UserRole("viewer")
        );
    }

    [Fact]
    public async Task RemoveProjectMember_ShouldDeleteMemberEntry()
    {
        var projectId = ProjectId.FromValue("project-remove-member");
        await _mongoCollection.InsertOneAsync(
            new MongodbResourceMember
            {
                ResourceType = "project",
                ResourceId = projectId.Value,
                MemberType = "user",
                MemberId = "user-to-remove",
                Role = "editor",
                AssignedAt = DateTimeOffset.UtcNow,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.RemoveProjectMember(
            projectId,
            "user-to-remove",
            "user",
            TestContext.Current.CancellationToken
        );

        var remaining = await _mongoCollection
            .Find(m => m.ResourceId == projectId.Value && m.MemberId == "user-to-remove")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        remaining.ShouldBeNull();
    }

    [Fact]
    public async Task RemoveAllProjectMembers_ShouldDeleteAllMembersForProject()
    {
        var projectId = ProjectId.FromValue("project-remove-all-members");
        var otherProjectId = ProjectId.FromValue("other-project-keep");

        await _mongoCollection.InsertManyAsync(
            [
                new MongodbResourceMember
                {
                    ResourceType = "project",
                    ResourceId = projectId.Value,
                    MemberType = "user",
                    MemberId = "user-1",
                    Role = "owner",
                    AssignedAt = DateTimeOffset.UtcNow,
                },
                new MongodbResourceMember
                {
                    ResourceType = "project",
                    ResourceId = projectId.Value,
                    MemberType = "group",
                    MemberId = "team-1",
                    Role = "viewer",
                    AssignedAt = DateTimeOffset.UtcNow,
                },
                new MongodbResourceMember
                {
                    ResourceType = "project",
                    ResourceId = otherProjectId.Value,
                    MemberType = "user",
                    MemberId = "user-other",
                    Role = "editor",
                    AssignedAt = DateTimeOffset.UtcNow,
                },
            ],
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.RemoveAllProjectMembers(projectId, TestContext.Current.CancellationToken);

        var remainingCount = await _mongoCollection.CountDocumentsAsync(
            m => m.ResourceId == projectId.Value,
            cancellationToken: TestContext.Current.CancellationToken
        );
        remainingCount.ShouldBe(0);

        var otherRemaining = await _mongoCollection
            .Find(m => m.ResourceId == otherProjectId.Value)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        otherRemaining.ShouldNotBeNull();
    }
}
