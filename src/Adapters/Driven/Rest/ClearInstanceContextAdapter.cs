using Refit;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;

namespace ScriptBee.Rest;

public class ClearInstanceContextAdapter(IHttpClientFactory httpClientFactory)
    : IClearInstanceContext
{
    public async Task Clear(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var contextApi = RestService.For<IContextApi>(client);

        await contextApi.Clear(cancellationToken);
    }
}
