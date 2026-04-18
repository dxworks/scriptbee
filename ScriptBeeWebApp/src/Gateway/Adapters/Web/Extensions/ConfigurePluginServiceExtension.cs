using ScriptBee.Plugins.Installer;
using ScriptBee.Plugins.Installer.Extensions;
using ScriptBee.Plugins.Loader.Extensions;
using ScriptBee.Web.Config;
using ScriptBee.Web.Services;

namespace ScriptBee.Web.Extensions;

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
            .AddPluginInstaller()
            .AddPluginLoader();
    }
}
