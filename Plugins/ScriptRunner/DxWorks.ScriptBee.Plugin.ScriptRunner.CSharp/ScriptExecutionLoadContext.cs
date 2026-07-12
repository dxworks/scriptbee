using System.Reflection;
using System.Runtime.Loader;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp;

internal class ScriptExecutionLoadContext(AssemblyLoadContext pluginContext)
    : AssemblyLoadContext("ScriptExecution", isCollectible: true)
{
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var loadedInPlugin = pluginContext.Assemblies.FirstOrDefault(a =>
            a.GetName().Name == assemblyName.Name
        );

        if (loadedInPlugin != null)
        {
            return loadedInPlugin;
        }

        try
        {
            return pluginContext.LoadFromAssemblyName(assemblyName);
        }
        catch
        {
            return null;
        }
    }
}
