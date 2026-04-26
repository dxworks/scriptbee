using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Installer;
using ScriptBee.Ports.Instance;

namespace ScriptBee.Service.Gateway.Analysis;

public sealed class DeleteProjectLevelPluginsService(
    IGetAllProjectInstances getAllProjectInstances,
    IBundlePluginUninstaller pluginUninstaller,
    IPluginPathProvider pluginPathProvider,
    ILogger<DeleteProjectLevelPluginsService> logger
)
{
    public async Task DeleteProjectPlugins(
        ProjectDetails projectDetails,
        CancellationToken cancellationToken
    )
    {
        var instances = await getAllProjectInstances.GetAll(projectDetails.Id, cancellationToken);
        if (instances.Any())
        {
            logger.LogInformation(
                "Project {ProjectId} has active instances. Skipping plugin deletion",
                projectDetails.Id
            );
            return;
        }

        var pathToPlugins = pluginPathProvider.GetPathToPlugins(projectDetails.Id);
        if (!Directory.Exists(pathToPlugins))
        {
            return;
        }

        var installedPlugins = projectDetails
            .InstalledPlugins.Select(p => new PluginId(p.PluginId, p.Version))
            .ToHashSet();

        DeletePluginsThatAreNotReferencedInProject(projectDetails, pathToPlugins, installedPlugins);
    }

    private void DeletePluginsThatAreNotReferencedInProject(
        ProjectDetails projectDetails,
        string pathToPlugins,
        HashSet<PluginId> installedPlugins
    )
    {
        foreach (var directory in Directory.GetDirectories(pathToPlugins))
        {
            var folderName = Path.GetFileName(directory);
            if (!PluginId.TryParse(folderName, out var pluginId))
            {
                continue;
            }
            if (installedPlugins.Contains(pluginId))
            {
                continue;
            }

            pluginUninstaller.Uninstall(pluginId, pathToPlugins);
            logger.LogInformation(
                "Deleted plugin {PluginId} for project {ProjectId}",
                pluginId,
                projectDetails.Id
            );
        }
    }
}
