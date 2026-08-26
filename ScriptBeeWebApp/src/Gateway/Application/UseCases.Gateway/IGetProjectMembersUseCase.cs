using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public interface IGetProjectMembersUseCase
{
    Task<List<ProjectMember>> GetProjectMembers(
        ProjectId projectId,
        CancellationToken cancellationToken
    );
}
