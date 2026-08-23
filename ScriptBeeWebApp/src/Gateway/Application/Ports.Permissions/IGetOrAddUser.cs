using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IGetOrAddUser
{
    Task<UserId> GetOrAddUser(
        string externalUserId,
        string externalUserName,
        CancellationToken cancellationToken
    );
}
