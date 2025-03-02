namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

// TODO FIXIT: update PluginIds to return more information (id+name)
public record WebGetProjectContextResponse(string Model, IEnumerable<string> PluginIds);
