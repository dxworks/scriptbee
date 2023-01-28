using System;
using System.Collections.Generic;

namespace ScriptBee.Plugin;

public interface IPluginRegistrationService
{
    void Add(string pluginKind, HashSet<Type> acceptedTypes);

    bool TryGetValue(string pluginKind, out HashSet<Type>? acceptedTypes);
}
