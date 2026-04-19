namespace ScriptBee.Domain.Model.Plugins.Manifest;

public class UiPluginExtensionPoint : PluginExtensionPoint
{
    public int Port { get; set; }
    public string RemoteEntry { get; set; } = "";
    public string ExposedModule { get; set; } = "";
    public string ComponentName { get; set; } = "";
    public string UiPluginType { get; set; } = "";
}
