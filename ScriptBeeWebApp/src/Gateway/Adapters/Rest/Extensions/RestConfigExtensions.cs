using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Ports.Instance;

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
            .AddSingleton<IGetInstanceContext, GetInstanceContextAdapter>()
            .AddSingleton<IGetInstanceContextGraph, GetInstanceContextGraphAdapter>()
            .AddSingleton<IClearInstanceContext, ClearInstanceContextAdapter>()
            .AddSingleton<ILinkInstanceContext, LinkInstanceContextAdapter>()
            .AddSingleton<ILoadInstanceContext, LoadInstanceContextAdapter>()
            .AddSingleton<IGenerateInstanceClasses, GenerateInstanceClassesAdapter>()
            .AddSingleton<ITriggerInstanceAnalysis, TriggerInstanceAnalysisAdapter>();
    }
}
