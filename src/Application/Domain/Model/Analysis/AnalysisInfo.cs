using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Analysis;

public record AnalysisInfo(
    AnalysisId Id,
    ProjectId ProjectId,
    ScriptId ScriptId,
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
            AnalysisStatus.Finished,
            [],
            [new AnalysisError(message)],
            date,
            date
        );
    }
}
