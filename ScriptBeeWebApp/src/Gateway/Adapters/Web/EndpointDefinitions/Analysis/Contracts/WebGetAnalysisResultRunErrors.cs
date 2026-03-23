using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public record WebGetAnalysisResultRunErrors(IEnumerable<WebAnalysisResultRunError> Errors)
{
    public static WebGetAnalysisResultRunErrors Map(IEnumerable<AnalysisErrorResult> errorResults)
    {
        return new WebGetAnalysisResultRunErrors(
            errorResults.Select(WebAnalysisResultRunError.Map)
        );
    }
}
