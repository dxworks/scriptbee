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
    IDeallocateProjectInstanceUseCase deallocateProjectInstance
) : IDeleteProjectUseCase
{
    public async Task DeleteProject(
        DeleteProjectCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var projectId = command.Id;

        await DeallocateAllInstances(projectId, cancellationToken);
        await removeProjectMember.RemoveAllProjectMembers(projectId, cancellationToken);
        await deleteProject.Delete(projectId, cancellationToken);
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

        await Task.WhenAll(tasks);
    }
}
