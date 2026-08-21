using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IResourceMemberService
{
    Task<UserRole> GetResourceRole(
        UserId userId,
        List<UserGroup> groups,
        OneOf<ProjectId> resourceId,
        CancellationToken cancellationToken
    );
}
