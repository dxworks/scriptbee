using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class TriggerAnalysisService(
    IDateTimeProvider dateTimeProvider,
    IGuidProvider guidProvider,
    IGetAllProjectInstances getAllProjectInstances,
    IAllocateInstance allocateInstance
) : ITriggerAnalysisUseCase
{
    public async Task<AnalysisResult> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var instanceInfo = await GetFirstPermanentInstanceOrAllocate(command, cancellationToken);

        // TODO FIXIT: call analysis instance

        return AnalysisResult.Started(
            AnalysisId.FromGuid(guidProvider.NewGuid()),
            instanceInfo,
            new AnalysisMetadata(command.Loaders, command.Linkers),
            dateTimeProvider.UtcNow()
        );
    }

    private async Task<InstanceInfo> GetFirstPermanentInstanceOrAllocate(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken
    )
    {
        var allInstances = await getAllProjectInstances.GetAll(
            command.ProjectId,
            cancellationToken
        );

        var calculationInstanceInfo = allInstances.FirstOrDefault();

        if (calculationInstanceInfo == null)
        {
            return await CreateInstance(command.ProjectId, command.Image, cancellationToken);
        }

        return calculationInstanceInfo;
    }

    private async Task<InstanceInfo> CreateInstance(
        ProjectId projectId,
        AnalysisInstanceImage image,
        CancellationToken cancellationToken
    )
    {
        var instanceUrl = await allocateInstance.Allocate(image, cancellationToken);

        return new InstanceInfo(
            InstanceId.FromGuid(guidProvider.NewGuid()),
            projectId,
            instanceUrl,
            dateTimeProvider.UtcNow()
        );
    }
}
