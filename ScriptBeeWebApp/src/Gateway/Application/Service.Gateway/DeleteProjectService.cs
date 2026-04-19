using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public class DeleteProjectService(IDeleteProject deleteProject) : IDeleteProjectUseCase
{
    public async Task DeleteProject(
        DeleteProjectCommand command,
        CancellationToken cancellationToken = default
    )
    {
        await deleteProject.Delete(command.Id, cancellationToken);
    }
}
