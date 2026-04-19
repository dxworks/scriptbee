using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Gateway.Analysis;

public interface ITriggerAnalysisUseCase
{
    Task<OneOf<AnalysisInfo, InstanceDoesNotExistsError>> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    );
}
