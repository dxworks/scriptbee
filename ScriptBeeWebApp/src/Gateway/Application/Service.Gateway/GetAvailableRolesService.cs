using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class GetAvailableRolesService(IGetAvailableRoles getAvailableRoles)
    : IGetAvailableRolesUseCase
{
    public Task<List<RoleInfo>> GetAvailableRoles(CancellationToken cancellationToken)
    {
        return getAvailableRoles.GetRoles(cancellationToken);
    }
}
