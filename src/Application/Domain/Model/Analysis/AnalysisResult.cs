namespace ScriptBee.Domain.Model.Analysis;

public class AnalysisResult
{
    public required AnalysisId Id { get; init; }
    public required InstanceInfo InstanceInfo { get; init; }
    public required AnalysisStatus Status { get; init; }
    public required AnalysisMetadata Metadata { get; init; }
    public required IEnumerable<Result> Results { get; init; }
    public required IEnumerable<AnalysisError> Errors { get; init; }
    public required DateTimeOffset CreationDate { get; init; }
    public DateTimeOffset? FinishedDate { get; init; }

    private AnalysisResult() { }

    public static AnalysisResult Started(
        AnalysisId analysisId,
        InstanceInfo instanceInfo,
        AnalysisMetadata metadata,
        DateTimeOffset creationDate
    )
    {
        return new AnalysisResult
        {
            Id = analysisId,
            InstanceInfo = instanceInfo,
            Status = AnalysisStatus.Started,
            Metadata = metadata,
            Results = [],
            Errors = [],
            CreationDate = creationDate,
            FinishedDate = null,
        };
    }
}
