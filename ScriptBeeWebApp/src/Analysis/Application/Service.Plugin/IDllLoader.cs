namespace ScriptBee.Service.Plugin;

public interface IDllLoader
{
    IEnumerable<(Type @interface, Type concrete)> LoadDllTypes(
        string fullPathToDll,
        ISet<Type> acceptedPluginTypes
    );
}
