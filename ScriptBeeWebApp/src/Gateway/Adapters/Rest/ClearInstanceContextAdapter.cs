using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;

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

        var contextApi = RestService.For<IAnalysisApi>(client);

        await contextApi.Clear(cancellationToken);
    }
}
