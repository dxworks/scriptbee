using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Driven.Project;
using ScriptBee.Ports.Driving.UseCases.Project;

namespace ScriptBee.Domain.Service.Project;

public class GetProjectsService(IGetAllProjects getAllProjects, IGetProject getProject) : IGetProjectsUseCase
{
    public async Task<IEnumerable<ProjectDetails>> GetAllProjects(CancellationToken cancellationToken = default)
    {
        return await getAllProjects.GetAll(cancellationToken);
    }

    public async Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> GetProject(GetProjectQuery query,
        CancellationToken cancellationToken = default)
    {
        return await getProject.GetById(query.Id, cancellationToken);
    }
}
