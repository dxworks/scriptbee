using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public interface IProjectPermissionsUseCase
{
    Task<UserPermissions?> GetProjectPermissions(
        GetProjectPermissionsQuery getProjectPermissions,
        CancellationToken cancellationToken
    );
}
