using ScriptBee.Domain.Model.Projects;

namespace ScriptBee.Ports.Driving.UseCases.Projects;

public interface ICreateProjectUseCase
{
    Task<Project> CreateProject(CreateProjectCommand command, CancellationToken cancellationToken = default);
}
