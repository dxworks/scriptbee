using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using ScriptBee.Rest.Api.Generated.Contracts;

namespace ScriptBee.Rest;

public class InstallPluginAdapter(IHttpClientFactory httpClientFactory) : IInstallPlugin
{
    public async Task Install(
        InstanceInfo instanceInfo,
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var analysisApi = RestService.For<IAnalysisApi>(client);

        var request = new InstallPluginCommand(pluginId.Name, pluginId.Version.ToString());
        await analysisApi.PluginsPost(request, cancellationToken);
    }
}
