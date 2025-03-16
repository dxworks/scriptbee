using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Ports.Plugins;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class LinkContextService(IProjectManager projectManager, IPluginRepository pluginRepository)
    : ILinkContextUseCase
{
    public async Task Link(IEnumerable<string> linkerIds, CancellationToken cancellationToken)
    {
        var project = projectManager.GetProject();

        foreach (var linkerId in linkerIds)
        {
            var modelLinker = pluginRepository.GetPlugin<IModelLinker>(linker =>
                linker.GetName() == linkerId
            );

            if (modelLinker is not null)
            {
                await modelLinker.LinkModel(
                    project.Context.Models,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
