namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public class WebScriptData
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string AbsolutePath { get; set; }
    public required WebScriptLanguage ScriptLanguage { get; set; }
    public required IEnumerable<WebScriptParameter> Parameters { get; set; }
}
