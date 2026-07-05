using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using ScriptBee.Rest.Api.Generated.Contracts;

namespace ScriptBee.Rest;

public class LinkInstanceContextAdapter(IHttpClientFactory httpClientFactory) : ILinkInstanceContext
{
    public async Task Link(
        InstanceInfo instanceInfo,
        IEnumerable<string> linkerIds,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var analysisApi = RestService.For<IAnalysisApi>(client);

        await analysisApi.Link(new LinkContextCommand(linkerIds.ToList()), cancellationToken);
    }
}
