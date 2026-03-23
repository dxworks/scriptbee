using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;

public record WebRunAnalysisResponse(
    string Id,
    string ProjectId,
    string ScriptId,
    string Status,
    DateTimeOffset CreationDate
)
{
    public static WebRunAnalysisResponse FromAnalysisInfo(AnalysisInfo info)
    {
        return new WebRunAnalysisResponse(
            info.Id.ToString(),
            info.ProjectId.Value,
            info.ScriptId.ToString(),
            info.Status.Value,
            info.CreationDate
        );
    }
}
