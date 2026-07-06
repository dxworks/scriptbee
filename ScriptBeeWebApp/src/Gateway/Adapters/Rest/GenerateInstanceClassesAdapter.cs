using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using ScriptBee.Rest.Api.Generated.Contracts;

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

        var analysisApi = RestService.For<IAnalysisApi>(client);

        return await analysisApi.GenerateClasses(
            new GenerateClassesRequest(languages, transferFormat ?? string.Empty),
            cancellationToken
        );
    }
}
