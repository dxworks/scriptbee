namespace ScriptBeeWebApp.EndpointDefinitions.Arguments;

public record GenerateScriptRequest(
    string ProjectId,
    string ScriptType
);
