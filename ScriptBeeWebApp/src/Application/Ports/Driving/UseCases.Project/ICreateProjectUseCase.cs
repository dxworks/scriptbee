using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project;

public interface ICreateProjectUseCase
{
    Task<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>> CreateProject(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default
    );
}
