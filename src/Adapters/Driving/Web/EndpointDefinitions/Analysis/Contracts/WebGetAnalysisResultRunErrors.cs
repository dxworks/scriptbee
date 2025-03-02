namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public record WebGetAnalysisResultRunErrors(IEnumerable<WebAnalysisResultRunError> Errors);
