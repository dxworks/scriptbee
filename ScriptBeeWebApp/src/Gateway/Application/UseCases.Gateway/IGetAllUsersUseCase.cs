using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public interface IGetAllUsersUseCase
{
    Task<List<UserInfo>> GetAllUsers(CancellationToken cancellationToken);
}
