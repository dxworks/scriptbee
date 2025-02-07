using OneOf;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driving.UseCases.Projects;

public interface ICreateProjectUseCase
{
    Task<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>> CreateProject(CreateProjectCommand command,
        CancellationToken cancellationToken = default);
}
