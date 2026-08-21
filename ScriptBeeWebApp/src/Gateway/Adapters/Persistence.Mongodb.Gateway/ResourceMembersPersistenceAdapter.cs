using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Persistence.Mongodb;

public sealed class ResourceMembersPersistenceAdapter : IResourceMemberService
{
    public Task<UserRole> GetResourceRole(
        UserId userId,
        List<UserGroup> groups,
        OneOf<ProjectId> resourceId,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
