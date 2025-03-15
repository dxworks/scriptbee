namespace ScriptBee.Domain.Model.Plugin.Manifest;

public class ScriptRunnerPluginExtensionPoint : PluginExtensionPoint
{
    public string Language { get; set; } = "";
    public string Extension { get; set; } = "";
}
