using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public record WebAnalysisResultRunError(string Title, string Message, string Severity)
{
    public static WebAnalysisResultRunError Map(AnalysisErrorResult result)
    {
        return new WebAnalysisResultRunError(result.Title, result.Message, result.Severity);
    }
}
