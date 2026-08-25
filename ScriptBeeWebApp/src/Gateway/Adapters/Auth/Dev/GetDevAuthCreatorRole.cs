using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Adapters.Auth.Dev;

public sealed class GetDevAuthCreatorRole : IGetDefaultCreatorRole
{
    public Task<UserRole> GetRole(CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserRole("DevAdmin"));
    }
}
