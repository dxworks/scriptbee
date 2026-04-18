using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Plugins.Loader;

public interface IPluginLoader
{
    void Load(Plugin plugin);
}
