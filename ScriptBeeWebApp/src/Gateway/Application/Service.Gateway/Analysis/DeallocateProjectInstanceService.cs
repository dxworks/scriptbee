using System.Threading.Channels;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Analysis;

using DeallocateResult = OneOf<Success, ProjectDoesNotExistsError>;

public sealed class DeallocateProjectInstanceService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    IDeallocateInstance deallocateInstance,
    IDeleteProjectInstance deleteProjectInstance,
    Channel<InstanceDeallocatedEvent> eventsChannel
) : IDeallocateProjectInstanceUseCase
{
    public async Task<DeallocateResult> Deallocate(
        ProjectId projectId,
        InstanceId instanceId,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(projectId, cancellationToken);

        return await result.Match<Task<DeallocateResult>>(
            async details =>
                await FindAndDeallocateInstance(details, instanceId, cancellationToken),
            error => Task.FromResult<DeallocateResult>(error)
        );
    }

    private async Task<DeallocateResult> FindAndDeallocateInstance(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match<Task<DeallocateResult>>(
            async info =>
            {
                await DeallocateInstance(projectDetails, info, cancellationToken);
                return new Success();
            },
            _ => Task.FromResult<DeallocateResult>(new Success())
        );
    }

    private async Task DeallocateInstance(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        await deallocateInstance.Deallocate(instanceInfo, cancellationToken);

        await deleteProjectInstance.Delete(instanceInfo, cancellationToken);

        await eventsChannel.Writer.WriteAsync(
            new InstanceDeallocatedEvent(projectDetails, instanceInfo),
            cancellationToken
        );
    }
}
