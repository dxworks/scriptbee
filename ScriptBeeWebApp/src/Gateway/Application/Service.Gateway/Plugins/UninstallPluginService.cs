using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Plugins;

using UninstallResult = OneOf<ProjectDetails, ProjectDoesNotExistsError>;

public class UninstallPluginService(
    IUninstallPlugin uninstallPlugin,
    IGetProject getProject,
    IGetAllProjectInstances getAllProjectInstances,
    IUpdateProject updateProject
) : IUninstallPluginUseCase
{
    public async Task<UninstallResult> UninstallPluginAsync(
        UninstallPluginCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<UninstallResult>>(
            async details =>
                await UninstallPluginAsync(details, command.PluginId, cancellationToken),
            error => Task.FromResult<UninstallResult>(error)
        );
    }

    private async Task<UninstallResult> UninstallPluginAsync(
        ProjectDetails projectDetails,
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        if (
            !projectDetails.InstalledPlugins.Any(plugin =>
                plugin.PluginId == pluginId.Name && plugin.Version == pluginId.Version
            )
        )
        {
            return projectDetails;
        }

        await UninstallPluginFromAllInstances(projectDetails.Id, pluginId, cancellationToken);

        return await UpdateProjectDetailsWithPlugin(projectDetails, pluginId, cancellationToken);
    }

    private async Task<UninstallResult> UpdateProjectDetailsWithPlugin(
        ProjectDetails projectDetails,
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        projectDetails = projectDetails with
        {
            InstalledPlugins = projectDetails.InstalledPlugins.Where(config =>
                config.PluginId != pluginId.Name || config.Version != pluginId.Version
            ),
        };

        return await updateProject.Update(projectDetails, cancellationToken);
    }

    private async Task UninstallPluginFromAllInstances(
        ProjectId projectId,
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        var instances = await getAllProjectInstances.GetAll(projectId, cancellationToken);

        foreach (var instanceInfo in instances)
        {
            await uninstallPlugin.Uninstall(instanceInfo, pluginId, cancellationToken);
        }
    }
}
