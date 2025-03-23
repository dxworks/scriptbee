using ScriptBee.Domain.Model.File;
using ScriptBee.Ports.Plugins;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class LoadContextService(IProjectManager projectManager, IPluginRepository pluginRepository)
    : ILoadContextUseCase
{
    public Task Load(
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
