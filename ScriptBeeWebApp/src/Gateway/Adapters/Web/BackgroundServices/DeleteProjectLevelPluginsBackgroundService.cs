using System.Threading.Channels;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Service.Gateway.Analysis;

namespace ScriptBee.Web.BackgroundServices;

public class DeleteProjectLevelPluginsBackgroundService(
    Channel<InstanceDeallocatedEvent> eventChannel,
    DeleteProjectLevelPluginsService service,
    ILogger<DeleteProjectLevelPluginsBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Delete Project Plugins Background Service is starting");

        while (await eventChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            InstanceDeallocatedEvent? request = null;
            try
            {
                request = await eventChannel.Reader.ReadAsync(stoppingToken);
                logger.LogDebug(
                    "Received instance-deallocated event for project {ProjectId}",
                    request.projectDetails.Id
                );
                await service.DeleteProjectPlugins(request.projectDetails, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "An error occurred while trying to delete project plugins for {Request}",
                    request
                );
            }
        }

        logger.LogInformation("Delete Project Plugins Background Service has stopped");
    }
}
