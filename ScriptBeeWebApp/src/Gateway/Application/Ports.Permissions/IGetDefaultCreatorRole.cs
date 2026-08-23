using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IGetDefaultCreatorRole
{
    public Task<UserRole> GetRole(CancellationToken cancellationToken);
}
