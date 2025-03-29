using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
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
    ICreateProjectInstance createProjectInstance
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
        // TODO FIXIT(#63): make image configurable
        var instanceUrl = await allocateInstance.Allocate(
            new AnalysisInstanceImage("scriptbee/analysis:latest"),
            cancellationToken
        );

        var instanceInfo = new InstanceInfo(
            new InstanceId(guidProvider.NewGuid()),
            projectId,
            instanceUrl,
            dateTimeProvider.UtcNow()
        );

        return await createProjectInstance.Create(instanceInfo, cancellationToken);
    }
}
