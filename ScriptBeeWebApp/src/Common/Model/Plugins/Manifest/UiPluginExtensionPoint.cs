namespace ScriptBee.Domain.Model.Plugins.Manifest;

public class UiPluginExtensionPoint : PluginExtensionPoint
{
    public required string RemoteName { get; set; }
    public required string RemoteEntry { get; set; }
    public required IEnumerable<UiPluginExtensionPointOutlet> Outlets { get; set; }
}

public class UiPluginExtensionPointOutlet
{
    public string Type { get; set; } = "";
}

public class RoutingOutlet : UiPluginExtensionPointOutlet
{
    public string ExposedModule { get; set; } = "";
    public string Path { get; set; } = "";
    public string Label { get; set; } = "";
    public bool? Nested { get; set; }
    public string? ComponentName { get; set; }
}

public class TopNavigationBarOutlet : RoutingOutlet;

public class SidePanelOutlet : RoutingOutlet
{
    public string Icon { get; set; } = "";
}

public class FilePreviewerOutlet : UiPluginExtensionPointOutlet
{
    public FilePreviewerOutlet() { }

    public FilePreviewerOutlet(
        string type,
        string exposedModule,
        string label,
        string? componentName,
        string? icon,
        List<string>? supportedFileExtensions
    )
    {
        Type = type;
        ExposedModule = exposedModule;
        Label = label;
        ComponentName = componentName;
        Icon = icon;
        SupportedFileExtensions = supportedFileExtensions;
    }

    public string ExposedModule { get; set; } = "";
    public string Label { get; set; } = "";
    public string? ComponentName { get; set; }
    public string? Icon { get; set; }
    public List<string>? SupportedFileExtensions { get; set; }
}
