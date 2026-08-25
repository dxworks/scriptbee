using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Web.Auth.Dev;

public sealed class GetDevAuthCreatorRole : IGetDefaultCreatorRole
{
    public Task<UserRole> GetRole(CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserRole("DevAdmin"));
    }
}
