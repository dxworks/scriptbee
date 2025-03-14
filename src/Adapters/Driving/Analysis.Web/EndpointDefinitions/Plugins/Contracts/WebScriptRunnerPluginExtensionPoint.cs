namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public class WebScriptRunnerPluginExtensionPoint
{
    public string Kind { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string Version { get; set; } = "";

    public string Language { get; set; } = "";
}
