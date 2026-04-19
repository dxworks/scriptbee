using ScriptBee.Plugins;
using ScriptBee.Plugins.Extensions;
using ScriptBee.Plugins.Loader.Extensions;
using ScriptBee.Service.Analysis;
using ScriptBee.Service.Analysis.Config;

namespace ScriptBee.Analysis.Web.Extensions;

public static class ConfigurePluginServiceExtension
{
    public static IServiceCollection AddPluginServices(
        this IServiceCollection services,
        string pluginConfigurationSection
    )
    {
        services.AddOptions<PluginsSettings>().BindConfiguration(pluginConfigurationSection);
        return services
            .AddSingleton<IPluginPathProvider, PluginPathProvider>()
            .AddPluginReader()
            .AddPluginLoader();
    }
}
