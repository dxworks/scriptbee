using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Plugins;

public interface IPluginReader
{
    Plugin? ReadPlugin(string pluginPath);

    IEnumerable<Plugin> ReadPlugins(string pluginFolderPath);
}
