using ScriptBee.Common.Plugins.Extensions;

namespace ScriptBee.Web.Exceptions;

public static class PluginExtensions
{
    public static IServiceCollection AddPluginsServices(this IServiceCollection services)
    {
        return services.AddPlugins("ScriptBee:Plugins");
    }
}
