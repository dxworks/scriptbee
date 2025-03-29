using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.UseCases.Project.Analysis;

public interface ITriggerAnalysisUseCase
{
    Task<OneOf<AnalysisInfo, InstanceDoesNotExistsError>> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    );
}
