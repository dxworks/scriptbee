using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using ReloadContextResult = OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>;

public class ReloadInstanceContextService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext,
    ILoadInstanceContext loadInstanceContext,
    ILinkInstanceContext linkInstanceContext
) : IReloadInstanceContextUseCase
{
    public async Task<ReloadContextResult> Reload(
        ReloadContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<ReloadContextResult>>(
            async projectDetails =>
                await ReloadContext(projectDetails, command.InstanceId, cancellationToken),
            error => Task.FromResult<ReloadContextResult>(error)
        );
    }

    private async Task<ReloadContextResult> ReloadContext(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match<Task<ReloadContextResult>>(
            async instanceInfo =>
            {
                await ReloadContext(projectDetails, instanceInfo, cancellationToken);
                return new Unit();
            },
            error => Task.FromResult<ReloadContextResult>(error)
        );
    }

    private async Task ReloadContext(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        await clearInstanceContext.Clear(instanceInfo, cancellationToken);
        await loadInstanceContext.Load(
            instanceInfo,
            projectDetails.LoadedFiles.ToDictionary(x => x.Key, x => x.Value.Select(f => f.Id)),
            cancellationToken
        );
        await linkInstanceContext.Link(instanceInfo, projectDetails.Linkers, cancellationToken);
    }
}
