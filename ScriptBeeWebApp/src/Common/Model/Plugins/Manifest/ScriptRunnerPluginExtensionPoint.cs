namespace ScriptBee.Domain.Model.Plugins.Manifest;

public class ScriptRunnerPluginExtensionPoint : PluginExtensionPoint
{
    public string Language { get; set; } = "";
    public string Extension { get; set; } = "";
}
