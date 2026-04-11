using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;

namespace ScriptBee.Rest;

public class GenerateInstanceClassesAdapter(IHttpClientFactory httpClientFactory)
    : IGenerateInstanceClasses
{
    public async Task Generate(InstanceInfo instanceInfo, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var contextApi = RestService.For<IContextApi>(client);

        await contextApi.GenerateClasses(cancellationToken);
    }
}
