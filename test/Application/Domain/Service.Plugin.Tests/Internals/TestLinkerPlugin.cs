using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Service.Plugin.Tests.Internals;

internal class TestLinkerPlugin : IModelLinker
{
    public Task LinkModel(
        Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context,
        Dictionary<string, object>? configuration = null,
        CancellationToken cancellationToken = default
    ) => Task.CompletedTask;

    public string GetName() => "";
}
