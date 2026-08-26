using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class GlobalPermissionsService(IGetGlobalPermissions getGlobalPermissions)
    : IGlobalPermissionsUseCase
{
    public Task<List<string>> GetGlobalPermissions(
        GetGlobalPermissionsQuery query,
        CancellationToken cancellationToken
    )
    {
        return getGlobalPermissions.GetPermissions(query.UserId, query.Groups, cancellationToken);
    }
}
