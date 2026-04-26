using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Plugins.Marketplace;

namespace ScriptBee.Plugins.Installer;

using InstallDependencyResult = OneOf<string, PluginManifestNotFoundError, PluginInstallationError>;
using InstallFromStreamResult = OneOf<
    List<PluginId>,
    PluginManifestNotFoundError,
    PluginInstallationError
>;

public class BundlePluginInstaller(
    IPluginReader pluginReader,
    ISimplePluginInstaller simplePluginInstaller,
    IPluginUrlFetcher pluginUrlFetcher,
    IPluginPathProvider pluginPathProvider,
    IPluginZipProcessor pluginZipProcessor,
    ILogger<BundlePluginInstaller> logger
) : IBundlePluginInstaller
{
    public async Task<OneOf<List<PluginId>, PluginInstallationError>> Install(
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await InstallPlugin(pluginId, cancellationToken);

            return await result.Match<Task<OneOf<List<PluginId>, PluginInstallationError>>>(
                async pluginResultPath =>
                    await InstallPluginBundle(pluginId, pluginResultPath, cancellationToken),
                error => Task.FromResult<OneOf<List<PluginId>, PluginInstallationError>>(error)
            );
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Error installing plugin {Name} with {Version}",
                pluginId.Name,
                pluginId.Version
            );
            return new PluginInstallationError(pluginId, []);
        }
    }

    public async Task<InstallFromStreamResult> Install(
        ProjectId projectId,
        Stream zipStream,
        CancellationToken cancellationToken
    )
    {
        var result = await pluginZipProcessor.ProcessZipStream(
            projectId,
            zipStream,
            cancellationToken
        );

        return await result.Match<Task<InstallFromStreamResult>>(
            async pluginId =>
            {
                var pluginPath = Path.Combine(
                    pluginPathProvider.GetPathToPlugins(projectId),
                    pluginId.GetFullyQualifiedName()
                );
                return await InstallProjectPluginBundle(
                    projectId,
                    pluginId,
                    pluginPath,
                    cancellationToken
                );
            },
            error => Task.FromResult<InstallFromStreamResult>(error),
            error => Task.FromResult<InstallFromStreamResult>(error)
        );
    }

    private async Task<InstallFromStreamResult> InstallProjectPluginBundle(
        ProjectId projectId,
        PluginId pluginId,
        string bundleFolder,
        CancellationToken cancellationToken
    )
    {
        var installedPluginIds = new List<PluginId> { pluginId };
        var pluginIdsThatCouldNotBeInstalled = new List<PluginId>();

        var ids = BundleExtensionPointUtils.GetPluginExtensionPointsIds(pluginReader, bundleFolder);

        foreach (var id in ids)
        {
            var installResult = await InstallProjectDependency(projectId, id, cancellationToken);

            await installResult.Match<Task>(
                async pluginPath =>
                {
                    var subBundleResult = await InstallProjectPluginBundle(
                        projectId,
                        id,
                        pluginPath,
                        cancellationToken
                    );

                    subBundleResult.Switch(
                        installedPluginIds.AddRange,
                        _ => pluginIdsThatCouldNotBeInstalled.Add(id),
                        _ => pluginIdsThatCouldNotBeInstalled.Add(id)
                    );
                },
                _ =>
                {
                    pluginIdsThatCouldNotBeInstalled.Add(id);
                    return Task.CompletedTask;
                },
                _ =>
                {
                    pluginIdsThatCouldNotBeInstalled.Add(id);
                    return Task.CompletedTask;
                }
            );
        }

        if (pluginIdsThatCouldNotBeInstalled.Count != 0)
        {
            var errorMessage = pluginIdsThatCouldNotBeInstalled
                .Select(id => $"Plugin {id.Name} version {id.Version} could not be installed.")
                .Aggregate(string.Empty, (s, s1) => s + s1);

            logger.LogError("{Message}", errorMessage);

            return new PluginInstallationError(pluginId, pluginIdsThatCouldNotBeInstalled);
        }

        return installedPluginIds;
    }

    private async Task<InstallDependencyResult> InstallProjectDependency(
        ProjectId projectId,
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        var projectPath = Path.Combine(
            pluginPathProvider.GetPathToPlugins(projectId),
            pluginId.GetFullyQualifiedName()
        );
        if (Directory.Exists(projectPath))
        {
            return projectPath;
        }

        var globalPath = Path.Combine(
            pluginPathProvider.GetPathToPlugins(),
            pluginId.GetFullyQualifiedName()
        );
        if (Directory.Exists(globalPath))
        {
            return globalPath;
        }

        var result = await pluginUrlFetcher.GetPluginUrl(pluginId, cancellationToken);

        return await result.Match<Task<InstallDependencyResult>>(
            async url =>
            {
                var installResult = await simplePluginInstaller.Install(
                    url,
                    pluginId,
                    cancellationToken
                );
                return installResult.Match<InstallDependencyResult>(path => path, error => error);
            },
            error =>
                Task.FromResult<InstallDependencyResult>(new PluginInstallationError(error.Id, [])),
            error =>
                Task.FromResult<InstallDependencyResult>(new PluginInstallationError(error.Id, []))
        );
    }

    private async Task<OneOf<List<PluginId>, PluginInstallationError>> InstallPluginBundle(
        PluginId pluginId,
        string? bundleFolder,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrEmpty(bundleFolder))
        {
            return new List<PluginId>();
        }

        var installedPluginIds = new List<PluginId> { pluginId };
        var pluginIdsThatCouldNotBeInstalled = new List<PluginId>();

        var ids = BundleExtensionPointUtils.GetPluginExtensionPointsIds(pluginReader, bundleFolder);

        foreach (var id in ids)
        {
            var installResult = await Install(id, cancellationToken);

            installResult.Switch(
                installedPluginIds.AddRange,
                _ => pluginIdsThatCouldNotBeInstalled.Add(id)
            );
        }

        if (pluginIdsThatCouldNotBeInstalled.Count != 0)
        {
            var errorMessage = pluginIdsThatCouldNotBeInstalled
                .Select(id => $"Plugin {id.Name} version {id.Version} could not be installed.")
                .Aggregate(string.Empty, (s, s1) => s + s1);

            logger.LogError("{Message}", errorMessage);

            return new PluginInstallationError(pluginId, pluginIdsThatCouldNotBeInstalled);
        }

        return installedPluginIds;
    }

    private async Task<OneOf<string, PluginInstallationError>> InstallPlugin(
        PluginId pluginId,
        CancellationToken cancellationToken
    )
    {
        var result = await pluginUrlFetcher.GetPluginUrl(pluginId, cancellationToken);

        return await result.Match<Task<OneOf<string, PluginInstallationError>>>(
            async url => await simplePluginInstaller.Install(url, pluginId, cancellationToken),
            error =>
                Task.FromResult<OneOf<string, PluginInstallationError>>(
                    new PluginInstallationError(error.Id, [])
                ),
            error =>
                Task.FromResult<OneOf<string, PluginInstallationError>>(
                    new PluginInstallationError(error.Id, [])
                )
        );
    }
}
