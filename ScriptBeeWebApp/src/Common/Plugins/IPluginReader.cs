using ScriptBee.Domain.Model.Plugin;

namespace ScriptBee.Common.Plugins;

public interface IPluginReader
{
    Plugin? ReadPlugin(string pluginPath);

    IEnumerable<Plugin> ReadPlugins(string pluginFolderPath);
}
