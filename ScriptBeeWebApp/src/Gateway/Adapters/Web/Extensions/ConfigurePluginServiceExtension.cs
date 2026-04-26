using ScriptBee.Plugins;
using ScriptBee.Plugins.Extensions;
using ScriptBee.Plugins.Installer.Extensions;
using ScriptBee.Plugins.Loader.Extensions;
using ScriptBee.Service.Gateway.Plugins;
using ScriptBee.UseCases.Gateway.Plugins;
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
            .AddSingleton<PluginPathProvider>()
            .AddSingleton<IPluginPathProvider>(sp => sp.GetRequiredService<PluginPathProvider>())
            .AddSingleton<IGatewayPluginPathProvider>(sp =>
                sp.GetRequiredService<PluginPathProvider>()
            )
            .AddPluginReader()
            .AddPluginInstaller()
            .AddPluginLoader();
    }
}
