using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Common.Plugins.Config;

namespace ScriptBee.Common.Plugins.Extensions;

public static class PluginExtensions
{
    public static IServiceCollection AddPlugins(
        this IServiceCollection services,
        string pluginConfigurationSection
    )
    {
        services.AddOptions<PluginsSettings>().BindConfiguration(pluginConfigurationSection);

        return services.AddSingleton<IPluginPathProvider, PluginPathProvider>();
    }
}
