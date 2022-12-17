namespace ScriptBee.Plugin.Manifest;

public abstract class PluginExtensionPoint
{
    public string Kind { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string Version { get; set; } = "";
}
