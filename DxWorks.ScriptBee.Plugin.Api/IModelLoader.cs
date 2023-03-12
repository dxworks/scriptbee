using DxWorks.ScriptBee.Plugin.Api.Model;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IModelLoader : IPlugin
{
    [Obsolete("Use LoadModel(IEnumerable<NamedFileStream> fileStreams, Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default) instead")]
    public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(List<Stream> fileStreams,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default);

    public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(
        IEnumerable<NamedFileStream> fileStreams, Dictionary<string, object>? configuration = default,
        CancellationToken cancellationToken = default)
    {
        return LoadModel(fileStreams.Select(x => x.Stream).ToList(), configuration, cancellationToken);
    }

    public string GetName();
}
