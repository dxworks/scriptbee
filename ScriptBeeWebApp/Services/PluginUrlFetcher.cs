using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task<string> GetPluginUrlAsync(string pluginId, string version,
        CancellationToken cancellationToken = default)
    {
        var plugins = await _marketPluginFetcher.GetProjectsAsync(cancellationToken);

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
