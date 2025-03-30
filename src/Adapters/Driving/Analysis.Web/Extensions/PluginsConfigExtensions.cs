using ScriptBee.Persistence.InMemory.Extensions;
using ScriptBee.Service.Plugin.Extensions;

namespace ScriptBee.Analysis.Web.Extensions;

public static class PluginsConfigExtensions
{
    public static IServiceCollection AddPluginsConfig(this IServiceCollection services)
    {
        return services.AddInMemoryPluginServices().AddPluginServices();
    }
}
