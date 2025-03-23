using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Plugins;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

public class GetInstanceLoadersService(
    IGetPlugins getPlugins,
    IGetProjectInstance getProjectInstance
) : IGetInstanceLoadersUseCase
{
    public async Task<IEnumerable<Loader>> Get(
        GetLoadersQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(query.InstanceId, cancellationToken);

        return await result.Match(
            async instanceInfo => await GetLoadersFromPlugins(instanceInfo, cancellationToken),
            _ => Task.FromResult(Enumerable.Empty<Loader>())
        );
    }

    private async Task<IEnumerable<Loader>> GetLoadersFromPlugins(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        var loadedPlugins = await getPlugins.GetLoadedPlugins(instanceInfo, cancellationToken);

        return from plugin in loadedPlugins
            let loaderExtensionPoints = plugin.Manifest.ExtensionPoints.Where(point =>
                point is LoaderPluginExtensionPoint
            )
            where loaderExtensionPoints.Any()
            select new Loader(plugin.Id, plugin.Manifest.Name);
    }
}
