using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class GetProjectsService(
    IGetAllProjects getAllProjects,
    IGetProject getProject,
    IGetGlobalPermissions getGlobalPermissions,
    IGetResourceRole getResourceRole
) : IGetProjectsUseCase
{
    private const string ViewAllProjectsPermission = "project:view_all";

    public async Task<IEnumerable<ProjectDetails>> GetAllProjects(
        GetAllProjectsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var globalPermissions = await getGlobalPermissions.GetPermissions(
            query.UserId,
            query.Groups,
            cancellationToken
        );

        var allProjects = await getAllProjects.GetAll(cancellationToken);

        if (globalPermissions.Contains(ViewAllProjectsPermission, StringComparer.OrdinalIgnoreCase))
        {
            return allProjects;
        }

        return await FilterProjectsByUserAccess(query, allProjects, cancellationToken);
    }

    public async Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> GetProject(
        GetProjectQuery query,
        CancellationToken cancellationToken = default
    )
    {
        return await getProject.GetById(query.Id, cancellationToken);
    }

    private async Task<IEnumerable<ProjectDetails>> FilterProjectsByUserAccess(
        GetAllProjectsQuery query,
        IEnumerable<ProjectDetails> projects,
        CancellationToken cancellationToken
    )
    {
        var accessibleProjectIds = await getResourceRole.GetAccessibleProjectIds(
            query.UserId,
            query.Groups,
            cancellationToken
        );

        var ids = new HashSet<ProjectId>(accessibleProjectIds);
        return projects.Where(project => ids.Contains(project.Id));
    }
}
