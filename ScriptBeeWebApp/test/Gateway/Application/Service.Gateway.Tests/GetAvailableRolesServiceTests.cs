using NSubstitute;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Service.Gateway.Tests;

public class GetAvailableRolesServiceTests
{
    private readonly IGetAvailableRoles _getAvailableRoles = Substitute.For<IGetAvailableRoles>();
    private readonly GetAvailableRolesService _service;

    public GetAvailableRolesServiceTests()
    {
        _service = new GetAvailableRolesService(_getAvailableRoles);
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
}
