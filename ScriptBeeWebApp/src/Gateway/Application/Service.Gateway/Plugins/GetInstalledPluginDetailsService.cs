using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.MarketPlace;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Plugins;

public class GetInstalledPluginDetailsService(
    IGetProject getProject,
    IPluginPathProvider pluginPathProvider,
    IPluginReader pluginReader
) : IGetInstalledPluginDetailsUseCase
{
    public async Task<OneOf<MarketPlacePlugin, PluginNotFoundError>> Get(
        ProjectId projectId,
        string pluginId,
        CancellationToken cancellationToken
    )
    {
        var projectResult = await getProject.GetById(projectId, cancellationToken);

        return projectResult.Match<OneOf<MarketPlacePlugin, PluginNotFoundError>>(
            project => GetPlugin(projectId, pluginId, project),
            _ => new PluginNotFoundError(new PluginId(pluginId, new Version()))
        );
    }

    private OneOf<MarketPlacePlugin, PluginNotFoundError> GetPlugin(
        ProjectId projectId,
        string pluginId,
        ProjectDetails project
    )
    {
        var installedPlugin = project.InstalledPlugins.FirstOrDefault(p => p.PluginId == pluginId);
        if (
            installedPlugin is null
            || !PluginId.TryParse(
                $"{installedPlugin.PluginId}@{installedPlugin.Version}",
                out var typedPluginId
            )
        )
        {
            return new PluginNotFoundError(new PluginId(pluginId, new Version()));
        }

        var pluginsPath = pluginPathProvider.GetPathToPlugins(projectId);
        var folderName = typedPluginId.GetFullyQualifiedName();
        var pluginPath = Path.Combine(pluginsPath, folderName);

        var loadedPlugin = pluginReader.ReadPlugin(pluginPath);

        if (loadedPlugin is null)
        {
            return new PluginNotFoundError(typedPluginId);
        }

        return new MarketPlacePlugin(
            loadedPlugin.Id.Name,
            loadedPlugin.Manifest.Name,
            MarketPlacePluginType.Plugin,
            loadedPlugin.Manifest.Description ?? "",
            loadedPlugin.Manifest.Author is not null ? [loadedPlugin.Manifest.Author] : [],
            [new PluginVersion("", loadedPlugin.Id.Version, "")]
        );
    }
}
