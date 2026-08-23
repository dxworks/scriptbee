using ScriptBee.Domain.Model.User;

namespace ScriptBee.Web.Auth;

public interface IGetDefaultCreatorRole
{
    public Task<UserRole> GetRole(CancellationToken cancellationToken);
}
