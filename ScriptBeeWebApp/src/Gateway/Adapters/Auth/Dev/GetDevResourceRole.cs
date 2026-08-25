using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Adapters.Auth.Dev;

public sealed class GetDevResourceRole : IGetResourceRole
{
    public Task<UserRole?> GetRole(
        UserId userId,
        List<UserGroup> groups,
        OneOf<ProjectId> resourceId,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult<UserRole?>(new UserRole("DevAdmin"));
    }
}
