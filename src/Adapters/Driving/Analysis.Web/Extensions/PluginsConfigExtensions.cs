using ScriptBee.Persistence.InMemory;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Analysis.Web.Extensions;

public static class PluginsConfigExtensions
{
    public static IServiceCollection AddPluginsConfig(this IServiceCollection services)
    {
        return services.AddSingleton<IPluginRepository, PluginRepository>();
    }
}
