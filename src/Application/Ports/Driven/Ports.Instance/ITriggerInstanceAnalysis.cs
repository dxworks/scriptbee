using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Ports.Instance;

public interface ITriggerInstanceAnalysis
{
    Task<AnalysisInfo> Trigger(
        InstanceInfo instanceInfo,
        ScriptId scriptId,
        CancellationToken cancellationToken = default
    );
}
