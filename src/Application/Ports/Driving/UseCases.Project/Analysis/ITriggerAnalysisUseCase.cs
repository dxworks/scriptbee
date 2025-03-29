using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface ITriggerAnalysisUseCase
{
    Task<OneOf<AnalysisInfo, ProjectDoesNotExistsError, InstanceDoesNotExistsError>> Trigger(
        TriggerAnalysisCommand command,
        CancellationToken cancellationToken = default
    );
}
