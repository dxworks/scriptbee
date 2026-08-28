using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class DeleteProjectService(
    IDeleteProject deleteProject,
    IRemoveProjectMember removeProjectMember
) : IDeleteProjectUseCase
{
    public async Task DeleteProject(
        DeleteProjectCommand command,
        CancellationToken cancellationToken = default
    )
    {
        await removeProjectMember.RemoveAllProjectMembers(command.Id, cancellationToken);
        await deleteProject.Delete(command.Id, cancellationToken);
    }
}
