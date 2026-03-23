using Refit;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest.Api;

public interface IPluginsApi
{
    [Get("/api/plugins")]
    Task<List<RestInstalledPlugin>> GetInstalledPlugins(CancellationToken cancellationToken);
}
