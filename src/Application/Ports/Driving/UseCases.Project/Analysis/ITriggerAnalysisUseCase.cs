using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.UseCases.Project.Analysis;

public interface ITriggerAnalysisUseCase
{
    Task<AnalysisInfo> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    );
}
