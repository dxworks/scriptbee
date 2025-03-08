using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.UseCases.Analysis;

public interface IRunAnalysisUseCase
{
    Task<AnalysisInfo> Run(
        RunAnalysisCommand command,
        CancellationToken cancellationToken = default
    );
}
