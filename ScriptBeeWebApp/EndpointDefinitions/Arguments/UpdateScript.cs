namespace ScriptBeeWebApp.EndpointDefinitions.Arguments;

public record UpdateScript(string Id, string ProjectId, List<ScriptParameter> Parameters);
