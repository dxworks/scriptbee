namespace ScriptBee.Ports.Plugins;

public interface IPluginRegistrationService
{
    void Add(string pluginKind, HashSet<Type> acceptedTypes);

    bool TryGetValue(string pluginKind, out HashSet<Type>? acceptedTypes);
}
