using Refit;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;

namespace ScriptBee.Rest;

public class GetInstanceContextAdapter(IHttpClientFactory httpClientFactory) : IGetInstanceContext
{
    public async Task<IEnumerable<ContextSlice>> Get(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var contextApi = RestService.For<IContextApi>(client);

        var restContextSlices = await contextApi.Get(cancellationToken);
        return restContextSlices.Select(s => s.Map());
    }
}
