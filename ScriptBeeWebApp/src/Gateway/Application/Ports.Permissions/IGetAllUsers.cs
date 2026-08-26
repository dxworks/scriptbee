using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IGetAllUsers
{
    Task<List<UserInfo>> GetAllUsers(CancellationToken cancellationToken);
}
