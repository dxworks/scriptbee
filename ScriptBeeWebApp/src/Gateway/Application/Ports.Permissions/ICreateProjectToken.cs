using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface ICreateProjectToken
{
    Task<ProjectToken> CreateToken(
        ProjectId projectId,
        string tokenHash,
        string? description,
        UserRole role,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken
    );
}
