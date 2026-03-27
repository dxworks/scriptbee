using OneOf;
using ScriptBee.Marketplace.Client.Errors;

namespace ScriptBee.Marketplace.Client;

public class PluginUrlFetcher(IMarketPluginFetcher marketPluginFetcher) : IPluginUrlFetcher
{
    public OneOf<string, PluginNotFoundError, PluginVersionNotFoundError> GetPluginUrl(
        string pluginId,
        string version
    )
    {
        var plugins = marketPluginFetcher.GetProjectsAsync();

        var plugin = plugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin is null)
        {
            return new PluginNotFoundError(pluginId);
        }

        var semver = Version.Parse(version);
        var pluginVersion = plugin.Versions.FirstOrDefault(v => v.Version == semver);

        if (pluginVersion is null)
        {
            return new PluginVersionNotFoundError(pluginId, version);
        }

        return pluginVersion.Url;
    }
}
