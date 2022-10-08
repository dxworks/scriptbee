using System.Collections.Generic;

namespace ScriptBee.Plugin.Manifest;

public class PluginManifest
{
    public string ApiVersion { get; set; } = ""; // todo use it
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Author { get; set; }
    public List<PluginExtensionPoint> ExtensionPoints { get; set; } = new();
}
