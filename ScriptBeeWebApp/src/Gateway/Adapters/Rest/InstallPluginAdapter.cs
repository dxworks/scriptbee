using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest;

public class InstallPluginAdapter(IHttpClientFactory httpClientFactory) : IInstallPlugin
{
    public async Task Install(
        InstanceInfo instanceInfo,
        string pluginId,
        string pluginVersion,
        CancellationToken cancellationToken
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var pluginsApi = RestService.For<IPluginsApi>(client);

        var request = new RestInstallPlugin { PluginId = pluginId, Version = pluginVersion };
        await pluginsApi.InstallPlugin(request, cancellationToken);
    }
}
