using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.TestsCommon;

public class TestModelLoader : IModelLoader
{
    public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(List<Stream> fileStreams,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default)
    {
        return new Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>>(() =>
            new Dictionary<string, Dictionary<string, ScriptBeeModel>>());
    }

    public string GetName()
    {
        return "";
    }
}
