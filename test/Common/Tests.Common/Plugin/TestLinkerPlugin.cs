using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Tests.Common.Plugin;

internal class TestLinkerPlugin : IModelLinker
{
    public Task LinkModel(
        Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context,
        Dictionary<string, object>? configuration = null,
        CancellationToken cancellationToken = default
    ) => Task.CompletedTask;

    public string GetName() => "";
}
