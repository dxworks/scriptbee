using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Tests.Common.Plugin;

internal class TestLoaderPlugin : IModelLoader
{
    public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(
        List<Stream> fileStreams,
        Dictionary<string, object>? configuration = null,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(new Dictionary<string, Dictionary<string, ScriptBeeModel>>());

    public string GetName() => "";
}
