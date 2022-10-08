namespace ScriptBee.Plugin.Manifest;

public class ScriptGeneratorPluginExtensionPoint : PluginExtensionPoint
{
    public string Language { get; set; } = "";
    public string Extension { get; set; } = "";
}
