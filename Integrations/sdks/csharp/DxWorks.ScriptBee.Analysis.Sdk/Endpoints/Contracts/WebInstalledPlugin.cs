using InstalledPlugin = ScriptBee.Domain.Model.Plugins.Plugin;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;

public record WebInstalledPlugin(
    string FolderPath,
    string Id,
    string Version,
    WebInstalledPluginManifest Manifest
)
{
    public static WebInstalledPlugin Map(InstalledPlugin plugin)
    {
        return new WebInstalledPlugin(
            plugin.FolderPath,
            plugin.Id.Name,
            plugin.Id.Version.ToString(),
            WebInstalledPluginManifest.Map(plugin.Manifest)
        );
    }
}
