using System.Collections.Generic;

namespace ScriptBee.Plugins
{
    public interface IPluginLoader
    {
        public List<string> GetPluginPaths();
    }
}