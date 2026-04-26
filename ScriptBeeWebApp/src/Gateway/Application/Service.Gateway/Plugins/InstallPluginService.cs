using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins.Installer;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Plugins;

namespace ScriptBee.Service.Gateway.Plugins;

using InstallPluginFromZipResult = OneOf<
    ProjectDetails,
    ProjectDoesNotExistsError,
    PluginManifestNotFoundError,
    PluginInstallationError
>;
using InstallResult = OneOf<ProjectDetails, ProjectDoesNotExistsError, PluginInstallationError>;

public class InstallPluginService(
    IBundlePluginInstaller bundlePluginInstaller,
    IInstallPlugin installPlugin,
    IGetProject getProject,
    IGetAllProjectInstances getAllProjectInstances,
    IUpdateProject updateProject,
    ILogger<InstallPluginService> logger
) : IInstallPluginUseCase
{
    public async Task<InstallResult> InstallPluginAsync(
        InstallPluginCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<InstallResult>>(
            async details => await InstallPluginAsync(details, command, cancellationToken),
            error => Task.FromResult<InstallResult>(error)
        );
    }

    public async Task<InstallPluginFromZipResult> InstallPluginAsync(
        ProjectId projectId,
        Stream zipStream,
        CancellationToken cancellationToken
    )
    {
        var projectResult = await getProject.GetById(projectId, cancellationToken);

        return await projectResult.Match<Task<InstallPluginFromZipResult>>(
            async projectDetails =>
                await InstallToAllInstances(
                    projectId,
                    zipStream,
                    projectDetails,
                    cancellationToken
                ),
            error => Task.FromResult<InstallPluginFromZipResult>(error)
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
                plugin.PluginId == command.PluginId.Name
                && plugin.Version == command.PluginId.Version
            )
        )
        {
            return projectDetails;
        }

        var result = await InstallPlugin(command.ProjectId, command.PluginId, cancellationToken);

        return await result.Match<Task<InstallResult>>(
            async pluginIds =>
                await UpdateProjectDetailsWithPlugin(projectDetails, pluginIds, cancellationToken),
            error => Task.FromResult<InstallResult>(error)
        );
    }

    private async Task<ProjectDetails> UpdateProjectDetailsWithPlugin(
        ProjectDetails projectDetails,
        List<PluginId> installedPluginIds,
        CancellationToken cancellationToken
    )
    {
        var newConfigs = installedPluginIds.Select(id => new PluginInstallationConfig(
            id.Name,
            id.Version
        ));
        projectDetails = projectDetails with
        {
            InstalledPlugins = projectDetails
                .InstalledPlugins.UnionBy(newConfigs, p => new { p.PluginId, p.Version })
                .ToList(),
        };

        return await updateProject.Update(projectDetails, cancellationToken);
    }

    private async Task<OneOf<List<PluginId>, PluginInstallationError>> InstallPlugin(
        ProjectId projectId,
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        var result = await bundlePluginInstaller.Install(pluginId, cancellationToken);

        return await result.Match<Task<OneOf<List<PluginId>, PluginInstallationError>>>(
            async plugins =>
            {
                await InstallPluginToAllInstances(projectId, plugins, cancellationToken);
                return plugins;
            },
            error => Task.FromResult<OneOf<List<PluginId>, PluginInstallationError>>(error)
        );
    }

    private async Task InstallPluginToAllInstances(
        ProjectId projectId,
        List<PluginId> pluginIds,
        CancellationToken cancellationToken
    )
    {
        var instances = await getAllProjectInstances.GetAll(projectId, cancellationToken);

        foreach (var instanceInfo in instances)
        {
            foreach (var pluginId in pluginIds)
            {
                try
                {
                    await installPlugin.Install(instanceInfo, pluginId, cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogWarning(
                        e,
                        "Could not install plugin to {InstanceId}",
                        instanceInfo.Id
                    );
                }
            }
        }
    }

    private async Task<InstallPluginFromZipResult> InstallToAllInstances(
        ProjectId projectId,
        Stream zipStream,
        ProjectDetails projectDetails,
        CancellationToken cancellationToken
    )
    {
        var installResult = await bundlePluginInstaller.Install(
            projectId,
            zipStream,
            cancellationToken
        );

        return await installResult.Match<Task<InstallPluginFromZipResult>>(
            async installedPluginIds =>
            {
                await InstallPluginToAllInstances(projectId, installedPluginIds, cancellationToken);
                return await UpdateProjectDetailsWithPlugin(
                    projectDetails,
                    installedPluginIds,
                    cancellationToken
                );
            },
            error => Task.FromResult<InstallPluginFromZipResult>(error),
            error => Task.FromResult<InstallPluginFromZipResult>(error)
        );
    }
}
