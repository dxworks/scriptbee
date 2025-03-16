using Refit;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;
using ScriptBee.Rest.Contracts;

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

        var contextApi = RestService.For<IContextApi>(client);

        await contextApi.Link(new RestContextLink { LinkerIds = linkerIds }, cancellationToken);
    }
}
