using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Instance;

public interface IAllocateInstance
{
    Task<string> Allocate(
        AnalysisInstanceImage image,
        CancellationToken cancellationToken = default
    );
}
