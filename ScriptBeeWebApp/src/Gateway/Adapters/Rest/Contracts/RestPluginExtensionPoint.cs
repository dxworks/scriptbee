using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Rest.Contracts;

public class RestPluginExtensionPoint
{
    public string Kind { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string Version { get; set; } = "";

    public string? Language { get; set; }
    public string? Extension { get; set; }

    public string? RemoteEntry { get; set; }
    public string? RemoteName { get; set; }
    public IEnumerable<RestUiPluginExtensionPointOutlet>? Outlets { get; set; }

    public PluginExtensionPoint? Map()
    {
        return Kind switch
        {
            PluginKind.Loader => new LoaderPluginExtensionPoint
            {
                Kind = Kind,
                EntryPoint = EntryPoint,
                Version = Version,
            },
            PluginKind.Linker => new LinkerPluginExtensionPoint
            {
                Kind = Kind,
                EntryPoint = EntryPoint,
                Version = Version,
            },
            PluginKind.HelperFunctions => new HelperFunctionsPluginExtensionPoint
            {
                Kind = Kind,
                EntryPoint = EntryPoint,
                Version = Version,
            },
            PluginKind.ScriptGenerator => new ScriptGeneratorPluginExtensionPoint
            {
                Kind = Kind,
                EntryPoint = EntryPoint,
                Version = Version,
                Language = Language!,
                Extension = Extension!,
            },
            PluginKind.ScriptRunner => new ScriptRunnerPluginExtensionPoint
            {
                Kind = Kind,
                EntryPoint = EntryPoint,
                Version = Version,
                Language = Language!,
                Extension = Extension!,
            },
            PluginKind.Ui => new UiPluginExtensionPoint
            {
                Kind = Kind,
                EntryPoint = EntryPoint,
                Version = Version,
                RemoteName = RemoteName!,
                RemoteEntry = RemoteEntry!,
                Outlets = (Outlets ?? []).Select(MapOutlet),
            },
            _ => null,
        };
    }

    private static UiPluginExtensionPointOutlet MapOutlet(RestUiPluginExtensionPointOutlet outlet)
    {
        return outlet.Type switch
        {
            "top-navigation-bar" => new TopNavigationBarOutlet(
                outlet.Type,
                outlet.ExposedModule,
                outlet.Path ?? "",
                outlet.Label ?? "",
                outlet.Nested,
                outlet.ComponentName
            ),
            "side-panel" => new SidePanelOutlet(
                outlet.Type,
                outlet.ExposedModule,
                outlet.Path ?? "",
                outlet.Label ?? "",
                outlet.Nested,
                outlet.ComponentName,
                outlet.Icon ?? ""
            ),
            "file-previewer" => new FilePreviewerOutlet(
                outlet.Type,
                outlet.ExposedModule,
                outlet.Label ?? "",
                outlet.ComponentName,
                outlet.Icon,
                outlet.SupportedFileExtensions
            ),
            _ => new UiPluginExtensionPointOutlet(outlet.Type),
        };
    }
}

public class RestUiPluginExtensionPointOutlet
{
    public required string Type { get; set; }
    public required string ExposedModule { get; set; }
    public string? Path { get; set; }
    public string? Label { get; set; }
    public bool? Nested { get; set; }
    public string? ComponentName { get; set; }
    public string? Icon { get; set; }
    public List<string>? SupportedFileExtensions { get; set; }
}
