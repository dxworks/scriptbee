using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public sealed record WebAnalysisInfo(
    string Id,
    string InstanceId,
    string Status,
    string ScriptId,
    string? ScriptFileId,
    DateTimeOffset CreationDate,
    DateTimeOffset? FinishedDate,
    List<WebAnalysisError>? Errors
)
{
    public static WebAnalysisInfo Map(AnalysisInfo analysisInfo)
    {
        var errors = analysisInfo.Errors.ToList();
        return new WebAnalysisInfo(
            analysisInfo.Id.ToString(),
            analysisInfo.InstanceId.ToString(),
            analysisInfo.Status.ToString(),
            analysisInfo.ScriptId.ToString(),
            analysisInfo.ScriptFileId?.ToString(),
            analysisInfo.CreationDate,
            analysisInfo.FinishedDate,
            errors.Count == 0
                ? null
                : errors.Select(error => new WebAnalysisError(error.Message)).ToList()
        );
    }
}
