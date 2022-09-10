using System;
using System.Linq;
using System.Reflection;
using ScriptBee.FileManagement;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginManager
{
    private readonly ILogger _logger;
    private readonly IPluginManifestReader _pluginManifestReader;
    private readonly IPluginLoaderFactory _pluginLoaderFactory;

    public PluginManager(ILogger logger, IPluginManifestReader pluginManifestReader,
        IPluginLoaderFactory pluginLoaderFactory)
    {
        _pluginManifestReader = pluginManifestReader;
        _pluginLoaderFactory = pluginLoaderFactory;
        _logger = logger;
    }

    public void LoadPlugins(string pluginFolder)
    {
        var pluginManifests = _pluginManifestReader.ReadManifests(pluginFolder);

        Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return ((AppDomain)sender).GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
        }

        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

        foreach (var pluginManifest in pluginManifests)
        {
            var pluginDll = Assembly.LoadFrom(pluginManifest.EntryPoint);

            // todo take into consideration the plugin manifest type to support more than just dlls
            foreach (var type in pluginDll.GetExportedTypes())
            {
                var pluginLoader = _pluginLoaderFactory.GetPluginLoader(type);
                if (pluginLoader is null)
                {
                    _logger.Warning("Plugin loader not found for type {type}", type);
                }
                else
                {
                    pluginLoader.LoadPlugin(pluginManifest, type);
                }
            }
        }

        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
    }
}
