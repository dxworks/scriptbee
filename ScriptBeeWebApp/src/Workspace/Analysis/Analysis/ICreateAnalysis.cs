using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Analysis;

public interface ICreateAnalysis
{
    Task<AnalysisInfo> Create(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken = default
    );
}
