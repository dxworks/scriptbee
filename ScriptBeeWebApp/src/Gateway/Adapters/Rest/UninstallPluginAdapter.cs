using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;

namespace ScriptBee.Rest;

public class UninstallPluginAdapter(IHttpClientFactory httpClientFactory) : IUninstallPlugin
{
    public async Task Uninstall(
        InstanceInfo instanceInfo,
        PluginId pluginId,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var pluginsApi = RestService.For<IPluginsApi>(client);

        await pluginsApi.UninstallPlugin(
            pluginId.Name,
            pluginId.Version.ToString(),
            cancellationToken
        );
    }
}
