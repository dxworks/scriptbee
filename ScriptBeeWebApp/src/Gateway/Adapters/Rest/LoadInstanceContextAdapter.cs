using Refit;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using ScriptBee.Rest.Api.Generated.Contracts;

namespace ScriptBee.Rest;

public class LoadInstanceContextAdapter(IHttpClientFactory httpClientFactory) : ILoadInstanceContext
{
    public async Task Load(
        InstanceInfo instanceInfo,
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var analysisApi = RestService.For<IAnalysisApi>(client);

        await analysisApi.Load(
            new LoadContextCommand(ConvertFilesToLoad(filesToLoad)),
            cancellationToken
        );
    }

    private static Dictionary<string, ICollection<string>> ConvertFilesToLoad(
        IDictionary<string, IEnumerable<FileId>> filesToLoad
    )
    {
        return filesToLoad.ToDictionary(
            x => x.Key,
            y => (ICollection<string>)y.Value.Select(f => f.ToString()).ToList()
        );
    }
}
