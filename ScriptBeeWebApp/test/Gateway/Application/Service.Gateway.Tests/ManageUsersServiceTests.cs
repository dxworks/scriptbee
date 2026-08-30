using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Service.Gateway.Config;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway.Tests;

public class ManageUsersServiceTests : IDisposable
{
    private readonly IOptions<ScriptBeeUserManagementConfig> _userManagementConfigOptions =
        Options.Create(new ScriptBeeUserManagementConfig { UserInfoCacheTimeSeconds = 3 });

    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    private readonly IGetAvailableRoles _getAvailableRoles = Substitute.For<IGetAvailableRoles>();
    private readonly IGetAllUsers _getAllUsers = Substitute.For<IGetAllUsers>();
    private readonly IGetOrAddUser _getOrAddUser = Substitute.For<IGetOrAddUser>();
    private readonly IGetProjectMembers _getProjectMembers = Substitute.For<IGetProjectMembers>();

    private readonly IRemoveProjectMember _removeProjectMember =
        Substitute.For<IRemoveProjectMember>();

    private readonly ISetResourceRole _setResourceRole = Substitute.For<ISetResourceRole>();

    private readonly ManageUsersService _service;

    public ManageUsersServiceTests()
    {
        _service = new ManageUsersService(
            _userManagementConfigOptions,
            _cache,
            _getAvailableRoles,
            _getOrAddUser,
            _getAllUsers,
            _getProjectMembers,
            _setResourceRole,
            _removeProjectMember
        );
    }

    public void Dispose()
    {
        _cache.Clear();
    }

    [Fact]
    public async Task GivenEntryInCache_ThenUserIdIsGetFromCache()
    {
        var userId = new UserId("user-id");
        _cache.Set("external-user-id-existing", userId);

        var id = await _service.GetUserId(
            "external-user-id-existing",
            "user-name",
            TestContext.Current.CancellationToken
        );

        Assert.Equal(userId, id);
        await _getOrAddUser
            .DidNotReceive()
            .GetOrAddUser(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenNoEntryInCache_ThenUserIdIsAdded()
    {
        var userId = new UserId("user-id");
        _getOrAddUser
            .GetOrAddUser("external-user-id", "user-name", Arg.Any<CancellationToken>())
            .Returns(userId);

        var id = await _service.GetUserId(
            "external-user-id",
            "user-name",
            TestContext.Current.CancellationToken
        );

        Assert.Equal(userId, id);
        await _getOrAddUser
            .Received(1)
            .GetOrAddUser("external-user-id", "user-name", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAvailableRoles_ShouldReturnRolesFromPort()
    {
        var expectedRoles = new List<RoleInfo>
        {
            new("owner", "Full control over the project"),
            new("editor", "Can modify project resources"),
            new("viewer", "Read-only access to the project"),
        };
        _getAvailableRoles
            .GetRoles(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedRoles));

        var result = await _service.GetAvailableRoles(TestContext.Current.CancellationToken);

        result.ShouldBeEquivalentTo(expectedRoles);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUsersFromPort()
    {
        var expectedUsers = new List<UserInfo>
        {
            new(new UserId("id-1"), "Alice"),
            new(new UserId("id-2"), "Bob"),
        };
        _getAllUsers
            .GetAllUsers(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedUsers));

        var result = await _service.GetAllUsers(TestContext.Current.CancellationToken);

        result.ShouldBeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task GetProjectMembers_ShouldReturnMembersFromPort()
    {
        var projectId = ProjectId.FromValue("project-id");
        var expectedMembers = new List<ProjectMember>
        {
            new("user-a", "user", new UserRole("owner")),
            new("team-b", "group", new UserRole("viewer")),
        };
        _getProjectMembers
            .GetProjectMembers(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedMembers));

        var result = await _service.GetProjectMembers(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEquivalentTo(expectedMembers);
    }

    [Fact]
    public async Task GetProjectMembers_WhenNoMembers_ShouldReturnEmptyList()
    {
        var projectId = ProjectId.FromValue("empty-project");
        _getProjectMembers
            .GetProjectMembers(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<ProjectMember>()));

        var result = await _service.GetProjectMembers(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task RemoveProjectMember_ShouldCallPortWithCorrectArguments()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new RemoveProjectMemberCommand(projectId, "user-a", "user");

        await _service.RemoveProjectMember(command, TestContext.Current.CancellationToken);

        await _removeProjectMember
            .Received(1)
            .RemoveProjectMember(projectId, "user-a", "user", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProjectMember_ShouldCallSetRoleForMemberWithCorrectArguments()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UpdateProjectMemberCommand(
            projectId,
            "user-a",
            "user",
            new UserRole("editor")
        );

        await _service.UpdateProjectMember(command, TestContext.Current.CancellationToken);

        await _setResourceRole
            .Received(1)
            .SetRoleForMember(
                "user-a",
                "user",
                projectId,
                new UserRole("editor"),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task UpdateProjectMember_ForGroup_ShouldCallSetRoleForMemberWithGroupType()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UpdateProjectMemberCommand(
            projectId,
            "dev-team",
            "group",
            new UserRole("viewer")
        );

        await _service.UpdateProjectMember(command, TestContext.Current.CancellationToken);

        await _setResourceRole
            .Received(1)
            .SetRoleForMember(
                "dev-team",
                "group",
                projectId,
                new UserRole("viewer"),
                Arg.Any<CancellationToken>()
            );
    }
}
