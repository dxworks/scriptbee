using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;

namespace ScriptBee.Rest;

public class LoadInstanceContextAdapter(IHttpClientFactory httpClientFactory) : ILoadInstanceContext
{
    public Task Load(
        InstanceInfo instanceInfo,
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
