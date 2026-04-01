using Refit;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest.Api;

public interface IPluginsApi
{
    [Get("/api/plugins")]
    Task<RestGetInstalledPluginsResponse> GetInstalledPlugins(CancellationToken cancellationToken);

    [Post("/api/plugins")]
    Task InstallPlugin([Body] RestInstallPlugin request, CancellationToken cancellationToken);

    [Delete("/api/plugins/{pluginId}")]
    Task UninstallPlugin(
        [AliasAs("pluginId")] string pluginId,
        [Query] string version,
        CancellationToken cancellationToken
    );
}
