﻿using System.Collections.Concurrent;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Persistence.InMemory;

public class PluginRegistrationService : IPluginRegistrationService
{
    private readonly ConcurrentDictionary<string, HashSet<Type>> _pluginRegistrations = new();

    public void Add(string pluginKind, HashSet<Type> acceptedTypes)
    {
        _pluginRegistrations.AddOrUpdate(pluginKind, acceptedTypes, (_, _) => acceptedTypes);
    }

    public bool TryGetValue(string pluginKind, out HashSet<Type>? acceptedTypes)
    {
        return _pluginRegistrations.TryGetValue(pluginKind, out acceptedTypes);
    }
}
