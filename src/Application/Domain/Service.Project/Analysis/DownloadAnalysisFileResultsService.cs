using DxWorks.ScriptBee.Plugin.Api.Model;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class DownloadAnalysisFileResultsService : IDownloadAnalysisFileResultsUseCase
{
    public Task<
        OneOf<NamedFileStream, AnalysisDoesNotExistsError, AnalysisResultDoesNotExistsError>
    > GetFileResultStream(
        ProjectId projectId,
        AnalysisId analysisId,
        ResultId resultId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<OneOf<NamedFileStream, AnalysisDoesNotExistsError>> GetAllFilesZipStream(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
