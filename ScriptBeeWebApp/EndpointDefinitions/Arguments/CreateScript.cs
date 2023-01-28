namespace ScriptBeeWebApp.EndpointDefinitions.Arguments;

public record CreateScript(string ProjectId, string FilePath, string ScriptType)
{
    public string FilePath { get; set; } = FilePath;
}
