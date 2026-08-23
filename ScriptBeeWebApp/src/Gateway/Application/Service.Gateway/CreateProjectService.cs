using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public class CreateProjectService(
    ICreateProject createProject,
    IDateTimeProvider dateTimeProvider,
    IGetDefaultCreatorRole getDefaultCreatorRole,
    ISetResourceRole setResourceRole
) : ICreateProjectUseCase
{
    public async Task<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>> CreateProject(
        CreateProjectCommand command,
        CancellationToken cancellationToken
    )
    {
        var projectDetails = new ProjectDetails(
            ProjectId.Create(command.Id),
            command.Name,
            dateTimeProvider.UtcNow(),
            new Dictionary<string, List<FileData>>(),
            new Dictionary<string, List<FileData>>(),
            [],
            []
        );

        var result = await createProject.Create(projectDetails, cancellationToken);

        if (result.IsT0)
        {
            await AssignDefaultRole(projectDetails.Id, command.UserId, cancellationToken);
        }

        return result.Match<OneOf<ProjectDetails, ProjectIdAlreadyInUseError>>(
            _ => projectDetails,
            error => error
        );
    }

    private async Task AssignDefaultRole(
        ProjectId projectId,
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        var creatorRole = await getDefaultCreatorRole.GetRole(cancellationToken);

        await setResourceRole.SetRoleForUser(userId, projectId, creatorRole, cancellationToken);
    }
}
