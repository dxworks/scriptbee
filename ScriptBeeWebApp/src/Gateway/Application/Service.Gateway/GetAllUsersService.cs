using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class GetAllUsersService(IGetAllUsers getAllUsers) : IGetAllUsersUseCase
{
    public Task<List<UserInfo>> GetAllUsers(CancellationToken cancellationToken)
    {
        return getAllUsers.GetAllUsers(cancellationToken);
    }
}
