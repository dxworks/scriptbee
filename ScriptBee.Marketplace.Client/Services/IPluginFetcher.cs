using ScriptBee.Marketplace.Client.Data;

namespace ScriptBee.Marketplace.Client.Services;

public interface IPluginFetcher
{
    // TODO: add pagination 
    Task<IEnumerable<Plugin>> GetPluginsAsync(CancellationToken cancellationToken = default);

    Task<Plugin> GetPluginAsync(string pluginId, CancellationToken cancellationToken = default);
    
    Task<string> GetDownloadUrlAsync(string url, CancellationToken cancellationToken = default);
}
