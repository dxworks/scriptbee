using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Service.Plugin.Extensions;

public static class ConfigurePluginServiceExtension
{
    public static IServiceCollection AddPluginServices(this IServiceCollection services)
    {
        return services.AddSingleton<IDllLoader, DllLoader>();
    }
}
