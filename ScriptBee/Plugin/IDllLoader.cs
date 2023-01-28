using System;
using System.Collections.Generic;

namespace ScriptBee.Plugin;

public interface IDllLoader
{
    IEnumerable<(Type @interface, Type concrete)> LoadDllTypes(string fullPathToDll, ISet<Type> acceptedPluginTypes);
}
