using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Loader;

public interface IPluginRepository
{
    TService? GetPlugin<TService>(
        Func<TService, bool> filter,
        IEnumerable<(Type @interface, object instance)>? services = null
    )
        where TService : IPlugin;

    IEnumerable<TService> GetPlugins<TService>(
        IEnumerable<(Type @interface, object instance)>? services = null
    )
        where TService : IPlugin;

    IEnumerable<Plugin> GetLoadedPlugins();
}
