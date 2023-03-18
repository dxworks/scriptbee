using ScriptBee.Marketplace.Client.Services;
using ScriptBee.Plugin;
using ScriptBeeWebApp.Data.Exceptions;

namespace ScriptBeeWebApp.Services;

public class PluginUrlFetcher : IPluginUrlFetcher
{
    private readonly IMarketPluginFetcher _marketPluginFetcher;

    public PluginUrlFetcher(IMarketPluginFetcher marketPluginFetcher)
    {
        _marketPluginFetcher = marketPluginFetcher;
    }

    public string GetPluginUrl(string pluginId, string version)
    {
        var plugins = _marketPluginFetcher.GetProjectsAsync();

        var plugin = plugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin is null)
        {
            throw new PluginNotFoundException(pluginId);
        }

        var semver = Version.Parse(version);
        var pluginVersion = plugin.Versions.FirstOrDefault(v => v.Version == semver);

        if (pluginVersion is null)
        {
            throw new PluginVersionNotFoundException($"{pluginId} {version}");
        }

        return pluginVersion.Url;
    }
}
