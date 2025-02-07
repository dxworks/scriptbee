using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Driven.Projects;
using ScriptBee.Ports.Driving.UseCases.Projects;

namespace ScriptBee.Domain.Service.Projects;

public class CreateProjectService(ICreateProject createProject) : ICreateProjectUseCase
{
    public async Task<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>> CreateProject(CreateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        var projectDetails = new ProjectDetails(ProjectId.FromName(command.Name), command.Name);

        var result = await createProject.CreateProject(projectDetails, cancellationToken);

        return result.Match<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
            _ => projectDetails,
            error => error);
    }
}
