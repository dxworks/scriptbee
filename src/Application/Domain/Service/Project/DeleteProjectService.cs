using ScriptBee.Ports.Driven.Project;
using ScriptBee.Ports.Driving.UseCases.Project;

namespace ScriptBee.Domain.Service.Project;

public class DeleteProjectService(IDeleteProject deleteProject) : IDeleteProjectUseCase
{
    public async Task DeleteProject(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        await deleteProject.Delete(command.Id, cancellationToken);
    }
}
