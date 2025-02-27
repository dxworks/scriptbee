namespace ScriptBee.Domain.Model.Analysis;

public record AnalysisResult(
    AnalysisId Id,
    InstanceInfo InstanceInfo,
    AnalysisStatus Status,
    AnalysisMetadata Metadata,
    IEnumerable<Result> Results,
    IEnumerable<AnalysisError> Errors,
    DateTimeOffset CreationDate,
    DateTimeOffset? FinishedDate
)
{
    public static AnalysisResult Started(
        AnalysisId analysisId,
        InstanceInfo instanceInfo,
        AnalysisMetadata metadata,
        DateTimeOffset creationDate
    )
    {
        return new AnalysisResult(
            analysisId,
            instanceInfo,
            AnalysisStatus.Started,
            metadata,
            [],
            [],
            creationDate,
            null
        );
    }
}
