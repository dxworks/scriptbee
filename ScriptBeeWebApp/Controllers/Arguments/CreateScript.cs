namespace ScriptBeeWebApp.Controllers.Arguments;

public record CreateScript(string projectId, string filePath, string scriptType)
{
    public string filePath { get; set; } = filePath;
}