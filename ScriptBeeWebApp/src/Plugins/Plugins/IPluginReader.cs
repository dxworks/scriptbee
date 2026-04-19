using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins;

public interface IPluginReader
{
    Plugin? ReadPlugin(string pluginFolderPath, PluginId pluginId);

    Plugin? ReadPlugin(string pluginPath);

    IEnumerable<Plugin> ReadPlugins(string pluginFolderPath);
}
