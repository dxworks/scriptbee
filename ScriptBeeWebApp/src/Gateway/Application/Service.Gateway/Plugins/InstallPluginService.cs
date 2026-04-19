using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Plugins;

namespace ScriptBee.Service.Project.Plugins;

using InstallResult = OneOf<ProjectDetails, ProjectDoesNotExistsError>;

public class InstallPluginService(
    IInstallPlugin installPlugin,
    IGetProject getProject,
    IGetAllProjectInstances getAllProjectInstances,
    IUpdateProject updateProject
) : IInstallPluginUseCase
{
    public async Task<InstallResult> InstallPluginAsync(
        InstallPluginCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<InstallResult>>(
            async details => await InstallPluginAsync(details, command, cancellationToken),
            error => Task.FromResult<InstallResult>(error)
        );
    }

    private async Task<InstallResult> InstallPluginAsync(
        ProjectDetails projectDetails,
        InstallPluginCommand command,
        CancellationToken cancellationToken
    )
    {
        if (
            projectDetails.InstalledPlugins.Any(plugin =>
                plugin.PluginId == command.PluginId && plugin.Version == command.PluginVersion
            )
        )
        {
            return projectDetails;
        }

        await InstallPluginToAllInstances(
            command.ProjectId,
            command.PluginId,
            command.PluginVersion,
            cancellationToken
        );

        return await UpdateProjectDetailsWithPlugin(projectDetails, command, cancellationToken);
    }

    private async Task<InstallResult> UpdateProjectDetailsWithPlugin(
        ProjectDetails projectDetails,
        InstallPluginCommand command,
        CancellationToken cancellationToken
    )
    {
        projectDetails = projectDetails with
        {
            InstalledPlugins = projectDetails.InstalledPlugins.Append(
                new PluginInstallationConfig(command.PluginId, command.PluginVersion)
            ),
        };

        return await updateProject.Update(projectDetails, cancellationToken);
    }

    private async Task InstallPluginToAllInstances(
        ProjectId projectId,
        string pluginId,
        string pluginVersion,
        CancellationToken cancellationToken
    )
    {
        var instances = await getAllProjectInstances.GetAll(projectId, cancellationToken);

        foreach (var instanceInfo in instances)
        {
            await installPlugin.Install(instanceInfo, pluginId, pluginVersion, cancellationToken);
        }
    }
}
