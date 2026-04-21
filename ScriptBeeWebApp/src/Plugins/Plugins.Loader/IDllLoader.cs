namespace ScriptBee.Plugins.Loader;

internal interface IDllLoader
{
    LoadedPlugin LoadDllTypes(string fullPathToDll, ISet<Type> acceptedPluginTypes);
}
