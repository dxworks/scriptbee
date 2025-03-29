using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetAnalysisResultsService : IGetAnalysisResultsUseCase
{
    public Task<OneOf<string, AnalysisDoesNotExistsError>> GetConsoleResult(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<
        OneOf<IEnumerable<AnalysisErrorResult>, AnalysisDoesNotExistsError>
    > GetErrorResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<IEnumerable<AnalysisFileResult>, AnalysisDoesNotExistsError>> GetFileResults(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
