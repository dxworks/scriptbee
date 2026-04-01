namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

public record WebGetContextResponse(IEnumerable<WebContextSlice> Data);
