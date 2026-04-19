using System.Threading.Channels;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Analysis;

using AllocateResult = OneOf<InstanceInfo, ProjectDoesNotExistsError>;

public sealed class AllocateProjectInstanceService(
    IGetProject getProject,
    IAllocateInstance allocateInstance,
    IGuidProvider guidProvider,
    IDateTimeProvider dateTimeProvider,
    ICreateProjectInstance createProjectInstance,
    IInstanceTemplateProvider instanceTemplateProvider,
    Channel<InstanceAllocatedEvent> eventsChannel
) : IAllocateProjectInstanceUseCase
{
    public async Task<AllocateResult> Allocate(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(projectId, cancellationToken);

        return await result.Match<Task<AllocateResult>>(
            async details => await CreateAndAllocateInstance(details, cancellationToken),
            error => Task.FromResult<AllocateResult>(error)
        );
    }

    private async Task<AllocateResult> CreateAndAllocateInstance(
        ProjectDetails projectDetails,
        CancellationToken cancellationToken
    )
    {
        var instanceId = new InstanceId(guidProvider.NewGuid());

        var instanceUrl = await allocateInstance.Allocate(
            projectDetails,
            instanceId,
            instanceTemplateProvider.GetTemplate(),
            cancellationToken
        );

        var instanceInfo = new InstanceInfo(
            instanceId,
            projectDetails.Id,
            instanceUrl,
            dateTimeProvider.UtcNow(),
            AnalysisInstanceStatus.NotFound
        );

        var info = await createProjectInstance.Create(instanceInfo, cancellationToken);

        await eventsChannel.Writer.WriteAsync(
            new InstanceAllocatedEvent(projectDetails, instanceInfo),
            cancellationToken
        );

        return info;
    }
}
