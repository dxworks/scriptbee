namespace ScriptBee.Plugin.Manifest;

public class PluginManifestMetadata
{
    public string Name { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string? Version { get; set; } 
    public string? Description { get; set; }
    public string? Author { get; set; }
}
