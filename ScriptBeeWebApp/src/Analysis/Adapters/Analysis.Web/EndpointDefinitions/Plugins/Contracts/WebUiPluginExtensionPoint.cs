namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public class WebUiPluginExtensionPoint
{
    public string Kind { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string Version { get; set; } = "";

    public string RemoteName { get; set; } = "";
    public string RemoteEntry { get; set; } = "";
    public IEnumerable<WebUiPluginOutlet> Outlets { get; set; } = [];
}

public class WebUiPluginOutlet
{
    public string Type { get; set; } = "";
    public string ExposedModule { get; set; } = "";
    public string? Path { get; set; }
    public string? Label { get; set; }
    public bool? Nested { get; set; }
    public string? ComponentName { get; set; }
    public string? Icon { get; set; }
    public List<string>? SupportedFileExtensions { get; set; }
}
