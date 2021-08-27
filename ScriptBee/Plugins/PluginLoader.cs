using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptBee.Plugins
{
    public class PluginLoader : IPluginLoader
    {
        private readonly string _pathToPlugins;

        public PluginLoader(string pathToPlugins)
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
}