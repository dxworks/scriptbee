namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebCreateScriptCommand(
    string Path,
    string Language,
    IEnumerable<WebScriptParameter>? Parameters
);
