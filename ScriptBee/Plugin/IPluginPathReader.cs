using System.Collections.Generic;

namespace ScriptBee.Plugin;

public interface IPluginPathReader
{
    public List<string> GetPluginPaths();
}
