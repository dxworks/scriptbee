using NSubstitute;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway.Tests;

public class GlobalPermissionsServiceTests
{
    private readonly IGetGlobalPermissions _getGlobalPermissions =
        Substitute.For<IGetGlobalPermissions>();

    private readonly GlobalPermissionsService _globalPermissionsService;

    public GlobalPermissionsServiceTests()
    {
        _globalPermissionsService = new GlobalPermissionsService(_getGlobalPermissions);
    }

    [Fact]
    public async Task GivenGlobalPermissions_ShouldReturnThem()
    {
        // Arrange
        var userId = new UserId("user-id");
        List<UserGroup> userGroups = [new("group")];
        var query = new GetGlobalPermissionsQuery(userId, userGroups);
        var expectedPermissions = new List<string>
        {
            "project:create",
            "gateway_plugin:management",
        };

        _getGlobalPermissions
            .GetPermissions(userId, userGroups, Arg.Any<CancellationToken>())
            .Returns(expectedPermissions);

        // Act
        var result = await _globalPermissionsService.GetGlobalPermissions(
            query,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expectedPermissions, result);
    }
}
