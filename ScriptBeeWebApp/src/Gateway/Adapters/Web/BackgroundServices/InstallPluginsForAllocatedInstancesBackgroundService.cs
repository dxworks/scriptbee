using System.Threading.Channels;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Service.Project.Analysis;

namespace ScriptBee.Web.BackgroundServices;

public class InstallPluginsForAllocatedInstancesBackgroundService(
    Channel<InstanceAllocatedEvent> eventChannel,
    InstallPluginsForNewlyAllocatedInstance service
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await eventChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            var request = await eventChannel.Reader.ReadAsync(stoppingToken);
            await service.InstallPlugins(
                request.projectDetails,
                request.instanceInfo,
                stoppingToken
            );
        }
    }
}
