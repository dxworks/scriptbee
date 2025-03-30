using Microsoft.Extensions.DependencyInjection;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin.Extensions;

public static class ConfigurePluginServiceExtension
{
    public static IServiceCollection AddPluginServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDllLoader, DllLoader>()
            .AddSingleton<IPluginLoader, PluginLoader>()
            .AddSingleton<IManagePluginsUseCase, PluginManager>();
    }
}
