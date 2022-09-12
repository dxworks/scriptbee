using System.Collections.Generic;

namespace ScriptBee.Plugin;

public interface IPluginReader
{
    IEnumerable<Models.Plugin> ReadPlugins(string pluginFolderPath);
}
