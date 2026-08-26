using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.Ports.Permissions;

public interface IGetProjectMembers
{
    Task<List<ProjectMember>> GetProjectMembers(
        ProjectId projectId,
        CancellationToken cancellationToken
    );
}
