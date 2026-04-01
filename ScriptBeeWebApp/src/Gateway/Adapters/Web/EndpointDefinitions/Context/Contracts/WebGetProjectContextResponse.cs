namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebGetProjectContextResponse(IEnumerable<WebProjectContextSlice> Data);
