using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Analysis;

public record AnalysisInfo(
    AnalysisId Id,
    ProjectId ProjectId,
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
        DateTimeOffset creationDate
    )
    {
        return new AnalysisInfo(
            analysisId,
            projectId,
            AnalysisStatus.Started,
            [],
            [],
            creationDate,
            null
        );
    }
}
