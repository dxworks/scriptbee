using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Instance.Allocation;

public interface IAllocateInstance
{
    Task<string> Allocate(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        AnalysisInstanceImage image,
        CancellationToken cancellationToken = default
    );
}
