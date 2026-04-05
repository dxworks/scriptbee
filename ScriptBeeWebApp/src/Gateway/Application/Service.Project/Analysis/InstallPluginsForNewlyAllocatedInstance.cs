using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Instance.Allocation;

namespace ScriptBee.Service.Project.Analysis;

public sealed class InstallPluginsForNewlyAllocatedInstance(
    IGetInstanceStatus getInstanceStatus,
    IInstallPlugin installPlugin,
    ILogger<InstallPluginsForNewlyAllocatedInstance> logger
)
{
    public async Task InstallPlugins(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        AnalysisInstanceStatus status;
        do
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            status = await getInstanceStatus.GetStatus(instanceInfo.Id, cancellationToken);
        } while (status == AnalysisInstanceStatus.Allocating);

        if (status != AnalysisInstanceStatus.Running)
        {
            logger.LogWarning(
                "Could not install plugins for instance {Instance} because instance is {Status}",
                instanceInfo,
                status
            );
            return;
        }

        await InstallPluginsForInstance(projectDetails, instanceInfo, cancellationToken);
    }

    private async Task InstallPluginsForInstance(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Installing plugins for project {Project}", projectDetails);
        foreach (var plugin in projectDetails.InstalledPlugins)
        {
            logger.LogInformation(
                "Installing plugin {Plugin} for project {Project}",
                plugin,
                projectDetails
            );

            await installPlugin.Install(
                instanceInfo,
                plugin.PluginId,
                plugin.Version,
                cancellationToken
            );

            logger.LogInformation(
                "Installed plugin {Plugin} for project {Project}",
                plugin,
                projectDetails
            );
        }
    }
}
