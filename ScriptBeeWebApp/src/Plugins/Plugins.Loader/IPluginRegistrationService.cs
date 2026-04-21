namespace ScriptBee.Plugins.Loader;

internal interface IPluginRegistrationService
{
    void Add(string pluginKind, HashSet<Type> acceptedTypes);

    bool TryGetValue(string pluginKind, out HashSet<Type>? acceptedTypes);
}
