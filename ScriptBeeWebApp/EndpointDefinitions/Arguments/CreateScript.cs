namespace ScriptBeeWebApp.EndpointDefinitions.Arguments;

public record CreateScript(string ProjectId, string FilePath, string ScriptLanguage, List<ScriptParameter> Parameters)
{
    public string FilePath { get; set; } = FilePath;
}

public record ScriptParameter(string Name, string Type, string? Value);
