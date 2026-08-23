using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public interface IManageUsersUseCase
{
    public Task<UserId> GetUserId(
        string externalUserId,
        string externalUserName,
        CancellationToken cancellationToken
    );
}
