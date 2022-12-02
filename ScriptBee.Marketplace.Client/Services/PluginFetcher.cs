using ScriptBee.Marketplace.Client.Data;
using ScriptBee.Marketplace.Client.Repository;

namespace ScriptBee.Marketplace.Client.Services;

public sealed class PluginFetcher : IPluginFetcher
{
    private const string PluginsCollection = "plugins";
    private readonly IDocumentRepository<Plugin> _pluginRepository;
    private readonly IStorageRepository _storageRepository;

    public PluginFetcher(IDocumentRepository<Plugin> pluginRepository, IStorageRepository storageRepository)
    {
        _pluginRepository = pluginRepository;
        _storageRepository = storageRepository;
    }

    public async Task<IEnumerable<Plugin>> GetPluginsAsync(CancellationToken cancellationToken = default)
    {
        return await _pluginRepository.GetAllDocumentsAsync(PluginsCollection, cancellationToken);
    }

    public async Task<Plugin> GetPluginAsync(string pluginId, CancellationToken cancellationToken = default)
    {
        return await _pluginRepository.GetDocumentAsync(PluginsCollection, pluginId, cancellationToken);
    }

    public async Task<string> GetDownloadUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _storageRepository.GetDownloadUrlAsync(url);
    }
}
