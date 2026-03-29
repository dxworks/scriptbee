using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public sealed record WebAnalysisStatus(
    string InstanceId,
    string Status,
    string ScriptId,
    DateTimeOffset CreationDate,
    DateTimeOffset? FinishedDate,
    List<WebAnalysisError>? Errors
)
{
    public static WebAnalysisStatus Map(AnalysisInfo analysisInfo)
    {
        var errors = analysisInfo.Errors.ToList();
        return new WebAnalysisStatus(
            analysisInfo.InstanceId.ToString(),
            analysisInfo.Status.ToString(),
            analysisInfo.ScriptId.ToString(),
            analysisInfo.CreationDate,
            analysisInfo.FinishedDate,
            errors.Count == 0
                ? null
                : errors.Select(error => new WebAnalysisError(error.Message)).ToList()
        );
    }
}
