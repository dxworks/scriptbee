namespace ScriptBeeWebApp.EndpointDefinitions.DTO;

public record ScriptDataResponse(
    string Id,
    string ProjectId,
    string Name,
    string FilePath,
    string AbsolutePath,
    string ScriptLanguage,
    IEnumerable<ScriptParameterResponse> Parameters
);

public record ScriptParameterResponse(string Name, string Type, string? Value);
