using System.Reflection;
using System.Runtime.Loader;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Plugins.Loader;

internal class PluginAssemblyLoadContext(string pluginPath)
    : AssemblyLoadContext(isCollectible: true)
{
    private static readonly string PluginApiAssemblyName =
        typeof(IPlugin).Assembly.GetName().Name!;

    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name == PluginApiAssemblyName)
        {
            return Default.Assemblies.FirstOrDefault(a => a.GetName().Name == PluginApiAssemblyName);
        }

        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}
