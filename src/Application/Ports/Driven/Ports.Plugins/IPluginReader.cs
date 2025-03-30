using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Ports.Plugins;

public interface IPluginReader
{
    Plugin? ReadPlugin(string pluginPath);

    IEnumerable<Plugin> ReadPlugins(string pluginFolderPath);
}
