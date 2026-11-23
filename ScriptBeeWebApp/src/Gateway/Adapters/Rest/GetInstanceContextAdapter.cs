using Refit;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using GeneratedContracts = ScriptBee.Rest.Api.Generated.Contracts;

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

        var analysisApi = RestService.For<IAnalysisApi>(client);

        var response = await analysisApi.Context(cancellationToken);
        return response.Data.Select(MapContextSlice);
    }

    private static ContextSlice MapContextSlice(GeneratedContracts.ContextSlice slice)
    {
        return new ContextSlice(slice.Model, slice.PluginIds);
    }
}
