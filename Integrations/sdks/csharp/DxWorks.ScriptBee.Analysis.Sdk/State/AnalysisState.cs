using ScriptBee.Analysis;
using ScriptBee.Domain.Model.Analysis;

namespace DxWorks.ScriptBee.Analysis.Sdk.State;

public sealed class AnalysisState(ICreateAnalysis createAnalysis, IUpdateAnalysis updateAnalysis)
    : IAnalysisState
{
    public async Task<AnalysisInfo> CreateAsync(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken
    )
    {
        return await createAnalysis.Create(analysisInfo, cancellationToken);
    }

    public async Task UpdateAsync(AnalysisInfo analysisInfo, CancellationToken cancellationToken)
    {
        await updateAnalysis.Update(analysisInfo, cancellationToken);
    }
}
