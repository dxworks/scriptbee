using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest;

public class GenerateInstanceClassesAdapter(IHttpClientFactory httpClientFactory)
    : IGenerateInstanceClasses
{
    public async Task<Stream> Generate(
        InstanceInfo instanceInfo,
        List<string> languages,
        string? transferFormat,
        CancellationToken cancellationToken
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var contextApi = RestService.For<IContextApi>(client);

        return await contextApi.GenerateClasses(
            new RestGenerateClasses(languages, transferFormat),
            cancellationToken
        );
    }
}
