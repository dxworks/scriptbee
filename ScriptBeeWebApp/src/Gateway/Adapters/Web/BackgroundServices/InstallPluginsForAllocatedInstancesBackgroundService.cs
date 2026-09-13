using System.Threading.Channels;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Service.Gateway.Analysis;

namespace ScriptBee.Web.BackgroundServices;

public class InstallPluginsForAllocatedInstancesBackgroundService(
    Channel<InstanceAllocatedEvent> eventChannel,
    InstallPluginsForNewlyAllocatedInstance service,
    ILogger<InstallPluginsForAllocatedInstancesBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Install Plugins Background Service is starting");

        while (await eventChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            InstanceAllocatedEvent? request = null;
            try
            {
                request = await eventChannel.Reader.ReadAsync(stoppingToken);
                logger.LogDebug(
                    "Received instance-allocated event for project {ProjectId}, instance {InstanceId}",
                    request.projectDetails.Id,
                    request.instanceInfo.Id
                );
                await service.InstallPlugins(
                    request.projectDetails,
                    request.instanceInfo,
                    stoppingToken
                );
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "An error occurred while trying to install plugins for {Request}",
                    request
                );
            }
        }

        logger.LogInformation("Install Plugins Background Service has stopped");
    }
}
