using OneOf;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Driving.UseCases.Project;

public interface IGetProjectsUseCase
{
    Task<List<ProjectDetails>> GetAllProjects(CancellationToken cancellationToken = default);

    Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> GetProject(GetProjectQuery query,
        CancellationToken cancellationToken = default);
}
