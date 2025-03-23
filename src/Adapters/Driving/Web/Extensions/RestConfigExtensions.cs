using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Plugins;
using ScriptBee.Rest;

namespace ScriptBee.Web.Extensions;

public static class RestConfigExtensions
{
    public static IServiceCollection AddRestConfig(this IServiceCollection services)
    {
        return services
            .AddHttpClient()
            .AddSingleton<IGetPlugins, GetPluginsAdapter>()
            .AddSingleton<IGetScriptLanguages, GetScriptLanguagesAdapter>()
            .AddSingleton<IClearInstanceContext, ClearInstanceContextAdapter>()
            .AddSingleton<ILinkInstanceContext, LinkInstanceContextAdapter>()
            .AddSingleton<ILoadInstanceContext, LoadInstanceContextAdapter>();
    }
}
