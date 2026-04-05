using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance.Allocation;

public interface IGetInstanceStatus
{
    Task<AnalysisInstanceStatus> GetStatus(
        InstanceId instanceId,
        CancellationToken cancellationToken
    );
}
