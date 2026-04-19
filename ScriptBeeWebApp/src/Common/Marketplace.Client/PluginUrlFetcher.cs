using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Marketplace.Client.Errors;

namespace ScriptBee.Marketplace.Client;

public class PluginUrlFetcher(IMarketPluginFetcher marketPluginFetcher) : IPluginUrlFetcher
{
    public async Task<OneOf<string, PluginNotFoundError, PluginVersionNotFoundError>> GetPluginUrl(
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        var plugins = await marketPluginFetcher.GetProjectsAsync(cancellationToken);

        var plugin = plugins.FirstOrDefault(p => p.Id == pluginId.Name);
        if (plugin is null)
        {
            return new PluginNotFoundError(pluginId);
        }

        var pluginVersion = plugin.Versions.FirstOrDefault(v => v.Version == pluginId.Version);
        if (pluginVersion is null)
        {
            return new PluginVersionNotFoundError(pluginId);
        }

        return pluginVersion.Url;
    }
}
