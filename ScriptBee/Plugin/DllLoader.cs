using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScriptBee.Plugin;

public class DllLoader : IDllLoader
{
    public IEnumerable<Type> LoadDllTypes(string fullPathToDll, ISet<Type> acceptedPluginTypes)
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

        var pluginDll = Assembly.LoadFrom(fullPathToDll);

        // todo take into consideration the plugin manifest type to support more than just dlls
        var acceptedTypes = pluginDll.GetExportedTypes()
            .Where(type => acceptedPluginTypes.Any(t => t.IsAssignableFrom(type)));

        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;

        return acceptedTypes;
    }

    private static Assembly CurrentDomainOnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        return ((AppDomain)sender).GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
    }
}
