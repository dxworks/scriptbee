namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public class WebProjectStructureNode
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string AbsolutePath { get; set; }
    public IEnumerable<WebProjectStructureNode>? Children { get; set; }
}
