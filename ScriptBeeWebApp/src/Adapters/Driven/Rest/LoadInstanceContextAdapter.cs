using Refit;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api;
using ScriptBee.Rest.Contracts;

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

        var contextApi = RestService.For<IContextApi>(client);

        await contextApi.Load(
            new RestContextLoad { FilesToLoad = ConvertFilesToLoad(filesToLoad) },
            cancellationToken
        );
    }

    private static Dictionary<string, List<string>> ConvertFilesToLoad(
        IDictionary<string, IEnumerable<FileId>> filesToLoad
    )
    {
        return filesToLoad.ToDictionary(
            x => x.Key,
            y => y.Value.Select(f => f.ToString()).ToList()
        );
    }
}
