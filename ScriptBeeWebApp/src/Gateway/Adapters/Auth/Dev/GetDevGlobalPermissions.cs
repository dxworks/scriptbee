using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Adapters.Auth.Dev;

public sealed class GetDevGlobalPermissions : IGetGlobalPermissions
{
    public Task<List<string>> GetPermissions(
        UserId userId,
        List<UserGroup> groups,
        CancellationToken cancellationToken
    )
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "project:create",
            "gateway_plugin:management",
        };

        return Task.FromResult(permissions.ToList());
    }
}
