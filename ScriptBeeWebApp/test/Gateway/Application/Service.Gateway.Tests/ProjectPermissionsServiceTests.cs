using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway.Tests;

public class ProjectPermissionsServiceTests
{
    private readonly IGetProjectPermissions _getProjectPermissions =
        Substitute.For<IGetProjectPermissions>();

    private readonly IGetResourceRole _getResourceRole = Substitute.For<IGetResourceRole>();

    private readonly ProjectPermissionsService _projectPermissionsService;

    public ProjectPermissionsServiceTests()
    {
        _projectPermissionsService = new ProjectPermissionsService(
            _getProjectPermissions,
            _getResourceRole
        );
    }

    [Fact]
    public async Task GiveNoRole_ShouldReturnNull()
    {
        var projectId = ProjectId.FromValue("id");
        var userId = new UserId("user-id");
        List<UserGroup> userGroups = [new("group")];
        var query = new GetProjectPermissionsQuery(projectId, userId, userGroups);
        _getResourceRole
            .GetRole(userId, userGroups, projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<UserRole?>(null));

        var userPermissions = await _projectPermissionsService.GetProjectPermissions(
            query,
            TestContext.Current.CancellationToken
        );

        Assert.Null(userPermissions);
        await _getProjectPermissions
            .DidNotReceive()
            .GetPermissions(
                Arg.Any<ProjectId>(),
                Arg.Any<UserId>(),
                Arg.Any<List<UserGroup>>(),
                Arg.Any<UserRole>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GiveRole_ShouldReturnUserPermissions()
    {
        var projectId = ProjectId.FromValue("id");
        var userId = new UserId("user-id");
        List<UserGroup> userGroups = [new("group")];
        var role = new UserRole("role");
        var query = new GetProjectPermissionsQuery(projectId, userId, userGroups);
        _getResourceRole
            .GetRole(userId, userGroups, projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<UserRole?>(role));
        _getProjectPermissions
            .GetPermissions(projectId, userId, userGroups, role, Arg.Any<CancellationToken>())
            .Returns(["permission"]);

        var userPermissions = await _projectPermissionsService.GetProjectPermissions(
            query,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(role, userPermissions!.Role);
        Assert.Equal(["permission"], userPermissions.Permissions);
    }
}
