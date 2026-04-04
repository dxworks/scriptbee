using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

using DeallocateResult = OneOf<Success, ProjectDoesNotExistsError>;

public sealed class DeallocateProjectInstanceService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    IDeallocateInstance deallocateInstance,
    IDeleteProjectInstance deleteProjectInstance
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
            async _ => await FindAndDeallocateInstance(instanceId, cancellationToken),
            error => Task.FromResult<DeallocateResult>(error)
        );
    }

    private async Task<DeallocateResult> FindAndDeallocateInstance(
        InstanceId instanceId,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match<Task<DeallocateResult>>(
            async info =>
            {
                await DeallocateInstance(info, cancellationToken);
                return new Success();
            },
            _ => Task.FromResult<DeallocateResult>(new Success())
        );
    }

    private async Task DeallocateInstance(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        await deallocateInstance.Deallocate(instanceInfo, cancellationToken);

        await deleteProjectInstance.Delete(instanceInfo, cancellationToken);
    }
}
