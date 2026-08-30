using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Permissions;

public interface IGetAllProjectTokens
{
    Task<List<ProjectToken>> GetAllForProjectId(
        ProjectId projectId,
        CancellationToken cancellationToken
    );
}
