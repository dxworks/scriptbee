using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScriptBee.Plugin;

public class DllLoader : IDllLoader
{
    public IEnumerable<(Type @interface, Type concrete)> LoadDllTypes(string fullPathToDll,
        ISet<Type> acceptedPluginTypes)
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

        var pluginDll = Assembly.LoadFrom(fullPathToDll);

        var acceptedTypes = new List<(Type @interface, Type concrete)>();

        foreach (var exportedType in pluginDll.GetExportedTypes())
        {
            foreach (var acceptedPluginType in acceptedPluginTypes)
            {
                if (acceptedPluginType.IsAssignableFrom(exportedType))
                {
                    acceptedTypes.Add((acceptedPluginType, exportedType));
                }
            }
        }

        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;

        return acceptedTypes;
    }

    private static Assembly CurrentDomainOnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        return ((AppDomain)sender).GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
    }
}
