namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebGetProjectContextResponse(string Model, IEnumerable<string> PluginIds);
