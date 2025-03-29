using DxWorks.ScriptBee.Plugin.Api.Model;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IDownloadAnalysisFileResultsUseCase
{
    Task<
        OneOf<NamedFileStream, AnalysisDoesNotExistsError, AnalysisResultDoesNotExistsError>
    > GetFileResultStream(
        ProjectId projectId,
        AnalysisId analysisId,
        ResultId resultId,
        CancellationToken cancellationToken = default
    );

    Task<OneOf<NamedFileStream, AnalysisDoesNotExistsError>> GetAllFilesZipStream(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
