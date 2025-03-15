namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public class WebUiPluginExtensionPoint
{
    public string Kind { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string Version { get; set; } = "";

    public int Port { get; set; }
    public string RemoteEntry { get; set; } = "";
    public string ExposedModule { get; set; } = "";
    public string ComponentName { get; set; } = "";
    public string UiPluginType { get; set; } = "";
}
