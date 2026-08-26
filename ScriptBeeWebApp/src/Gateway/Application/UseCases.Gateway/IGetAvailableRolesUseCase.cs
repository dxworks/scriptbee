using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public interface IGetAvailableRolesUseCase
{
    Task<List<RoleInfo>> GetAvailableRoles(CancellationToken cancellationToken);
}
