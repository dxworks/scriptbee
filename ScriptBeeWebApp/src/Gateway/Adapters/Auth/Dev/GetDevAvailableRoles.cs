using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Adapters.Auth.Dev;

public sealed class GetDevAvailableRoles : IGetAvailableRoles
{
    public Task<List<RoleInfo>> GetRoles(CancellationToken cancellationToken)
    {
        return Task.FromResult(
            new List<RoleInfo>
            {
                new("owner", "Full control over the project"),
                new("editor", "Can modify project resources"),
                new("viewer", "Read-only access to the project"),
            }
        );
    }
}
