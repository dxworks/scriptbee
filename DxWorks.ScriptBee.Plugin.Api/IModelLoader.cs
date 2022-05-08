using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IModelLoader
{
    public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(List<Stream> fileStreams,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default);

    public string GetName();
}