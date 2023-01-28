using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Tests.Plugin.Internals;

internal class TestLoaderPlugin : IModelLoader
{
    public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(List<Stream> fileStreams,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Dictionary<string, Dictionary<string, ScriptBeeModel>>());
    }

    public string GetName()
    {
        return "";
    }
}
