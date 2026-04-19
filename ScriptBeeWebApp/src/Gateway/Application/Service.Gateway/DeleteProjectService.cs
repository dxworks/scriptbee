using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project;

namespace ScriptBee.Service.Project;

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
