using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Driven.Project;
using ScriptBee.Ports.Driving.UseCases;
using ScriptBee.Ports.Driving.UseCases.Project;

namespace ScriptBee.Domain.Service.Project;

public class CreateProjectService(ICreateProject createProject, IDateTimeProvider dateTimeProvider)
    : ICreateProjectUseCase
{
    public async Task<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>> CreateProject(CreateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        var projectDetails = new ProjectDetails(ProjectId.Create(command.Id), command.Name, dateTimeProvider.UtcNow());

        var result = await createProject.Create(projectDetails, cancellationToken);

        return result.Match<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
            _ => projectDetails,
            error => error);
    }
}
