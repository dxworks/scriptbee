namespace ScriptBeeWebApp.EndpointDefinitions.DTO;

public record CreateScriptResponse(
    string Id,
    string ProjectId,
    string Name,
    string FilePath,
    string SrcPath,
    string ScriptLanguage,
    IEnumerable<ScriptParameterResponse> Parameters
);

public record ScriptParameterResponse(string Name, string Type, string? Value);
