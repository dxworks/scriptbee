namespace ScriptBee.Plugins.Loader;

public interface IDllLoader
{
    IEnumerable<(Type @interface, Type concrete)> LoadDllTypes(
        string fullPathToDll,
        ISet<Type> acceptedPluginTypes
    );
}
