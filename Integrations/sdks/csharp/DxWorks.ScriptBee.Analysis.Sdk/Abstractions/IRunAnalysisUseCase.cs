using ScriptBee.Domain.Model.Analysis;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface IRunAnalysisUseCase
{
    Task<AnalysisInfo> Run(
        RunAnalysisCommand command,
        CancellationToken cancellationToken = default
    );
}
