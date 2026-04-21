using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Loader;

public interface IPluginLoader
{
    void Load(Plugin plugin);

    void Unload(PluginId pluginId);
}
