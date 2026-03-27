using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Ports.Instance.Allocation;

public interface IAllocateInstance
{
    Task<string> Allocate(
        InstanceId instanceId,
        AnalysisInstanceImage image,
        CancellationToken cancellationToken = default
    );
}
