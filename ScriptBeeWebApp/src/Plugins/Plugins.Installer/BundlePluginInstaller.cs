using Microsoft.Extensions.Logging;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins.Marketplace;

namespace ScriptBee.Plugins.Installer;

public class BundlePluginInstaller(
    IPluginReader pluginReader,
    ISimplePluginInstaller simplePluginInstaller,
    IPluginUrlFetcher pluginUrlFetcher,
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
                async pluginResult =>
                    await InstallPluginBundle(pluginId, pluginResult, cancellationToken),
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

    private async Task<OneOf<List<PluginId>, PluginInstallationError>> InstallPluginBundle(
        PluginId pluginId,
        string? bundleFolder,
        CancellationToken cancellationToken
    )
    {
        if (!PluginId.TryParse(bundleFolder, out var bundleId))
        {
            return new List<PluginId>();
        }

        var installedPluginIds = new List<PluginId> { bundleId };

        var tasks = BundleExtensionPointUtils
            .GetPluginExtensionPointsIds(pluginReader, bundleFolder!)
            .Select(id => Install(id, cancellationToken))
            .ToList();

        var installResults = await Task.WhenAll(tasks);
        var pluginIdsThatCouldNotBeInstalled = new List<PluginId>();

        foreach (var installResult in installResults)
        {
            installResult.Switch(
                installedPluginIds.AddRange,
                error => pluginIdsThatCouldNotBeInstalled.Add(error.Id)
            );
        }

        if (pluginIdsThatCouldNotBeInstalled.Count <= 0)
        {
            return installedPluginIds;
        }

        var errorMessage = pluginIdsThatCouldNotBeInstalled
            .Select(id => $"Plugin {id.Name} version {id.Version} could not be installed.")
            .Aggregate(string.Empty, (s, s1) => s + s1);

        logger.LogError("{Message}", errorMessage);

        return new PluginInstallationError(pluginId, pluginIdsThatCouldNotBeInstalled);
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
