using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IGetAvailableRoles
{
    Task<List<RoleInfo>> GetRoles(CancellationToken cancellationToken);
}
