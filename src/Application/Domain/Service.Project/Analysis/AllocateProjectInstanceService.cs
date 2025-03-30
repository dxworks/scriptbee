using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

using AllocateResult = OneOf<InstanceInfo, ProjectDoesNotExistsError>;

public class AllocateProjectInstanceService(
    IGetProject getProject,
    IAllocateInstance allocateInstance,
    IGuidProvider guidProvider,
    IDateTimeProvider dateTimeProvider,
    ICreateProjectInstance createProjectInstance,
    IInstanceTemplateProvider instanceTemplateProvider
) : IAllocateProjectInstanceUseCase
{
    public async Task<AllocateResult> Allocate(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(projectId, cancellationToken);

        return await result.Match<Task<AllocateResult>>(
            async _ => await CreateAndAllocateInstance(projectId, cancellationToken),
            error => Task.FromResult<AllocateResult>(error)
        );
    }

    private async Task<AllocateResult> CreateAndAllocateInstance(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var instanceId = new InstanceId(guidProvider.NewGuid());

        var instanceUrl = await allocateInstance.Allocate(
            instanceId,
            instanceTemplateProvider.GetTemplate(),
            cancellationToken
        );

        var instanceInfo = new InstanceInfo(
            instanceId,
            projectId,
            instanceUrl,
            dateTimeProvider.UtcNow()
        );

        return await createProjectInstance.Create(instanceInfo, cancellationToken);
    }
}
