using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Service.Gateway.Config;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class ManageUsersService(
    IOptions<ScriptBeeUserManagementConfig> userManagementConfigOptions,
    IMemoryCache cache,
    IGetOrAddUser getOrAddUser
) : IManageUsersUseCase
{
    public async Task<UserId> GetUserId(
        string externalUserId,
        string externalUserName,
        CancellationToken cancellationToken
    )
    {
        var userId = await cache.GetOrCreateAsync(
            externalUserId,
            _ => getOrAddUser.GetOrAddUser(externalUserId, externalUserName, cancellationToken),
            GetCacheOptions()
        );

        return userId;
    }

    private MemoryCacheEntryOptions GetCacheOptions()
    {
        var config = userManagementConfigOptions.Value;
        return new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(config.UserInfoCacheTimeSeconds),
        };
    }
}
