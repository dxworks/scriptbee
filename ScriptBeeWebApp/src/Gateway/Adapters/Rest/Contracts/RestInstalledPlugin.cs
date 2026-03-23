using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Rest.Contracts;

public class RestInstalledPlugin
{
    public string FolderPath { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Version { get; set; } = null!;
    public RestInstalledPluginManifest Manifest { get; set; } = null!;

    public Plugin Map()
    {
        return new Plugin(FolderPath, Id, new Version(Version), Manifest.Map());
    }
}
