namespace ScriptBee.Plugins.Loader;

internal class DllLoader : IDllLoader
{
    public LoadedPlugin LoadDllTypes(string fullPathToDll, ISet<Type> acceptedPluginTypes)
    {
        var context = new PluginAssemblyLoadContext(fullPathToDll);

        var pluginDll = context.LoadFromAssemblyPath(fullPathToDll);

        var acceptedTypes = (
            from exportedType in pluginDll.GetExportedTypes()
            from acceptedPluginType in acceptedPluginTypes
            where acceptedPluginType.IsAssignableFrom(exportedType)
            select (acceptedPluginType, exportedType)
        ).ToList();

        return new LoadedPlugin(context, acceptedTypes);
    }
}
