using System;
using System.Collections.Generic;

namespace ScriptBee.Plugin;

public interface IDllLoader
{
    IEnumerable<Type> LoadDllTypes(string fullPathToDll, ISet<Type> acceptedPluginTypes);
}
