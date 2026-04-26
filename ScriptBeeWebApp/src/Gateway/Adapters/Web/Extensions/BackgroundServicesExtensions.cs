using System.Threading.Channels;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.Service.Gateway.Config;
using ScriptBee.Web.BackgroundServices;

namespace ScriptBee.Web.Extensions;

public static class BackgroundServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBackgroundServices(string instanceConfiguration)
        {
            return services
                .AddInstallPluginsForAllocatedInstancesServices(instanceConfiguration)
                .AddDeleteProjectLevelPluginsServices();
        }

        private IServiceCollection AddInstallPluginsForAllocatedInstancesServices(
            string instanceConfiguration
        )
        {
            services.AddOptions<ScriptBeeInstanceConfig>().BindConfiguration(instanceConfiguration);

            return services
                .AddHostedService<InstallPluginsForAllocatedInstancesBackgroundService>()
                .AddSingleton<Channel<InstanceAllocatedEvent>>(_ =>
                    Channel.CreateUnbounded<InstanceAllocatedEvent>(
                        new UnboundedChannelOptions { SingleReader = true }
                    )
                )
                .AddSingleton<InstallPluginsForNewlyAllocatedInstance>();
        }

        private IServiceCollection AddDeleteProjectLevelPluginsServices()
        {
            return services
                .AddHostedService<DeleteProjectLevelPluginsBackgroundService>()
                .AddSingleton<Channel<InstanceDeallocatedEvent>>(_ =>
                    Channel.CreateUnbounded<InstanceDeallocatedEvent>(
                        new UnboundedChannelOptions { SingleReader = true }
                    )
                )
                .AddSingleton<DeleteProjectLevelPluginsService>();
        }
    }
}
