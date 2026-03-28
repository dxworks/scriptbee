using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Rest.Extensions;

public static class RestConfigExtensions
{
    public static IServiceCollection AddRestConfig(this IServiceCollection services)
    {
        return services
            .AddHttpClient()
            .AddSingleton<IGetPlugins, GetPluginsAdapter>()
            .AddSingleton<IInstallPlugin, InstallPluginAdapter>()
            .AddSingleton<IUninstallPlugin, UninstallPluginAdapter>()
            .AddSingleton<IGetScriptLanguages, GetScriptLanguagesAdapter>()
            .AddSingleton<IGetInstanceContext, GetInstanceContextAdapter>()
            .AddSingleton<IClearInstanceContext, ClearInstanceContextAdapter>()
            .AddSingleton<ILinkInstanceContext, LinkInstanceContextAdapter>()
            .AddSingleton<ILoadInstanceContext, LoadInstanceContextAdapter>();
    }
}
