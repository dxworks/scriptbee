using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Loader;

internal interface IPluginRegistry
{
    void RegisterPlugin(Plugin plugin, LoadedPlugin loadedPlugin);

    void RegisterPlugin(Plugin plugin);

    void UnRegisterPlugin(PluginId pluginId);
}
