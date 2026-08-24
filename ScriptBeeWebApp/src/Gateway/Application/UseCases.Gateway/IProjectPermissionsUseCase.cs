using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public interface IProjectPermissionsUseCase
{
    public Task<UserPermissions?> GetProjectPermissions(
        GetProjectPermissionsQuery getProjectPermissions,
        CancellationToken cancellationToken
    );
}
