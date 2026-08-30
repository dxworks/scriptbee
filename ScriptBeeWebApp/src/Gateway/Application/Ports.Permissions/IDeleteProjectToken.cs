using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Permissions;

public interface IDeleteProjectToken
{
    Task DeleteToken(
        ProjectId projectId,
        ProjectTokenId tokenId,
        CancellationToken cancellationToken
    );
}
