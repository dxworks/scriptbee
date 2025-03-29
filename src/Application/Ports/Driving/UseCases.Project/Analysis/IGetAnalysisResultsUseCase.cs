using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IGetAnalysisResultsUseCase
{
    Task<OneOf<string, AnalysisDoesNotExistsError>> GetConsoleResult(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );

    Task<OneOf<IEnumerable<AnalysisErrorResult>, AnalysisDoesNotExistsError>> GetErrorResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );

    Task<OneOf<IEnumerable<AnalysisFileResult>, AnalysisDoesNotExistsError>> GetFileResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
