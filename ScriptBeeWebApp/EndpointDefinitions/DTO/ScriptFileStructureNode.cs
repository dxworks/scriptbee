namespace ScriptBeeWebApp.EndpointDefinitions.DTO;

public class ScriptFileStructureNode
{
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string AbsolutePath { get; set; } = null!;
    public bool IsDirectory { get; set; }
    public int Level { get; set; }
    public ScriptDataResponse? ScriptData { get; set; }
    public IEnumerable<ScriptFileStructureNode>? Children { get; set; }
}
