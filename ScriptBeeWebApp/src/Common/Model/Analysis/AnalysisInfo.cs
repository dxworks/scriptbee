using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Analysis;

public record AnalysisInfo(
    AnalysisId Id,
    ProjectId ProjectId,
    ScriptId ScriptId,
    FileId? ScriptFileId,
    AnalysisStatus Status,
    IEnumerable<ResultSummary> Results,
    IEnumerable<AnalysisError> Errors,
    DateTimeOffset CreationDate,
    DateTimeOffset? FinishedDate
)
{
    public static AnalysisInfo Started(
        AnalysisId analysisId,
        ProjectId projectId,
        ScriptId scriptId,
        DateTimeOffset creationDate
    )
    {
        return new AnalysisInfo(
            analysisId,
            projectId,
            scriptId,
            null,
            AnalysisStatus.Started,
            [],
            [],
            creationDate,
            null
        );
    }

    public static AnalysisInfo FailedToStart(
        AnalysisId analysisId,
        ProjectId projectId,
        ScriptId scriptId,
        DateTimeOffset date,
        string message
    )
    {
        return new AnalysisInfo(
            analysisId,
            projectId,
            scriptId,
            null,
            AnalysisStatus.Finished,
            [],
            [new AnalysisError(message)],
            date,
            date
        );
    }

    public AnalysisInfo Failed(DateTimeOffset finishedDate, string message)
    {
        return this with
        {
            Status = AnalysisStatus.Finished,
            Errors = [new AnalysisError(message)],
            FinishedDate = finishedDate,
        };
    }

    public AnalysisInfo Success(DateTimeOffset finishedDate, IEnumerable<ResultSummary> results)
    {
        return this with
        {
            Status = AnalysisStatus.Finished,
            Results = results,
            FinishedDate = finishedDate,
        };
    }
}
