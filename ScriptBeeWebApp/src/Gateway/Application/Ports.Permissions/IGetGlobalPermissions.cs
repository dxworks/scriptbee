using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IGetGlobalPermissions
{
    Task<List<string>> GetPermissions(
        UserId userId,
        List<UserGroup> groups,
        CancellationToken cancellationToken
    );
}
