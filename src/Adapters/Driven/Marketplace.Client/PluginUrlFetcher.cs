using ScriptBee.Marketplace.Client.Exceptions;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Marketplace.Client;

public class PluginUrlFetcher(IMarketPluginFetcher marketPluginFetcher) : IPluginUrlFetcher
{
    public string GetPluginUrl(string pluginId, string version)
    {
        var plugins = marketPluginFetcher.GetProjectsAsync();

        var plugin = plugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin is null)
        {
            // TODO FIXIT(#51): convert to error instead of exception
            throw new PluginNotFoundException(pluginId);
        }

        var semver = Version.Parse(version);
        var pluginVersion = plugin.Versions.FirstOrDefault(v => v.Version == semver);

        if (pluginVersion is null)
        {
            // TODO FIXIT(#51): convert to error instead of exception
            throw new PluginVersionNotFoundException($"{pluginId} {version}");
        }

        return pluginVersion.Url;
    }
}
