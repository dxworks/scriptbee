using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Analysis;

public interface IUpdateAnalysis
{
    Task<AnalysisInfo> Update(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken = default
    );
}
