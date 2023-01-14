using System.Collections.Generic;

namespace ScriptBee.Plugin;

public interface IPluginReader
{
    Models.Plugin? ReadPlugin(string pluginPath);

    IEnumerable<Models.Plugin> ReadPlugins(string pluginFolderPath);
}
