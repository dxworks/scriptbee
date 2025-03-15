namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public class WebPluginExtensionPoint
{
    public string Kind { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string Version { get; set; } = "";
}
