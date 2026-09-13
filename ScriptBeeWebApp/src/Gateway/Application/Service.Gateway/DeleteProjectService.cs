using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway;

public sealed class DeleteProjectService(
    IDeleteProject deleteProject,
    IRemoveProjectMember removeProjectMember,
    IGetAllProjectInstances getAllProjectInstances,
    IDeallocateProjectInstanceUseCase deallocateProjectInstance,
    ILogger<DeleteProjectService> logger
) : IDeleteProjectUseCase
{
    public async Task DeleteProject(
        DeleteProjectCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var projectId = command.Id;

        logger.LogInformation("Deleting project {ProjectId}", projectId);

        await DeallocateAllInstances(projectId, cancellationToken);
        await removeProjectMember.RemoveAllProjectMembers(projectId, cancellationToken);
        await deleteProject.Delete(projectId, cancellationToken);

        logger.LogInformation("Project {ProjectId} deleted", projectId);
    }

    private async Task DeallocateAllInstances(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var instances = await getAllProjectInstances.GetAll(projectId, cancellationToken);

        var tasks = instances
            .Select(instance =>
                deallocateProjectInstance.Deallocate(projectId, instance.Id, cancellationToken)
            )
            .Cast<Task>()
            .ToList();

        if (tasks.Count > 0)
        {
            logger.LogInformation(
                "Deallocating {InstanceCount} instance(s) for project {ProjectId}",
                tasks.Count,
                projectId
            );
        }

        await Task.WhenAll(tasks);
    }
}
