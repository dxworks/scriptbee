using ScriptBee.Project.Ports;
using ScriptBee.Project.UseCases;

namespace ScriptBee.Project.Service;

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
