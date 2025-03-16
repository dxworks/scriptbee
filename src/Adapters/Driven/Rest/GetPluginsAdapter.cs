using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Ports.Plugins;
using ScriptBee.Rest.Api;

namespace ScriptBee.Rest;

public class GetPluginsAdapter(IHttpClientFactory httpClientFactory) : IGetPlugins
{
    public async Task<IEnumerable<Plugin>> GetLoadedPlugins(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var pluginsApi = RestService.For<IPluginsApi>(client);

        var restInstalledPlugins = await pluginsApi.GetInstalledPlugins(cancellationToken);

        return restInstalledPlugins.Select(plugin => plugin.Map());
    }
}
