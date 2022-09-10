using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptBee.Plugin;

public class PluginPathReader : IPluginPathReader
{
    private readonly string _pathToPlugins;

    public PluginPathReader(string pathToPlugins)
    {
        _pathToPlugins = pathToPlugins;
    }

    public List<string> GetPluginPaths()
    {
        if (Directory.Exists(_pathToPlugins))
        {
            return Directory.GetFiles(_pathToPlugins, "*.dll").ToList();
        }

        return new List<string>();
    }
}
