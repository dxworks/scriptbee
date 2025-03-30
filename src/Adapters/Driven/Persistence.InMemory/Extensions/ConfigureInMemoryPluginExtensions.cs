using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Persistence.InMemory.Extensions;

public static class ConfigureInMemoryPluginExtensions
{
    public static IServiceCollection AddInMemoryPluginServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPluginRepository, PluginRepository>()
            .AddSingleton<IPluginRegistrationService, PluginRegistrationService>();
    }
}
