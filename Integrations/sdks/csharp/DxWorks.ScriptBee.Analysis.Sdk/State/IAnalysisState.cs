using ScriptBee.Domain.Model.Analysis;

namespace DxWorks.ScriptBee.Analysis.Sdk.State;

public interface IAnalysisState
{
    Task<AnalysisInfo> CreateAsync(AnalysisInfo analysisInfo, CancellationToken cancellationToken);

    Task UpdateAsync(AnalysisInfo analysisInfo, CancellationToken cancellationToken);
}
