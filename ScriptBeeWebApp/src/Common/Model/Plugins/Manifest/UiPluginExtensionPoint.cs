namespace ScriptBee.Domain.Model.Plugins.Manifest;

public class UiPluginExtensionPoint : PluginExtensionPoint
{
    public required string RemoteName { get; set; }
    public required string RemoteEntry { get; set; }
    public required IEnumerable<UiPluginExtensionPointOutlet> Outlets { get; set; }
}

public record UiPluginExtensionPointOutlet(string Type);

public record RoutingOutlet(
    string Type,
    string ExposedModule,
    string Path,
    string Label,
    bool? Nested,
    string? ComponentName
) : UiPluginExtensionPointOutlet(Type);

public record TopNavigationBarOutlet(
    string Type,
    string ExposedModule,
    string Path,
    string Label,
    bool? Nested,
    string? ComponentName
) : RoutingOutlet(Type, ExposedModule, Path, Label, Nested, ComponentName);

public record SidePanelOutlet(
    string Type,
    string ExposedModule,
    string Path,
    string Label,
    bool? Nested,
    string? ComponentName,
    string Icon
) : RoutingOutlet(Type, ExposedModule, Path, Label, Nested, ComponentName);

public record FilePreviewerOutlet(
    string Type,
    string ExposedModule,
    string Label,
    string? ComponentName,
    string? Icon,
    List<string>? SupportedFileExtensions
) : UiPluginExtensionPointOutlet(Type);
