using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Permissions;

public interface IGetProjectTokenByHash
{
    Task<ProjectToken?> GetTokenByHash(string tokenHash, CancellationToken cancellationToken);
}
