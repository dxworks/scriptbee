using System.Collections.Generic;

namespace ScriptBee.PluginManager
{
    public interface IPluginPathReader
    {
        public List<string> GetPluginPaths();
    }
}