using System.Threading.Channels;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.Web.BackgroundServices;

namespace ScriptBee.Web.Extensions;

public static class InstallPluginsForAllocatedInstancesExtensions
{
    public static IServiceCollection AddInstallPluginsForAllocatedInstancesServices(
        this IServiceCollection services
    )
    {
        return services
            .AddHostedService<InstallPluginsForAllocatedInstancesBackgroundService>()
            .AddSingleton<Channel<InstanceAllocatedEvent>>(_ =>
                Channel.CreateUnbounded<InstanceAllocatedEvent>(
                    new UnboundedChannelOptions { SingleReader = true }
                )
            )
            .AddSingleton<InstallPluginsForNewlyAllocatedInstance>();
    }
}
