using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Rest.Contracts;

public class RestPluginExtensionPoint
{
    public string Kind { get; set; } = "";
    public string EntryPoint { get; set; } = "";
    public string Version { get; set; } = "";

    public string? Language { get; set; }
    public string? Extension { get; set; }

    public int? Port { get; set; }
    public string? RemoteEntry { get; set; }
    public string? ExposedModule { get; set; }
    public string? ComponentName { get; set; }
    public string? UiPluginType { get; set; }

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
                Port = Port!.Value,
                ComponentName = ComponentName!,
                ExposedModule = ExposedModule!,
                RemoteEntry = RemoteEntry!,
                UiPluginType = UiPluginType!,
            },
            _ => null,
        };
    }
}
