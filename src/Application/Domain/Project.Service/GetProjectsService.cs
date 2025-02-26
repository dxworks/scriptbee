using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Project.Ports;
using ScriptBee.Project.UseCases;

namespace ScriptBee.Project.Service;

public class GetProjectsService(IGetAllProjects getAllProjects, IGetProject getProject)
    : IGetProjectsUseCase
{
    public async Task<IEnumerable<ProjectDetails>> GetAllProjects(
        CancellationToken cancellationToken = default
    )
    {
        return await getAllProjects.GetAll(cancellationToken);
    }

    public async Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> GetProject(
        GetProjectQuery query,
        CancellationToken cancellationToken = default
    )
    {
        return await getProject.GetById(query.Id, cancellationToken);
    }
}
