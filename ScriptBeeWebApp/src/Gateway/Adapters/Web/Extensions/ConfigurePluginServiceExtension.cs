using ScriptBee.Plugins;
using ScriptBee.Plugins.Extensions;
using ScriptBee.Plugins.Installer.Extensions;
using ScriptBee.Plugins.Loader.Extensions;
using ScriptBee.Service.Project.Plugins;
using ScriptBee.UseCases.Project.Plugins;
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
            .AddSingleton<IManagePluginsUseCase, PluginManager>()
            .AddSingleton<IPluginPathProvider, PluginPathProvider>()
            .AddPluginReader()
            .AddPluginInstaller()
            .AddPluginLoader();
    }
}
