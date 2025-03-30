using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Ports.Plugins;

public interface IPluginLoader
{
    void Load(Plugin plugin);
}
