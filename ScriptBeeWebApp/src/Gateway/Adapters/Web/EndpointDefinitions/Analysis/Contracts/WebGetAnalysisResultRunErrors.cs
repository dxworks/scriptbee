using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public sealed record WebGetAnalysisResultRunErrors(IEnumerable<WebAnalysisResultRunError> Data)
{
    public static WebGetAnalysisResultRunErrors Map(IEnumerable<AnalysisErrorResult> errorResults)
    {
        return new WebGetAnalysisResultRunErrors(
            errorResults.Select(WebAnalysisResultRunError.Map)
        );
    }
}
