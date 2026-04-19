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
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<UninstallResult>>(
            async details => await UninstallPluginAsync(details, command, cancellationToken),
            error => Task.FromResult<UninstallResult>(error)
        );
    }

    private async Task<UninstallResult> UninstallPluginAsync(
        ProjectDetails projectDetails,
        UninstallPluginCommand command,
        CancellationToken cancellationToken
    )
    {
        if (
            !projectDetails.InstalledPlugins.Any(plugin =>
                plugin.PluginId == command.PluginId.Name
                && plugin.Version == command.PluginId.Version
            )
        )
        {
            return projectDetails;
        }

        await UninstallPluginToAllInstances(command.ProjectId, command.PluginId, cancellationToken);

        return await UpdateProjectDetailsWithPlugin(projectDetails, command, cancellationToken);
    }

    private async Task<UninstallResult> UpdateProjectDetailsWithPlugin(
        ProjectDetails projectDetails,
        UninstallPluginCommand command,
        CancellationToken cancellationToken
    )
    {
        projectDetails = projectDetails with
        {
            InstalledPlugins = projectDetails.InstalledPlugins.Where(config =>
                config.PluginId != command.PluginId.Name
                || config.Version != command.PluginId.Version
            ),
        };

        return await updateProject.Update(projectDetails, cancellationToken);
    }

    private async Task UninstallPluginToAllInstances(
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
