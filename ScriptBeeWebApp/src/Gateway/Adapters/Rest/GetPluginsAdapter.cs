using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;

namespace ScriptBee.Rest;

public class GetPluginsAdapter(IHttpClientFactory httpClientFactory) : IGetPlugins
{
    public async Task<IEnumerable<Plugin>> GetLoadedPlugins(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var pluginsApi = RestService.For<IPluginsApi>(client);

        var response = await pluginsApi.GetInstalledPlugins(cancellationToken);

        return response.Data.Select(plugin => plugin.Map());
    }
}
