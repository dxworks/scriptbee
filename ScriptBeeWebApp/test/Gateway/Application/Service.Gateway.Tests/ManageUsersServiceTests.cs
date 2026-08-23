using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Service.Gateway.Config;

namespace ScriptBee.Service.Gateway.Tests;

public class ManageUsersServiceTests : IDisposable
{
    private readonly IOptions<ScriptBeeUserManagementConfig> _userManagementConfigOptions =
        Options.Create(new ScriptBeeUserManagementConfig { UserInfoCacheTimeSeconds = 3 });

    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly IGetOrAddUser _getOrAddUser = Substitute.For<IGetOrAddUser>();

    private readonly ManageUsersService _manageUsersService;

    public ManageUsersServiceTests()
    {
        _manageUsersService = new ManageUsersService(
            _userManagementConfigOptions,
            _cache,
            _getOrAddUser
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

        var id = await _manageUsersService.GetUserId(
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

        var id = await _manageUsersService.GetUserId(
            "external-user-id",
            "user-name",
            TestContext.Current.CancellationToken
        );

        Assert.Equal(userId, id);
        await _getOrAddUser
            .Received(1)
            .GetOrAddUser("external-user-id", "user-name", Arg.Any<CancellationToken>());
    }
}
