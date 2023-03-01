namespace DxWorks.ScriptBee.Plugin.Api;

public interface IModelLinker : IPlugin
{
    public Task LinkModel(Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default);

    public string GetName();
}
