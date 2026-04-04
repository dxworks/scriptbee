using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance.Allocation;

public interface IGetInstanceStatus
{
    Task<CalculationInstanceStatus> GetStatus(
        InstanceId instanceId,
        CancellationToken cancellationToken
    );
}
