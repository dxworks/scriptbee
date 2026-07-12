using System.Reflection;
using System.Runtime.Loader;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

internal class ScriptExecutionLoadContext(AssemblyLoadContext pluginContext)
    : AssemblyLoadContext("ScriptExecution", isCollectible: true)
{
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (pluginContext.IsCollectible && !pluginContext.Assemblies.Any())
        {
            return Default.LoadFromAssemblyName(assemblyName);
        }
        try
        {
            var loadedInPlugin = pluginContext.Assemblies.FirstOrDefault(a =>
                a.GetName().Name == assemblyName.Name
            );

            return loadedInPlugin != null
                ? loadedInPlugin
                : pluginContext.LoadFromAssemblyName(assemblyName);
        }
        catch (InvalidOperationException) { }

        return Default.LoadFromAssemblyName(assemblyName);
    }
}
