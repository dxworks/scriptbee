using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Plugins;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

public class GetInstanceLinkersService(
    IGetPlugins getPlugins,
    IGetProjectInstance getProjectInstance
) : IGetInstanceLinkersUseCase
{
    public async Task<IEnumerable<Linker>> Get(
        GetLinkersQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(query.InstanceId, cancellationToken);

        return await result.Match(
            async instanceInfo => await GetLinkersFromPlugins(instanceInfo, cancellationToken),
            _ => Task.FromResult(Enumerable.Empty<Linker>())
        );
    }

    private async Task<IEnumerable<Linker>> GetLinkersFromPlugins(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        var loadedPlugins = await getPlugins.GetLoadedPlugins(instanceInfo, cancellationToken);

        return from plugin in loadedPlugins
            let linkerExtensionPoints = plugin.Manifest.ExtensionPoints.Where(point =>
                point is LinkerPluginExtensionPoint
            )
            where linkerExtensionPoints.Any()
            select new Linker(plugin.Id, plugin.Manifest.Name);
    }
}
