using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class ProjectPermissionsService(
    IGetProjectPermissions getProjectPermissions,
    IGetResourceRole getResourceRole
) : IProjectPermissionsUseCase
{
    public async Task<UserPermissions?> GetProjectPermissions(
        GetProjectPermissionsQuery query,
        CancellationToken cancellationToken
    )
    {
        var resourceRole = await getResourceRole.GetRole(
            query.UserId,
            query.Groups,
            OneOf<ProjectId>.FromT0(query.ProjectId),
            cancellationToken
        );

        if (!resourceRole.HasValue)
        {
            return null;
        }

        var permissions = await getProjectPermissions.GetPermissions(
            query.ProjectId,
            query.UserId,
            query.Groups,
            resourceRole.Value,
            cancellationToken
        );

        return new UserPermissions(resourceRole.Value, permissions);
    }
}
