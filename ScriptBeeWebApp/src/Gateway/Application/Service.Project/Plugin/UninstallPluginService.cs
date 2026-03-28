using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Plugin;

namespace ScriptBee.Service.Project.Plugin;

using UninstallResult = OneOf<ProjectDetails, ProjectDoesNotExistsError>;

public class UninstallPluginService(
    IUninstallPlugin installPlugin,
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
                plugin.PluginId == command.PluginId && plugin.Version == command.PluginVersion
            )
        )
        {
            return projectDetails;
        }

        await UninstallPluginToAllInstances(
            command.ProjectId,
            command.PluginId,
            command.PluginVersion,
            cancellationToken
        );

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
                config.PluginId != command.PluginId || config.Version != command.PluginVersion
            ),
        };

        return await updateProject.Update(projectDetails, cancellationToken);
    }

    private async Task UninstallPluginToAllInstances(
        ProjectId projectId,
        string pluginId,
        string pluginVersion,
        CancellationToken cancellationToken
    )
    {
        var instances = await getAllProjectInstances.GetAll(projectId, cancellationToken);

        foreach (var instanceInfo in instances)
        {
            await installPlugin.Uninstall(instanceInfo, pluginId, pluginVersion, cancellationToken);
        }
    }
}
