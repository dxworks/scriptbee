using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.UseCases.Project.Analysis;

public interface ITriggerAnalysisUseCase
{
    Task<AnalysisResult> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    );
}
