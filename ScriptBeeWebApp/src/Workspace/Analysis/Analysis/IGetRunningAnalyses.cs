using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Analysis;

public interface IGetRunningAnalyses
{
    Task<IEnumerable<AnalysisInfo>> GetRunning(CancellationToken cancellationToken);
}
