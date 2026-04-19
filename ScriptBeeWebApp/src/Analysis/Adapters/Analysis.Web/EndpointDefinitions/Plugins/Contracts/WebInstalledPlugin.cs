using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

public record WebInstalledPlugin(
    string FolderPath,
    string Id,
    string Version,
    WebInstalledPluginManifest Manifest
)
{
    public static WebInstalledPlugin Map(Plugin plugin)
    {
        return new WebInstalledPlugin(
            plugin.FolderPath,
            plugin.Id.Name,
            plugin.Id.Version.ToString(),
            WebInstalledPluginManifest.Map(plugin.Manifest)
        );
    }
}
