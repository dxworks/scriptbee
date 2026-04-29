using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Analysis;

public interface IAnalysisStatusMonitorService : IAnalysisTracker
{
    Task SeedRunningAnalyses(CancellationToken cancellationToken);
    Task Monitor(CancellationToken cancellationToken);
}
