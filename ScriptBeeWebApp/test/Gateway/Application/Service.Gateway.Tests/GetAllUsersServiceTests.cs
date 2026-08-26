using NSubstitute;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Service.Gateway.Tests;

public class GetAllUsersServiceTests
{
    private readonly IGetAllUsers _getAllUsers = Substitute.For<IGetAllUsers>();
    private readonly GetAllUsersService _service;

    public GetAllUsersServiceTests()
    {
        _service = new GetAllUsersService(_getAllUsers);
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
}
