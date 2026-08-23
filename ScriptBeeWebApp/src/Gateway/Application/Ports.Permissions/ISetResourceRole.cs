using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface ISetResourceRole
{
    Task SetRoleForUser(
        UserId userId,
        ProjectId project,
        UserRole role,
        CancellationToken cancellationToken
    );
}
