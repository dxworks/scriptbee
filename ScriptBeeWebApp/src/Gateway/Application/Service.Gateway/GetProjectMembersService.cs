using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class GetProjectMembersService(IGetProjectMembers getProjectMembers)
    : IGetProjectMembersUseCase
{
    public Task<List<ProjectMember>> GetProjectMembers(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        return getProjectMembers.GetProjectMembers(projectId, cancellationToken);
    }
}
