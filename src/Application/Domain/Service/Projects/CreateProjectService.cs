using ScriptBee.Domain.Model.Projects;
using ScriptBee.Ports.Driving.UseCases.Projects;

namespace ScriptBee.Domain.Service.Projects;

public class CreateProjectService : ICreateProjectUseCase
{
    public Task<Project> CreateProject(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        // TODO: implement this properly 
        return Task.FromResult(new Project(ProjectId.FromName(command.Name), command.Name));
    }
}
