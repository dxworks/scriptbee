using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IGetProjectPermissions
{
    Task<List<string>> GetPermissions(
        ProjectId projectId,
        UserId userId,
        List<UserGroup> groups,
        UserRole userRole,
        CancellationToken cancellationToken
    );
}
