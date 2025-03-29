using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public record WebGetAnalysisResultFile(string Id, string Name, string Type)
{
    public static WebGetAnalysisResultFile Map(AnalysisFileResult result)
    {
        return new WebGetAnalysisResultFile(result.Id.ToString(), result.Name, result.Type);
    }
}
